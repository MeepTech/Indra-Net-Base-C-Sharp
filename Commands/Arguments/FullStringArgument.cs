using System.Linq;

namespace Indra.Net {
  /// <summary>
  /// USed to get the full arguments provided as a string.
  /// </summary>
  public class FullStringArgument : Argument {

    ///<summary><inheritdoc/></summary>
    public FullStringArgument(bool isRequired = false, string? @default = null) : base("_string", Enumerable.Empty<string>(), isRequired, @default) {
    }

    ///<summary><inheritdoc/></summary>
    public override object? GetValue(Command commandData)
      => string.IsNullOrEmpty(commandData.ArgumentString) 
        ? UseDefault
        : commandData.ArgumentString;
  }
}