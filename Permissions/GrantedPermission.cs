using System.Collections.Generic;

namespace Indra.Net {
  public class GrantedPermission {
    internal Entity? _entityActor;
    internal User? _userActor;
    internal Server? _serverActor;

    public Permission Type { get; private set; }
    public IReadOnlyCollection<string>? Modes { get; private set; }

    public IActor Actor
      => _entityActor
        ?? _userActor as IActor
        ?? _serverActor!;
  }
}
