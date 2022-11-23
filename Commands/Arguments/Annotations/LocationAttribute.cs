using System;

namespace Indra.Net.Config {
  /// <summary>
  /// Indicates the location argument. 
  /// Not needed if you place location after the actor.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
  public class LocationAttribute : ArgumentAttribute {

    ///<summary><inheritdoc/></summary>
    protected override Argument ArgumentConstructor(object? defaultValue = null)
      => new LocationArgument();
  }
}
