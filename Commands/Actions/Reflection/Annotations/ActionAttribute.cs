using Indra.Net.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Indra.Net.Config {

  /// <summary>
  /// Can be added to a get method of a map/IDictionary to indicate an action on the object to get the list of items should be created.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, Inherited = true)]
  public class GetMapActionAttribute: ActionAttribute { }

  /// <summary>
  /// Can be added to a set method of an IList to indicate an action on the object to set and clear the entire list of items
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, Inherited = true)]
  public class SetListActionAttribute : ActionAttribute { 
    
    
  }

  /// <summary>
  /// Used to indicate a method should be exposed as an action.
  /// The first use of an IPlace parameter will be bound to the execution location,
  ///    the first use of an IActor parameter will be bound to the execution actor,
  ///    and 'this' is automatically bound to the execution focus.
  /// Alternatively; you can accept an ICommandData object, or an object of
  ///    the expected type of command data: ex: Action.CommandData.
  /// 
  /// See the other attributes for the binding of user provided action arguments and flags.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, Inherited = true)]
  public class ActionAttribute : Attribute {
    IEnumerable<string> _aliases = null!;

    /// <summary>
    /// The secondary aliases of the action
    /// (each must be unique on the foucs+sub-type)
    /// TODO: test if enumerable works instead of list for the attribute set syntax.
    /// </summary>
    public string[] Aliases {
      get => _aliases.ToArray();
      init => _aliases ??= value;
    }

    /// <summary>
    /// The base/main name of the action (must be unique on the foucs+sub-type)
    /// </summary>
    public string? Key { 
      get => _key; 
      init => _key = value;
    } string? _key = null;

    /// <summary>
    /// All potential names and aliases for this action
    /// (each must be unique on the foucs+sub-type)
    /// </summary>
    public IReadOnlyList<string> AllNames
      => Aliases.Append(Key).ToList()!;

    /// <summary>
    /// Make a new action from a function.
    /// </summary>
    public ActionAttribute(params string[] aliases) {
      Key = null;
      Aliases ??= aliases;
    }

    internal void _init(MethodInfo method) {
      var (actionKey, aliases) = _Init(method, this);

      _key = actionKey;
      _aliases = aliases;
    }

    internal static (string, string[]) _Init(MethodInfo method, ActionAttribute? @this = null) {
      string baseKey = @this?.Key ?? method.Name;
      return ReflectionBasedAction.NormalizeKey(baseKey, @this?.Aliases);
    }
  }
}