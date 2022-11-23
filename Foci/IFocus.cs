using System.Collections.Generic;

namespace Indra.Net {

  /**
   * Can be the focus of an action
   */
  public interface IFocus : IUnique {

    /**
     * The root type of focus
     */
    string FocusType {
      get;
    }

    /**
     * The permissions required to interact with this focus or it's children.
     */
    IReadOnlyDictionary<string, Permission> RequiredPermissions {
      get;
    }

    /**
     * All actions indexed by their keys and aiases. (may be duplicates under different keys/aliases)
     */
    IReadOnlyDictionary<string, Action> Actions {
      get;
    }
    /*
    /// <summary>
    /// Add an action to a focus.
    /// </summary>
    /// <returns></returns>
    [Action("add-permission", "require", "require-permissions", "requirePermissions", "add-p", "ap", "addp")]
    public Action.Result<Action> AddPermission(
      [Service] Configuration.DbContext dbContext,
      [Arg("permissions", "p", Index = 0, IsRequired = true)] string permissionsKey,
      IActor? actor = null,
      IPlace? location = null,
      [Flags] IReadOnlyList<string>? allOrderedFlags = null,
      [Arg] YamlMappingNode? allArgs = null
    );

    /// <summary>
    /// Add an action to a focus.
    /// </summary>
    [Action("add-action")]
    public Action.Result<Action> AddAction(
      [Service] Configuration.DbContext dbContext,
      [Arg("action", "from", "source", "a", Index = 0, IsRequired = true)] object actionSource,
      IActor? actor = null,
      IPlace? location = null,
      [Flags] IReadOnlyList<string>? allOrderedFlags = null,
      [Arg] YamlMappingNode? allArgs = null
    );*/
  }

  /**
   * Can be the focus of an action
   */
  public interface IFocus<T> : IFocus {

    /**
     * The root type of focus
     */
    string IFocus.FocusType
      => typeof(T).ToString().ToLower();
  }
}