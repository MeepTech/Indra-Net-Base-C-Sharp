namespace Indra.Net {

  /// <summary>
  /// Marks an item that can be stored in an inventory
  /// </summary>
  public interface IStorable {

    /// <summary>
    /// The original key of the item before it was placed in the given inventory.
    /// </summary>
    public string? OriginalKey {
      get;
      internal set;
    }

    /// <summary>
    /// The inventory this item is currently stored in
    /// </summary>
    public Inventory? Inventory { get; }

    /// <summary>
    /// If this item is currently stored in an inventory
    /// </summary>
    public bool IsStored
      => Inventory is not null;

    public string Store(Inventory inInventory);

    public string Drop(Place atLocation);
  }
}