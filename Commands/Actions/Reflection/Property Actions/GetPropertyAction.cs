using System;
using System.Collections.Generic;
using System.Reflection;

namespace Indra.Net.Actions {

  /// <summary>
  /// Get the value of a property on a focus with an action
  /// </summary>
  public class GetPropertyAction : MethodBasedAction {

    /// <summary>
    /// The get method.
    /// </summary>
    public MethodInfo GetMethod
      => Method;

    internal GetPropertyAction(
      string key, 
      IEnumerable<string> aliases,
      MethodInfo getMethod
    ) : base(key, aliases, Array.Empty<Argument>(), getMethod) {}

    ///<summary><inheritdoc/></summary>
    protected override Result ExecuteFor(Command command) {
      return new Result() {
        ReturnValue = GetMethod.Invoke(command.Focus, Array.Empty<object>()),
        Success = true
      };
    }
  }
}
