using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Indra.Net.Actions {
  /// <summary>
  /// An action made from plugin reflection properties.
  /// </summary>
  public abstract class ReflectionBasedAction : Action {

    /// <summary>
    /// The member this reflection based action is for.
    /// TODO: this should be keyed somehow and found via the key.
    /// </summary>
    public MemberInfo Member {
      get;
    }

    internal ReflectionBasedAction(string key, IEnumerable<string> aliases, IEnumerable<Argument> arguments, MemberInfo member)
      : base(key, aliases, arguments) {
      Member = member;
    }

    /// <summary>
    /// Normalize the key for a c# item into it's natural aliases.
    /// </summary>
    public static (string key, string[] aliases) NormalizeKey(string baseKey, IEnumerable<string>? existingAliases = null) {
      string lowerMethodNameWithUnderscores = baseKey.ToLower();
      string actionKey = lowerMethodNameWithUnderscores.Replace('_', '-');
      var actionAliases = new string[] {
        actionKey,
        lowerMethodNameWithUnderscores
      };
      var aliases = actionAliases.Concat(existingAliases ?? Enumerable.Empty<string>()).Distinct().ToArray();

      return (actionKey, aliases);
    }
  }
}
