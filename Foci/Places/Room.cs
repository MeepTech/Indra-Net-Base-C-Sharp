namespace Indra.Net {
  public class Room : EntityConntainer, IFocus<Room> {
    public Zone Zone {
      get;
    }

    protected internal Room(Zone zone, string key) : base(key) {
      Zone = zone;
    }
  }
}
