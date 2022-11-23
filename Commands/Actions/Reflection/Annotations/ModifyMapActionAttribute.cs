namespace Indra.Net.Config {

  /// <summary>
  /// Abstraction for modifying a a map's contents only 
  /// (not the list objext itself)
  /// </summary>
  public class ModifyMapActionAttribute
    : ModifyActionAttribute {

    /// <summary>
    /// Don't Include the properties from the SetActionAttribute
    /// </summary>
    public override bool IncludeSetAction
      => false;

    /// <summary>
    /// Don't Include the properties from the UpdateActionAttribute
    /// </summary>
    public override bool IncludeUpdateAction
      => false;

    /// <summary>
    /// Don't Include the properties from the ClearActionAttribute
    /// </summary>
    public override bool IncludeClearAction
      => false;
  }

  /// <summary>
  /// Used to add an action to add an item to a map/IDictionary using the 'set'  method of a property or field
  /// </summary>
  public class AddToMapActionAttribute
    : SetActionAttribute { }

  /// <summary>
  /// Used to add an action to update an existing item in a map/IDictionary using the 'set'  method of a property or field
  /// </summary>
  public class UpdateInMapActionAttribute
    : SetActionAttribute { }

  /// <summary>
  /// Used to add an action to update an existing item, or add a new item to a map/IDictionary using the 'set' method of a property or field
  /// </summary>
  public class UpsertIntoMapActionAttribute
    : SetActionAttribute { }

  /// <summary>
  /// Used to remove an item from a map/IDictionary using the 'set' method of a property or field
  /// </summary>
  public class RemoveFromMapActionAttribute
    : SetActionAttribute { }
}

