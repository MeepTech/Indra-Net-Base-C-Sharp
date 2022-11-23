using System.Collections.Generic;

namespace Indra.Net {
  public class Zone : ZoneContainer, IFocus<Zone> {
    IReadOnlyDictionary<string, Room> _rooms;

    public World World
      => Parent is World world
       ? world
       : (Parent as Zone)!.World;

    public ZoneContainer Parent {
      get;
    }

    public IReadOnlyDictionary<string, Room> Rooms 
      => _rooms;

    protected internal Zone(ZoneContainer container, string key)
    : base(key) {
      Parent = container;
      _rooms = new Dictionary<string, Room>();
    }
  }
}
