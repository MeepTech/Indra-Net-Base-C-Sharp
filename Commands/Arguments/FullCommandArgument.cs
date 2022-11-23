using System.Linq;

namespace Indra.Net {
  /// <summary>
  /// USed to get the full command data provided.
  /// </summary>
  public class FullCommandArgument : Argument {

    ///<summary><inheritdoc/></summary>
    public FullCommandArgument() : base("_string", Enumerable.Empty<string>(), true) {
    }

    ///<summary><inheritdoc/></summary>
    public override object? GetValue(Command commandData)
      => commandData ?? UseDefault;
  }
}