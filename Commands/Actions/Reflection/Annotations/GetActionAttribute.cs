namespace Indra.Net.Config {

  /// <summary>
  /// An action that gets and returns a value from a getter.
  /// 
  /// Adds an action to get the list/map items too if it's a list or map 
  ///   (from GetChildItemsActionAttribute)
  /// </summary>
  public class GetActionAttribute
    : ActionAttribute 
  {

    /// <summary>
    /// Whether to include GetChildItemsActionAttribute if this is a list too.
    /// </summary>
    public bool IncludeChildItemsGetAction 
      { get; init; } = true;
  }

  /// <summary>
  /// An action that gets and returns a list/map of values from a getter.
  /// </summary>
  public class GetChildItemsActionAttribute
    : GetActionAttribute { }
}