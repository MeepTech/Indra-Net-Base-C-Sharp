using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Indra.Net {

  /// <summary>
  /// A place that can contain either smaller places or entities.
  /// </summary>
  [Index(nameof(Key), nameof(FocusType), nameof(Location), IsUnique = true)]
  public abstract class Place : Focus<Place>, IPlace {

    ///<summary><inheritdoc/></summary>
    public virtual string Source {
      get => _prototypeType ??= GetType().Name.ToLower();
      protected set => _prototypeType = value;
    } string _prototypeType = null!;

    ///<summary><inheritdoc/></summary>
    public Address Address
      => new(this);

    ///<summary><inheritdoc/></summary>
    public IReadOnlyDictionary<string, Focus> Foci 
      { get;}

    ///<summary><inheritdoc/></summary>
    internal protected Place(string key)
      : base(key) { }
  }
}
