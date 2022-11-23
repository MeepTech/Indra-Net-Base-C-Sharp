using System.Collections.Generic;

namespace Indra.Net {

  /// <summary>
  /// A place that can contain other places or entities
  /// </summary>
  public interface IPlace: IPrototypeable {

    /// <summary>
    /// The compiled address of this place.
    /// </summary>
    public Address Address 
      { get; }

    /// <summary>
    /// All foci/focuses in this place, organized by key
    /// </summary>
    public IReadOnlyDictionary<string, Focus> Foci 
      { get; }
  }
}
