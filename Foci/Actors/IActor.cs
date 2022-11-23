namespace Indra.Net {
  public interface IActor : IUnique {
    string ActorType { get; }

  }
  public interface IActor<T> : IActor {
    string IActor.ActorType
      => typeof(T).Name.ToLower();
  }
}
