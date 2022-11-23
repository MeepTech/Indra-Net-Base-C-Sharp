namespace Indra.Net {

  /// <summary>
  /// A storeable entity
  /// Includes tools, furniture, pets, etc.
  /// </summary>
  public class Item : Denizen, IStorable, IFocus<Item> {

    internal override string _focusType {
      get => __focusType;
      set => __focusType = value; 
    } string __focusType
      = nameof(Item).ToLower();

    string IUnique.Index {
      get;
      private set;
    }
  }
}
