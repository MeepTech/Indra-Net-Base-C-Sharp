using System.Collections.Generic;
using System.Linq;

namespace Indra.Net {

  /// <summary>
  /// Marks an argument for all flags in a collection.
  /// </summary>
  public class AllFlagsArgument : Argument {

    /// <summary>
    /// If all flags are grabbed, should they be ordered?
    /// Defaults to true. If false, a hash-set is provided instead.
    /// </summary>
    public bool Ordered {
      get;
      init;
    } = true;

    /// <summary>
    /// Make a new all args argument.
    /// </summary>
    public AllFlagsArgument(bool isOrdered = true) 
      : base("_flags", Enumerable.Empty<string>(), false, isOrdered ? new List<string>() : new HashSet<string>()) {}

    ///<summary><inheritdoc/></summary>
    public override object? GetValue(Command commandData)
      => (Ordered
        ? (object?)commandData.OrderedFlags
        : commandData.AllFlags
      ) ?? UseDefault;
  }
}