namespace Indra.Net {

  /// <summary>
  /// A user of a server.
  /// </summary>
  public class User : Focus<User>, IActor<User> {

    /// <summary>
    /// The current user
    /// </summary>
    public static User Current {
      get;
      internal set;
    } = null!;

    public string PasswordHash { get; set; }

    internal protected User(string key) 
      : base(key) {}
  }
}
