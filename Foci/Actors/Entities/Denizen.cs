using System.Collections.Generic;
using System.Linq;

namespace Indra.Net {

  /// <summary>
  /// Entities that are not player-charachters.
  /// NPCs, Items, Objects, Furniture, etc.
  /// </summary>
  public abstract class Denizen : Entity, IEntityCreated, IFocus<Denizen> {
    internal override string _focusType {
      get => __focusType;
      set => __focusType = value; 
    } string __focusType
      = nameof(Denizen).ToLower();

    ///<summary><inheritdoc/></summary>
    public new Entity Creator {
      get => (Entity)base.Creator;
      private set => base.Creator = value;
    } Entity IEntityCreated.Creator {
      get => Creator;
      set => Creator = value;
    }

    ///<summary><inheritdoc/></summary>
    public new Entity Owner {
      get => (Entity)base.Owner;
      set => base.Owner = value;
    } Entity IEntityCreated.Owner {
      get => Owner;
      set => Owner = value;
    }

    ///<summary><inheritdoc/></summary>
    internal protected Denizen(string key, Entity creator, IEnumerable<string> subTypes)
      : base(key, creator, subTypes.Prepend(nameof(Denizen).ToLower())) {}
  }
}
