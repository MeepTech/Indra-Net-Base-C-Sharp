using Indra.Net.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Indra.Net {

  public class Server :  Focus<Server>,IActor<Server>, IHaveDto<ServerInfoDto> {

    /// <summary>
    /// The current server
    /// </summary>
    public static Server Current {
      get;
      private set;
    } = null!;

    [Key]
    public string Id {
      get => _id ??= Guid.NewGuid().ToString();
      private set => _id = value;
    }
    string? _id;

    public string Key { get; private set; }

    public string Address { get; private set; }

    public string DisplayName { get; protected set; }

    public string? Description { get; protected set; }

    public string? IconUrl { get; protected set; }

    [Column("Worlds")] HashSet<World> _worlds { get; set; }
    public IReadOnlyDictionary<string, World> Worlds
      => _worldsByKey ??= _worlds.ToDictionary(w => w.Key);
    Dictionary<string, World>? _worldsByKey;

    public IReadOnlyList<Permission> Permissions {
      get => _requiredPermissions;
      private set => _requiredPermissions = value.ToList();
    }
    List<Permission> _requiredPermissions;

    internal protected Server(string key, string address) {
      Key = key;
      Address = new Uri(address).AbsoluteUri;
      DisplayName = key;
      _worlds = new();
      _requiredPermissions = new List<Permission>();
    }

    public static void Initialize(
      string key,
      string address,
      DbContext dbContext
    ) {
      // get a server object if it already exists
      dbContext.Database.EnsureCreated();
      Current = dbContext
        .Set<Server>()
        .FirstOrDefault(server
          => server.Key == key)!;

      // make a new server if this is the first run/one wasn't found.
      if (Current == null) {
        Current = new Server(key, address);
        dbContext.Add(Current);

        dbContext.SaveChanges();
      } // server key/address mismatch
      else if (Current.Address != new Uri(address).AbsolutePath) {
        throw new ArgumentException($"The address value given: {new Uri(address).AbsolutePath}, does not match the expected: {Current.Address}, for the provided server key: {key}");
      }
    }

    public async virtual Task<ServerInfoDto> ToDto(IActor forActor, [NotNull] DbContext dbContext) {
      if (forActor != null && await dbContext.HasPermission(forActor, "secret-server-info", this, "see")) {
        return new FullServerInfoDto(this);
      }

      return new ServerInfoDto(this);
    }
  }
}
