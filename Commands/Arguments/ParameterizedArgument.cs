using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.RepresentationModel;

namespace Indra.Net {

  /// <summary>
  /// An argument based on a passed in parameter value.
  /// These are parsed via loose YAML conventions
  /// </summary>
  public class ParameterizedArgument : Argument {

    /// <summary>
    /// Denotes the ordered arguments set within the named arguments object in the case hybrid arguments are provided.
    /// </summary>
    public const string SequentialArgumentsKey = "[_]";

    /// <summary>
    /// The index the item should be found at if the params are passed in ordered.
    /// </summary>
    public int? ExpectedIndex {
      get;
      internal set;
    }

    ///<summary><inheritdoc/></summary>
    public ParameterizedArgument(string? key, IEnumerable<string> aliases, object? @default = null, bool isRequired = false, int? expectedIndex = null) : base(
      key 
        ?? expectedIndex?.ToString() 
        ?? throw new System.ArgumentNullException("key or expectedIndex"),
      aliases,
      isRequired,
      @default
    ) {
      ExpectedIndex = expectedIndex;
    }

    /// <summary>
    /// Get either the value at the key, the index, or all values via yaml parsing.
    /// </summary>
    /// <returns>
    /// If key is set, it will grab by key and return the node at the value
    /// if index is set, it will grab by index and return the node at the value
    /// If neither are set it will return the full sequence/mapping node.
    /// </returns>
    public override object? GetValue(Command commandData) {
      if (!Keys.Any()) {
        if (!ExpectedIndex.HasValue) {
          throw new InvalidDataException();
        } else if (commandData.Yaml is YamlSequenceNode sequenceNode) {
          return sequenceNode[ExpectedIndex.Value];
        }

        if (commandData.Yaml is YamlMappingNode mappingNode
          && mappingNode.Children.TryGetValue("[_]", out var orderedItems)
          && orderedItems is YamlSequenceNode sequenceSubNode
        ) {
          return sequenceSubNode.Count() > ExpectedIndex.Value
            ? sequenceSubNode[ExpectedIndex.Value]
            : UseDefault;
        }
      }
      else {
        if (commandData.Yaml is YamlMappingNode mappingNode) {
          if (mappingNode.Children.TryGetValue(Key, out var node)) {
            return node;
          } else {
            var matchingKey = Aliases.FirstOrDefault(a => mappingNode.Children.ContainsKey(a));
            if (matchingKey is not null) { 
              var match = mappingNode.Children[matchingKey];
              return match;
            }
          }
          
          if (ExpectedIndex.HasValue
            && mappingNode.Children.TryGetValue("[_]", out var orderedItems)
            && orderedItems is YamlSequenceNode sequenceSubNode
          ) {
            return sequenceSubNode.Count() > ExpectedIndex.Value
              ? sequenceSubNode[ExpectedIndex.Value]
              : UseDefault;
          }
        }
        else if (ExpectedIndex.HasValue && commandData.Yaml is YamlSequenceNode sequenceNode) {
          return sequenceNode.Count() > ExpectedIndex.Value
              ? sequenceNode[ExpectedIndex.Value]
              : UseDefault;
        }
      }

      return UseDefault;
    }

    /// <summary>
    /// Parse yaml into a yaml sequence or mapping node.
    /// </summary>
    public static YamlNode ParseFullArgumentsFromYaml(string yamlText) {
      var stream = new YamlStream();

      // if it's an object
      if (yamlText.StartsWith("{")) {
        stream.Load(new StringReader(yamlText));
        return (YamlMappingNode)stream.Documents[0].RootNode;
      } // if it's a list
      else if (yamlText.StartsWith("[")) {
        stream.Load(new StringReader(yamlText));
        return (YamlSequenceNode)stream.Documents[0].RootNode;
      } // if it's regular yaml without the borders we need to see if it's a list or object:
      else if (new Regex("^[^ ]+:.+ ", RegexOptions.Singleline).IsMatch(yamlText)) {
        stream.Load(new StringReader("{" + yamlText + "}"));
        return (YamlMappingNode)stream.Documents[0].RootNode;
      }

      // maybe it's in (=) mode?
      var cropper = new StringClosureCropper(
        yamlText,
        ('"', '"'),
        ('`', '`'),
        ('\'', '\''),
        // TODO: process variables and action calls within parenthases: () with another cropping cycle, then do a third cropping cycle for the below elements:
        ('{', '}'),
        ('[', ']')
      );

      string fixedCroppedResult = cropper.CroppedResult.Replace("=", ":");
      string fixedYaml = string.Format(
        fixedCroppedResult,
        cropper.RemovedSections
          .Select(s =>
            s.delimiters.start + s.contents + s.delimiters.end)
              .ToArray()
      );

      // if it's quick (=) syntax => yaml with a full object and no positionals:
      if (new Regex("^[^ ]+:.+ ", RegexOptions.Singleline).IsMatch(fixedCroppedResult)) {
        stream.Load(new StringReader("{" + fixedYaml + "}"));
        return (YamlMappingNode)stream.Documents[0].RootNode;
      }

      // hybrid mode: /command arg1, arg2, namedArg(:|=)4 (a yaml map node with property: _ being the indexed items in a sequence sub-node.)
      int namedParamsStartIndex;
      if ((namedParamsStartIndex
        = new Regex(
          ",(?:(?:\\s+(?:[^\\s,]+))|(?:(?:[^\\s,]+))):(?:[^,\\s]+)"
        ).Match(fixedCroppedResult)?.Index ?? -1) != -1
      ) {
        string orderedParams = fixedCroppedResult.Substring(0, namedParamsStartIndex - 1);
        string namedParams = fixedCroppedResult.Substring(namedParamsStartIndex + 1);
        Match match;
        if ((match = new Regex("{([0-9]+)}").Match(namedParams))?.Index - 1 != -1) {
          int firstReplacementInNamedParams = int.Parse(match!.Captures[0].Value);
          orderedParams = string.Format(
            orderedParams,
            cropper.RemovedSections[..firstReplacementInNamedParams]
              .Select(s => s.delimiters.start + s.contents + s.delimiters.end)
          );
          namedParams = string.Format(
            namedParams,
            cropper.RemovedSections[firstReplacementInNamedParams..]
              .Select(s => s.delimiters.start + s.contents + s.delimiters.end)
          );
        }

        stream.Load(new StringReader("{" + namedParams + "}"));
        var named = (YamlMappingNode)stream.Documents[0].RootNode;
        stream.Load(new StringReader("[" + namedParams + "]"));
        var ordered = (YamlSequenceNode)stream.Documents[0].RootNode;
        named.Add(SequentialArgumentsKey, ordered);

        return named;
      }
      // else asume list and try to read as yaml list:
      else {
        stream.Load(new StringReader("[" + yamlText + "]"));
        return (YamlSequenceNode)stream.Documents[0].RootNode;
      }
    }
  }
}