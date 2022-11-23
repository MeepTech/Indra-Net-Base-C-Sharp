using Indra.Net.Config;
using System;
using System.Collections.Generic;
using System.Reflection;
using YamlDotNet.RepresentationModel;

namespace Indra.Net.Actions {

  /// <summary>
  /// Actions built from methods in c# plugins
  /// </summary>
  public class MethodBasedAction : ReflectionBasedAction {

    /// <summary>
    /// The method this action is built from.
    /// </summary>
    public MethodInfo Method
      => (MethodInfo)Member;

    /// <summary>
    /// Used to make an action from a plugin method.
    /// </summary>
    public MethodBasedAction(
      string key,
      IEnumerable<string> aliases,
      IEnumerable<Argument> arguments,
      MethodInfo method
    ) : base(key, aliases, arguments, method) {}

    /// <summary>
    /// Execute the command for the bound focus.
    /// </summary>
    protected override Action.Result ExecuteFor(Command command) {
      // initialize arguments
      List<object?> parameters = new();
      foreach (Argument argument in Arguments.Values) {
        // get
        object? value = argument.GetValue(command);

        // validate if is required
        if (argument.IsRequired && value is Argument.NotProvided) {
          return new(
            Message: $"Missing required Argument: {argument}, for Action: {Key}, on Focus: {Focus!.Key}|{Focus!.Id}",
            Success: false
          );
        }

        // collect
        parameters.Add(value);
      }

      // invoke
      return new (Method.Invoke(Focus, parameters.ToArray()));
    }

    /// <summary>
    /// Build a new unbound action from a method, with the attribute provided (doesn't check if not provided/null)
    /// </summary>
    public static MethodBasedAction FromMethod(MethodInfo method, ActionAttribute actionAttribute, IReadOnlyDictionary<string, object> services)
      => _buildActionFromMethod(method, actionAttribute, services);

    /// <summary>
    /// Build a new type of action from a method (checks for the attribute automatically)
    /// </summary>
    public static MethodBasedAction FromMethod(MethodInfo method, IReadOnlyDictionary<string, object> services)
      => _buildActionFromMethod(method, method.GetCustomAttribute<ActionAttribute>(), services);

    static MethodBasedAction _buildActionFromMethod(MethodInfo method, ActionAttribute? actionAttribute, IReadOnlyDictionary<string, object> services) {
      string actionKey;
      string[] actionAliases;
      if (actionAttribute is not null) {
        actionAttribute._init(method);
        actionKey = actionAttribute.Key!;
        actionAliases = actionAttribute.Aliases;
      } else {
        (actionKey, actionAliases) = ActionAttribute._Init(method);
      }

      List<Argument> arguments = new();
      foreach (var param in method.GetParameters()) {
        ArgumentAttribute? argumentAttribute = param.GetCustomAttribute<ArgumentAttribute>();

        if (argumentAttribute is not null) {
          argumentAttribute._init(services, param);
          arguments.Add(argumentAttribute._buildArgument());

          continue;
        } // infered types:
        else {
          // actor
          if (param.ParameterType.IsAssignableTo(typeof(IActor))) {
            arguments.Add(new ActorArgument());
            continue;
          } // location
          else if (param.ParameterType.IsAssignableTo(typeof(IPlace))) {
            arguments.Add(new LocationArgument());
            continue;
          } // full arguments string
          else if (param.ParameterType.IsAssignableTo(typeof(string))) {
            arguments.Add(new FullStringArgument());
            continue;
          }
          else if (param.ParameterType.IsAssignableTo(typeof(Command))) {
            arguments.Add(new FullCommandArgument());
            continue;
          }
          else if (param.ParameterType.IsAssignableTo(typeof(ISet<string>))) {
            arguments.Add(new AllFlagsArgument(false));
            continue;
          }
          else if (param.ParameterType.IsAssignableTo(typeof(IReadOnlyList<string>))) {
            arguments.Add(new AllFlagsArgument());
            continue;
          }
          else if (param.ParameterType.IsAssignableTo(typeof(YamlNode))) {
            arguments.Add(new AllParametersArgument(new YamlMappingNode()));
            continue;
          }
          else {
            throw new NotSupportedException();
          }
        }
      }

      return new MethodBasedAction(actionKey, actionAliases, arguments, method);
    }
  }
}