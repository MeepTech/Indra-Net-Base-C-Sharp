using System;

namespace Indra.Net.Config {

  /// <summary>
  /// Indicates the actor argument. 
  /// Not needed if you place location after the actor.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
  public class ActorAttribute : ArgumentAttribute {

    ///<summary><inheritdoc/></summary>
    protected override Argument ArgumentConstructor(object? defaultValue = null)
      => new ActorArgument();
  }
}
