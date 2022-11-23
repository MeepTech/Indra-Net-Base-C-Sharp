using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indra.Net {
  public struct Address {
    string _key = null!;

    public World World {
      get;
    }

    public IReadOnlyList<Zone> Zones {
      get;
    }

    public Room? Room {
      get;
    } = null;

    public Area? Area {
      get;
    } = null;

    public Place Location
      => Area ?? Room ?? Zones.LastOrDefault() ?? (Place)World;

    public string Key =>
      _key ??= BuildKey(World, Zones, Room, Area);

    internal Address(IPlace place) {
      switch (place) {
        case Area a:
          Area = a;
          Room = a.Room;
          (Zones, World) = _GetZonesAndWorld(a);
          break;
        case Room r:
          Room = r;
          (Zones, World) = _GetZonesAndWorld(r);
          break;
        case Zone z:
          (Zones, World) = _GetZonesAndWorld(z);
          break;
        case World w:
          Zones = new List<Zone>();
          World = w;
          break;
        default:
          throw new ArgumentException(nameof(place));
      }
    }

    static (IReadOnlyList<Zone>, World World) _GetZonesAndWorld(IPlace place) {
      Zone root;
      var zones = new List<Zone>();
      if (place is Area a) {
        root = a.Room.Zone;
      }
      else if (place is Room r) {
        root = r.Zone;
      }
      else if (place is Zone z) {
        root = z;
      }
      else throw new ArgumentException(nameof(place));

      zones.Add(root);
      ZoneContainer currentParent = root.Parent;
      while (currentParent is Zone currentZone) {
        zones.Add(currentZone);
        currentParent = currentZone.Parent;
      }

      return (zones, currentParent as World)!;
    }

    public static string BuildKey(Address address)
      => BuildKey(address.World, address.Zones, address.Room, address.Area);

    static string BuildKey(World world, IReadOnlyList<Zone> zones, Room? room, Area? area) {
      StringBuilder builder = new("/");
      builder.Append(world.Key);
      if (zones.Any()) {
        foreach (Zone zone in zones) {
          builder.Append("/");
          builder.Append(zone.Key);
        }
        if (room != null) {

          builder.Append("/");
          builder.Append(room.Key);
          if (area != null) {
            builder.Append("#");
            builder.Append(area.Key);
          }
        }
      }

      return builder.ToString();
    }

    public static Address ParseKey(string addressKey, DbContext dbContext) {
      var parts = new Stack<string>(addressKey.Split("/"));
      // discard the server address. Using this function assumes it's for the current server.
      string serverAddress = parts.Pop();
      string worldKey = parts.Pop();
      string rootZoneKey = parts.Pop();

      World world = dbContext.Set<World>().Find(worldKey);
      List<Zone> zones = new();

      Zone rootZone = dbContext.Set<Zone>().Find(rootZoneKey);
      IPlace currentPlace = rootZone;
      string currentKey = rootZone.Key;
      while (currentPlace is Zone currentZone && parts.Any()) {
        zones.Add(currentZone);
        currentKey = parts.Pop();
        if (currentKey == "") {
          break;
        }

        if (currentZone.Zones.TryGetValue(currentKey, out var nextZone)) {
          currentPlace = nextZone;
          continue;
        }
        else {
          if (currentZone.Rooms.TryGetValue(currentKey, out var nextRoom)) {
            currentPlace = nextRoom;
            continue;
          }
        }
        throw new Exception($"Key {currentKey} not found as a Room or Zone in Zone: {currentZone.Key}");
      }

      if (currentPlace is Room room) {
        return new Address(room);
      }
      else if (currentKey.Contains("#")) {
        var roomKeyParts = currentKey.Split("#");
        var containingRoom = dbContext.Set<Room>().Find(roomKeyParts[0]);
        var area = new Area(containingRoom, roomKeyParts[1]);

        return new Address(area);
      }
      else {
        return new(zones.Last());
      }
    }
  }
}
