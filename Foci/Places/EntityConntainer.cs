using System.Collections.Generic;

namespace Indra.Net {

  /// <summary>
  /// A place that can contain entities.
  /// </summary>
  public abstract class EntityConntainer : Place {
    Dictionary<string, Entity> _entities;

    public IReadOnlyDictionary<string, Entity> Entities 
      => _entities;

    protected internal EntityConntainer(string key)
    : base(key) {
      _entities = new Dictionary<string, Entity>();
    }
  }
}
