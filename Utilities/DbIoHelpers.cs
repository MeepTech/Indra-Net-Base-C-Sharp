using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Indra.Net {
  public static class DbIoHelpers {

    //TODO: make roles and check roles.
    //TODO: check up the scope chain as well.
    public async static Task<bool> HasPermission(this DbContext context, IActor forActor, Permission permission, params string[] modes)
      => await context.CheckPermission(forActor, permission, modes) != null;

    public async static Task<bool> HasPermission(this DbContext context, IActor forActor, string key, IFocus focus, params string[] modes)
      => await context.CheckPermission(forActor, new(key, focus), modes) != null;

    public async static Task<bool> HasPermission(this DbContext context, IActor forActor, Permission permission, IEnumerable<string>? modes = null)
      => await context.CheckPermission(forActor, permission, modes) != null;

    public async static Task<bool> HasPermission(this DbContext context, IActor forActor, string key, IFocus focus, IEnumerable<string>? modes = null)
      => await context.CheckPermission(forActor, new(key, focus), modes) != null;

    public async static Task<GrantedPermission?> CheckPermission(this DbContext context, IActor forActor, Permission permission, params string[] modes) => await context.CheckPermission(forActor, permission, (IEnumerable<string>)modes);

    public async static Task<GrantedPermission?> CheckPermission(this DbContext context, IActor forActor, Permission permission, IEnumerable<string>? modes = null) {
      var found = await context.Set<GrantedPermission>()
        .FirstOrDefaultAsync(p =>
          p.Type.Name == permission.Name
            && p.Type.FocusQuery == permission.FocusQuery
            && ((p._entityActor != null && p._entityActor.Id == forActor.Id)
              || (p._serverActor != null && p._serverActor.Id == forActor.Id)
              || (p._userActor != null && p._userActor.Id == forActor.Id)));

      if (found == null || modes == null || !modes.Any()) {
        return found;
      }

      if (modes.Except(found.Modes).Any()) {
        return null;
      }

      return found;
    }
  }
}
