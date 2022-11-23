namespace Indra.Net {

  /// <summary>
  /// A unique item with an immutable unique id and a mutable but uniquely enforced key.
  /// </summary>
  public interface IUnique {

    /// <summary>
    /// The non-human readable, never changing unique id of the item
    /// </summary>
    public string Id {
      get;
      internal set;
    }

    /// <summary>
    /// The sometimes human readable, and potentialy changeable unique key.
    /// // TODO: enforce uniqueness
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// The current unique index of the item.
    /// </summary>
    public string Index { get; }
  }
}