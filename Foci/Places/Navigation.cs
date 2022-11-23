using Microsoft.EntityFrameworkCore;

namespace Indra.Net {

  /// <summary>
  /// A link between two rooms/areas
  /// </summary>
  [Index(nameof(Key), nameof(FocusType), nameof(Location), IsUnique = true)]
  public class Navigation: Focus<Navigation>, IEntityCreated {

    internal override string _focusType {
      get => __focusType;
      set => __focusType = value;
    } string __focusType
      = "link";

    ///<summary><inheritdoc/></summary>
    public Entity Creator {
      get;
      private set;
    } Entity IEntityCreated.Creator {
      get => Creator;
      set => Creator = value;
    }

    ///<summary><inheritdoc/></summary>
    public Entity Owner {
      get;
      private set;
    } Entity IEntityCreated.Owner {
      get => Owner;
      set => Owner = value;
    }


    ///<summary><inheritdoc/></summary>
    internal protected Navigation(string key, Entity creator)
      : base(key) 
    {
      Creator 
        = Owner 
        = creator;
    }
  }
}
