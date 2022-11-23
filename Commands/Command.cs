using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace Indra.Net {

  /// <summary>
  /// Data parsed from a command line string.
  /// </summary>
  public abstract record Command {
    public const string CurrentServerFocusQuery = "/.";
    public const string LastFocusedObjectFocusQuery = "/..";
    public const string CurrentObjectFocusQueryKey = ".";
    public const char ObjectActionSeperatorCharacter = '.';

    /// <summary>
    /// The string after the commad invocation slash before the first space and any flags or parameters
    /// </summary>
    public string ActionWithTarget
      { get; private set; }

    /// <summary>
    /// A list of the flags provided to the command call, in order
    /// </summary>
    public IReadOnlyList<string> OrderedFlags
      { get; private set; }

    /// <summary>
    /// A hash-set of the flags provided to the command call
    /// </summary>
    public ISet<string> AllFlags 
      { get; private set; }

    /// <summary>
    /// the full unprocessed string of arguments
    /// </summary>
    public string ArgumentString 
      { get; private set; }

    /// <summary>
    /// The actor who called the command
    /// </summary>
    public IActor Actor
      => (IActor)_focusableActor!;
    Focus? _focusableActor { get; set; }

    /// <summary>
    /// The focus of the command (loads if not yet loaded)
    /// </summary>
    public Focus Focus {
      get;
      internal set;
    }

    /// <summary>
    /// The location of this command's execution.
    /// </summary>
    public Place Location 
      { get; private set; }

    /// <summary>
    /// Manage the yaml arguments
    /// </summary>
    public YamlNode Yaml
      => _yaml ??= ParameterizedArgument
        .ParseFullArgumentsFromYaml(ArgumentString
          .TrimStart());
    YamlNode _yaml
      = null!;

    /// <summary>
    /// Used to make a new type of command execution data
    /// </summary>
    protected Command(
      IActor actor,
      Place location,
      string actionWithTarget,
      IReadOnlyList<string> orderedFlags,
      ISet<string> allFlags,
      string argumentString
    ) {
      if (actor is Focus f) {
        _focusableActor = f;
      } else throw new System.NotSupportedException();

      Location = location;
      ActionWithTarget = actionWithTarget;
      OrderedFlags = orderedFlags;
      AllFlags = allFlags;
      ArgumentString = argumentString;
    } Command() { }
  }
}