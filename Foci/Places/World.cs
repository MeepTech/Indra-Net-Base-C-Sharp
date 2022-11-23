namespace Indra.Net {
  public class World : ZoneContainer, IFocus<World> {
    public Server Server 
      { get; private set; }

    public string DisplayName 
      { get; private set; }

    internal protected World(Server server, string key) : base(key) {
      Server = server;
      DisplayName = key;
    }
  }
}
