namespace Indra.Net.Config {

  /// <summary>
  /// An action that modifies some value via a setter.
  /// This attribute auto-includes the following ActionAttributes:
  /// Set,
  /// Clear,
  /// Update,
  /// AddToList (if list),
  /// RemoveFromList (if list),
  /// UpdateInList (if list),
  /// UpsertToList (if list)
  /// AddToMap (if list),
  /// RemoveFromMap (if list),
  /// UpdateInMap (if list),
  /// UpsertToMap (if list)
  /// </summary>
  public class ModifyActionAttribute 
    : ActionAttribute 
  {

    /// <summary>
    /// Include the properties from the SetActionAttribute
    /// </summary>
    public virtual bool IncludeSetAction {
      get; init;
    } = true;

    /// <summary>
    /// Include the properties from the UpdateActionAttribute
    /// </summary>
    public virtual bool IncludeUpdateAction {
      get; init;
    } = true;

    /// <summary>
    /// Include the properties from the ClearActionAttribute
    /// </summary>
    public virtual bool IncludeClearAction {
      get; init;
    } = true;

    /// <summary>
    /// Include the properties from the AddTo[List/Map]ActionAttribute
    /// </summary>
    public bool IncludeAddAction {
      get; init;
    } = true;

    /// <summary>
    /// Include the properties from the RemoveFrom[List/Map]ActionAttribute
    /// </summary>
    public bool IncludeRemoveAction {
      get; init;
    } = true;

    /// <summary>
    /// Include the properties from the UpdateIn[List/Map]ActionAttribute
    /// </summary>
    public bool IncludeUpdateListItemAction {
      get; init;
    } = true;

    /// <summary>
    /// Include the properties from the UpsertInto[List/Map]ActionAttribute
    /// </summary>
    public bool IncludeUpsertAction {
      get; init;
    } = true;
  }

  /// <summary>
  /// An action that sets a value and returns it from a getter.
  /// </summary>
  public abstract class SetActionAttribute
    : ActionAttribute { }

  /// <summary>
  /// Used to add an action to update a value at a property or field using the set method.
  /// Also adds the value to the object's update method if it has one available.
  /// </summary>
  public class UpdateActionAttribute
    : SetActionAttribute { }

  /// <summary>
  /// Used to add an action to clear the value at the property or reset it to default using the set method.
  /// </summary>
  public class ClearActionAttribute
    : SetActionAttribute { }
}

