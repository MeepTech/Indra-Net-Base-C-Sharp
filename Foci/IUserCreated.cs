namespace Indra.Net {
  /// <summary>
  /// A focus/object created by a user.
  /// </summary>
  public interface IUserCreated : ICreateable {

    /// <summary>
    /// The original creator of this thing
    /// </summary>
    public new User Creator { get; internal set; }
    IActor ICreateable.Creator {
      get => Creator;
      set => Creator = (User)value;
    }

    /// <summary>
    /// The current owner of this thing
    /// </summary>
    public new User Owner { get; internal set; }
    IActor ICreateable.Owner {
      get => Owner;
      set => Owner = (User)value;
    }
  }
}
