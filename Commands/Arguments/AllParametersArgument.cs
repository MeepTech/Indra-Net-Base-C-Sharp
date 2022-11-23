using System.Linq;

namespace Indra.Net {
  /// <summary>
  /// An argument representing all passed in yaml argument parameters
  /// </summary>
  public class AllParametersArgument : Argument {

    ///<summary><inheritdoc/></summary>
    public AllParametersArgument(object? @default = null) 
      : base("_args", Enumerable.Empty<string>(), false, @default) {}

    ///<summary><inheritdoc/></summary>
    public override object? GetValue(Command commandData) 
      => commandData.Yaml ?? Default;
  }
}