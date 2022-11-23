namespace Indra.Net {

  /// <summary>
  /// A focus/object created by a charachter.
  /// </summary>
  public interface IEntityCreated : ICreateable {

    /// <summary>
    /// The original creator of this thing
    /// </summary>
    public new Entity Creator 
      { get; internal set; }
    IActor ICreateable.Creator {
      get => Creator;
      set => Creator = (Entity)value;
    }

    /// <summary>
    /// The current owner of this thing
    /// </summary>
    public new Entity Owner 
      { get; internal set; }
    IActor ICreateable.Owner {
      get => Owner;
      set => Owner = (Entity)value;
    }
  }
}
