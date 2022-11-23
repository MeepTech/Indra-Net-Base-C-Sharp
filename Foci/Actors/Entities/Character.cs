
namespace Indra.Net {

  /// <summary>
  /// A user controled entity representing them in the world.
  /// </summary>
  public class Character : Entity, IFocus<Character>, IActor<Character>, IPrototypeable {

    internal override string _focusType {
      get => __focusType;
      set => __focusType = value;
    } string __focusType
      = nameof(Character).ToLower();

    string IActor.ActorType
      => nameof(Character).ToLower();

    protected internal Character(string key) 
      : base(key) {}
  }
}
