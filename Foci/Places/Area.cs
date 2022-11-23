namespace Indra.Net {
  public class Area : EntityConntainer, IFocus<Area> {
    public object Data { get; }

    public Room Room { get; }

    internal Area(Room room, object data) : base(data.ToString() ?? throw new System.ArgumentNullException(nameof(data))) {
      Room = room;
      Data = data;
    }
  }
}
