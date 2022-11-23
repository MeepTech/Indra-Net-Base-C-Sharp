using System.Linq;

namespace Indra.Net {
  /// <summary>
  /// Indicates an argument for the actor/executor of an action.
  /// </summary>
  public class ActorArgument : Argument {

    /// <summary>
    /// Make an actor argument.
    /// </summary>
    public ActorArgument()
      : base("_actor", Enumerable.Empty<string>(), true) { }

    ///<summary><inheritdoc/></summary>
    public override object? GetValue(Command commandData)
      => commandData.Actor ?? UseDefault;
  }
}