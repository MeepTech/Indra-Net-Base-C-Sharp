using System.Collections.Generic;

namespace Indra.Net {
  public abstract class ZoneContainer : Place {

    public IReadOnlyDictionary<string, Zone> Zones {
      get;
    }

    internal ZoneContainer(string key) 
      : base(key) {}
  }
}
