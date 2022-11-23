using System.Collections.Generic;
using System.IO;

namespace Indra.Net {


  /// <summary>
  /// Attribute to mark a flag argument
  /// </summary>
  public class FlagsArgument : Argument {

    /// <summary>
    /// The index the flag should be found in, if we cant find it by name.
    /// </summary>
    public int? ExpectedIndex {
      get;
      internal set;
    }

    /// <summary>
    /// Make a new flag argument
    /// </summary>
    public FlagsArgument(string? key, IEnumerable<string> aliases, bool isRequired = false, object? @default = null, int? expectedIndex = null)
      : base(
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
    /// <inheritdoc/>
    /// </summary>
    /// <returns>
    /// A bool for the desired flag being provided if a key is provided
    ///   a string or null if an index is provided,
    ///   or readonly string list of all provided flags in order if no arguments are provided
    ///     (or a hash set if Ordered is false).
    /// 
    /// Base:
    /// </returns>
    public override object? GetValue(Command commandData)
      => Key is null
        ? ExpectedIndex.HasValue
          ? commandData.OrderedFlags.Count > ExpectedIndex.Value
            ? !(bool)Default!
            : UseDefault
          : throw new InvalidDataException()
        : commandData.AllFlags.Contains(Key)
         ? !(bool)Default!
         : UseDefault;
  }
}