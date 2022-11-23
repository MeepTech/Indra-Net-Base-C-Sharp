using System.Linq;

namespace Indra.Net {

  /// <summary>
  /// The argument for the location where the command was executed from.
  /// </summary>
  public class LocationArgument : Argument {

    /// <summary>
    /// Make a special location argument
    /// </summary>
    public LocationArgument()
      : base("_location", Enumerable.Empty<string>(), true) { }

    ///<summary><inheritdoc/></summary>
    public override object? GetValue(Command commandData)
      => commandData.Location ?? UseDefault;
  }
}