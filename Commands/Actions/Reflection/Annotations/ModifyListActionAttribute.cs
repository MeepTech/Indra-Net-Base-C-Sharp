namespace Indra.Net.Config {
  /// <summary>
  /// Abstraction for setting to a list's contents only 
  /// (not the list objext itself)
  /// </summary>
  public class ModifyListActionAttribute 
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
  /// Used to add an action to add an item to a list using the 'set'  method of a property or field
  /// </summary>
  public class AddToListActionAttribute
    : ModifyListActionAttribute { }

  /// <summary>
  /// Used to add an action to update an existing item in a list using the 'set'  method of a property or field
  /// </summary>
  public class UpdateInListActionAttribute
    : ModifyListActionAttribute { }

  /// <summary>
  /// Used to add an action to update an existing item, or add a new item to a list using the 'set' method of a property or field
  /// </summary>
  public class UpsertIntoListActionAttribute
    : ModifyListActionAttribute { }

  /// <summary>
  /// Used to remove an item from a list using the 'set' method of a property or field
  /// </summary>
  public class RemoveFromListActionAttribute
    : ModifyListActionAttribute { }
}

