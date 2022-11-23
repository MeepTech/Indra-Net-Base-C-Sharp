using Indra.Net.Config;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using YamlDotNet.RepresentationModel;

namespace Indra.Net {

  /// <summary>
  /// The base class for any object that can be physically in a room, living or non living, including charachters.
  /// </summary>
  public abstract class Entity : Focus<Entity>, IActor<Entity>, IPrototypeable, ICreateable<Entity> {

    ///<summary><inheritdoc/></summary>
    public Focus? Base {
      get;
      internal set;
    } Focus? IPrototypeable.Base {
      get => Base;
      set => Base = value;
    }

    /// <summary>
    /// The location of this entity.
    /// Null if it has not been placed. Or is a prototype.
    /// </summary>
    public new EntityConntainer? Location
      => (EntityConntainer?)base.Location;

    /// <summary>
    /// If this entity is placed at a location
    /// </summary>
    public bool IsPlaced
      => Location is not null;

    ///<summary><inheritdoc/></summary>
    public IReadOnlyList<string> SubTypes {
      get => _subTypes;
      private set => _subTypes = value.ToList();
    } List<string> _subTypes;

    ///<summary><inheritdoc/></summary>
    protected internal Entity(
      IActor creator,
      IActor owner, 
      Place location, 
      string? key, 
      IReadOnlyList<Permission>? requiredPermissions,
      IEnumerable<string> subTypes
    ) : base(creator, owner, location, key, requiredPermissions) {
      _subTypes = subTypes.Prepend(FocusType).ToList();
    }

    /// <summary>
    /// An action that can be used to create a new type of this object.
    /// THIS is the "Archetype" version of the object. not the object being created.
    /// </summary>
    [Action("m", "new", Key = "make")]
    public abstract Action.Result<Entity> Create(
      IActor actor,
      IPlace location,
      [Service] DbContext dbContext,
      [Flags("into-inventory", "inventory", "i", "intoinventory")] bool intoInventory = false,
      [Arg("for", "give-to", "to", "f", "owner", "own", "o")] string? targetOwner = null,
      [Arg("target-location", "target", "location", "t", "l", "at", "targetlocation")] string? targetLocation = null,
      [Arg("subtype", "sub", "subs", "st", "sub-type", "sub-types", "subtypes")] string? subType = null,
      [Arg("key", "id", "k", "i")] string? key = null,
      [Arg("display-name", "n", "name", "display", "displayname")] string? displayName = null,
      [Flags] IReadOnlyList<string> allOrderedFlags = default!,
      [Arg] YamlMappingNode allArgs = default!
    );

    Action.Result<Entity> ICreateable<Entity>.Create(IActor actor, IPlace location, DbContext dbContext, IReadOnlyList<string> allOrderedFlags, YamlMappingNode allArgs)
       => Create(
        actor,
        location,
        dbContext,
        intoInventory: allOrderedFlags.Any()
          && new[] {
            "inventory",
            "i",
            "intoinventory",
            "into-inventory"
          }.Any(f
            => allOrderedFlags.Contains(f)
        ),
        targetOwner: allArgs.Children.TryGetValue(new[] { "for", "give-to", "to", "f", "owner", "own", "o" }
          .FirstOrDefault(k => allArgs.Children.ContainsKey(k)) ?? "", out var foundO)
            ? foundO.ToString()
            : null,
        targetLocation: allArgs.Children.TryGetValue(new[] { "target", "location", "at", "t", "l", "target-location", "targetlocation" }
          .FirstOrDefault(k => allArgs.Children.ContainsKey(k)) ?? "", out var found)
            ? found.ToString()
            : null,
        subType: allArgs.Children.TryGetValue(new[] { "subtype", "sub", "subs", "st", "sub-type", "sub-types", "subtypes" }
          .FirstOrDefault(k => allArgs.Children.ContainsKey(k)) ?? "", out var foundSt)
            ? foundSt.ToString()
            : null,
        key: allArgs.Children.TryGetValue(new[] { "key", "k", "i", "id" }
          .FirstOrDefault(k => allArgs.Children.ContainsKey(k)) ?? "", out var foundK)
            ? foundK.ToString()
            : null,
        displayName: allArgs.Children.TryGetValue(new[] { "display-name", "n", "name", "display", "displayname" }
          .FirstOrDefault(k => allArgs.Children.ContainsKey(k)) ?? "", out var foundDn)
            ? foundDn.ToString()
            : null,
        allOrderedFlags,
        allArgs
      );

    /// <summary>
    /// An action used to clone the object.
    /// </summary>
    [Action]
    public Action.Result<Entity> Clone(
      IActor actor,
      Place location,
      [Service] DbContext dbContext,
      [Flags("into-inventory", "inventory", "i", "intoinventory")] bool intoInventory = false,
      [Flags("here", "h")] bool toCurrentLocation = false,
      [Arg("target", "location", "to", "t", "target-location", "targetlocation")] string? targetLocation = null,
      [Arg("for", "give-to", "to", "f", "owner", "own", "o")] string? targetOwner = null,
      [Flags] IReadOnlyList<string> allOrderedFlags = default!,
      [Arg] YamlMappingNode allArgs = default!
    ) {
      Entity clone = (Entity)JsonSerializer.Deserialize(JsonSerializer.Serialize(this), GetType())!;

      clone.Id = System.Guid.NewGuid().ToString();
      clone.Creator = actor;
      clone.Base = this;

      if (clone is Focus focus) {
        focus.Location = intoInventory
          ? null
          : toCurrentLocation
            ? location
            : targetLocation != null
              ? Address.ParseKey(targetLocation, dbContext).Location
              : focus.Location;
      }

      if (replaceOwnership) {
        clone.Owner = creator;
      }

      dbContext.Add(clone);
      dbContext.SaveChanges();

      return new(
        clone,
        () => {
          var result = clone.Destroy(dbContext);
          dbContext.SaveChanges();

          return result;
        }
      );
    }

    Action.Result<Entity> ICreateable<Entity>.Clone(
      IActor actor,
      IPlace location,
      DbContext dbContext,
      IReadOnlyList<string> allOrderedFlags,
      YamlMappingNode allArgs
    ) => Clone(
      actor,
      location,
      dbContext,
      intoInventory: allOrderedFlags.Any()
        && new[] {
          "inventory",
          "i",
          "intoinventory", 
          "into-inventory"
        }.Any(f
          => allOrderedFlags.Contains(f)
      ),
      allOrderedFlags.Any()
        && new[] {
          "here",
          "h"
        }.Any(f
          => allOrderedFlags.Contains(f)
      ),
      allArgs.Children.TryGetValue(new[] { "target", "location", "at", "t", "l", "target-location", "targetlocation" }
        .FirstOrDefault(k => allArgs.Children.ContainsKey(k)) ?? "", out var found)
          ? found.ToString()
          : null,
      allOrderedFlags,
      allArgs
    );

    /// <summary>
    /// Used to destroy the entity/remove it from the world.
    /// </summary>
    public Action.Result Destroy(
      [Service] DbContext dbContext,
      IActor? actor = null,
      IPlace? location = null,
      [Flags] IReadOnlyList<string>? allOrderedFlags = null,
      YamlMappingNode? allArgs = null
    ) {
      throw new System.NotImplementedException();
    }

    Action.Result ICreateable<Entity>.Destroy(IActor actor, IPlace location, DbContext dbContext, IReadOnlyList<string> allOrderedFlags, YamlMappingNode allArgs) 
      => Destroy(dbContext, actor, location, allOrderedFlags, allArgs);
  }
}
