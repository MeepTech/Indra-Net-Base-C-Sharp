using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using Indra.Net.Config;
using System.Text.Json.Nodes;

namespace Indra.Net {

  /// <summary>
  /// A saved copy of a type of object, used to make more copies of the object.
  /// </summary>
  public sealed class Prototype : Focus<Prototype>, ICreateable, IStorable {

    /// <summary>
    /// The base of the prototype's data.
    /// </summary>
    public JsonObject Data
      => (JsonObject)JsonNode.Parse(_base.ToJsonString())!;
    [Column("original")] JsonObject _base;

    /// <summary>
    /// The original object's id.
    /// </summary>
    public string OriginalId 
      { get; private set; }

    /// <summary>
    /// The focus type of this prototyped object
    /// </summary>
    public string OriginalFocusType
      => _base[nameof(IPrototypeable.FocusType)]!.GetValue<string>();

    /// <summary>
    /// The sub type of this prototyped object
    /// </summary>
    public IReadOnlyList<string> OriginalSubTypes
      => _base[nameof(IPrototypeable.FocusType)]!
        .AsArray()
        .Select(s
          => s!.GetValue<string>())
        .ToList();

    ///<summary><inheritdoc/></summary>
    public Prototype(string key, IActor creator, [NotNull] IPrototypeable @base) : base(key) {
      Owner = Creator = creator;
      _base = (JsonObject)JsonNode.Parse(JsonSerializer.Serialize(@base))!;
      _base.Remove("id");
      _base.Remove("location");
    }

    /// <summary>
    /// Make a copy of the current prototyped object.
    /// </summary>
    public IPrototypeable Make(IActor creator, [Flags("theirs")] bool replaceOwnership = true) {
      var clone = (IPrototypeable)
        JsonSerializer.Deserialize(
          _base,
          Types[OriginalFocusType]
        )!;

      clone.Id = System.Guid.NewGuid().ToString();
      clone.Creator = creator;
      clone.Base = this;

      if (clone is Focus focus) {
        focus.Location = null;
      }

      if (replaceOwnership) {
        clone.Owner = creator;
      }

      return clone;
    }
  }
}
