namespace Indra.Net {
  public class Npc : Denizen, IFocus<Npc> {
    internal override string _focusType {
      get => __focusType;
      set => __focusType = value; 
    } string __focusType
      = nameof(Npc).ToLower();
  }
}
