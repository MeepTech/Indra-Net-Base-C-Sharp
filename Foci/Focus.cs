using Indra.Net.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace Indra.Net {

  /// <summary>
  /// Something that can be focused on/contains actions.
  /// </summary>
  [Index(IsUnique = true)]
  public abstract class Focus : IFocus, ICreateable {

    /// <summary>
    /// All types mapped to their focus type.
    /// </summary>
    public static IReadOnlyDictionary<string, System.Type> Types
      => _types; internal static Dictionary<string, System.Type> _types
        = new();

    ///<summary><inheritdoc/></summary>
    [Key]
    public string Id {
      get => _id ??= Guid.NewGuid().ToString();
      internal set => _id = value;
    } // implicit property: 
    [NotMapped]
    string IUnique.Id {
      get => _id;
      set => _id = value;
    } // backing field:
    string _id = null!;

    ///<summary><inheritdoc/></summary>
    public string Key {
      [GetAction]
      get => _key;
      [UpdateAction]
      private set {
        _key = value;
        _clearIndex();
      }
    } // backing field:
    string _key = null!;

    ///<summary><inheritdoc/></summary>
    public abstract string FocusType 
      { get; internal set; }

    /// <summary>
    /// The containing location of this thing.
    /// Null if it has not been placed. Or is a prototype.
    /// </summary>
    public Place? Location {
      get => _location;
      internal set {
        _location = value;
        _clearIndex();
      }
    } // backing field:
    Place? _location = null;

    ///<summary><inheritdoc/></summary>
    [NotMapped]
    public IReadOnlyDictionary<string, Permission> RequiredPermissions {
      [GetChildItemsAction]
      get => _requiredPermissionsByKey;
    } // db column:
    [Column(name: "required_permissions")]
    ICollection<Permission> _requiredPermissions {
      get => _requiredPermissionsByKey.Values;
      [ModifyListAction]
      set {
        _requiredPermissionsByKey
          = value.ToDictionary(e => e.Key);
      }
    } // backing field: 
    Dictionary<string, Permission> _requiredPermissionsByKey
      = new();

    ///<summary><inheritdoc/></summary>
    [NotMapped]
    public IReadOnlyDictionary<string, Action> Actions {
      [GetChildItemsAction]
      get => _actionsByKey;
    } // db column:
    [Column("actions")]
    ICollection<Action> _actions {
      get => _actionsByKey.Values;
      [ModifyListAction]
      set {
        _actionsByKey
          = value.ToDictionary(e => e.Key);
      }
    } // backing field:
    Dictionary<string, Action> _actionsByKey
      = new();

    ///<summary><inheritdoc/></summary>
    [NotMapped]
    public IActor Creator {
      get;
      internal set;
    } = null!; // implicit property:
    IActor ICreateable.Creator {
      get => Creator;
      set => Creator = value;
    }

    ///<summary><inheritdoc/></summary>
    [NotMapped]
    public IActor Owner {
      get;
      internal set;
    } = null!; // implicit property:
    IActor ICreateable.Owner {
      get => Owner;
      set => Owner = value;
    }

    /// <summary>
    /// A unique index for this focus that can be passed to the server, like an address for where it is.
    /// </summary>
    public string Index {
      get => _index ??= _getCurrentIndex();
      private set => _index = value;
    } // backing field:
    string? _index = null;

    ///<summary><inheritdoc/></summary>
    internal Focus(
      IActor creator,
      IActor owner,
      Place location,
      string? key,
      IReadOnlyList<Permission>? requiredPermissions
    ) {
      Creator = creator;
      Owner = owner;
      Location = location;
      _requiredPermissions = requiredPermissions?.ToList() ?? new List<Permission>();
      Key = key ?? BuildDefaultKey();
    }
    internal Focus() { }

    ///<summary><inheritdoc/></summary>
    public virtual string BuildDefaultKey() {
      if (Location is null) {
        return FocusType;
      }

      // make sure it's unique within it's location
      int index = 1;
      while (this.Location.Foci.TryGetValue(getCurrentFocus(index), out _)) {
        index++;
      }

      return index > 1 ? FocusType + index : FocusType;

      // Helper to get the current focus
      string getCurrentFocus(int index)
        => index > 1
          ? FocusType + index
          : FocusType;
    }
    internal virtual string _getCurrentIndex() {
      return (Location?.Address.ToString() ?? throw new NotImplementedException())
        + Command.ObjectActionSeperatorCharacter
        + Key;
    }

    internal void _clearIndex() {
      _index = null;
    }
  }

  /// <summary>
  /// Something that can be focused on/contains actions.
  /// </summary>
  public abstract class Focus<TBase> : Focus, IFocus<TBase> {

    ///<summary><inheritdoc/></summary>
    public sealed override string FocusType {
      get => _focusType;
      internal set => _focusType = value;
    }
    [NotMapped]
    internal virtual string _focusType { get; set; }
      = typeof(TBase).Name.ToLower();

    static Focus() {
      var dummy = (Focus)FormatterServices.GetUninitializedObject(typeof(TBase))!;
      _types.Add(dummy.FocusType, typeof(TBase));
    }

    ///<summary><inheritdoc/></summary>
    internal protected Focus(
      IActor creator,
      IActor owner,
      Place location,
      string? key,
      IReadOnlyList<Permission>? requiredPermissions
    ) : base(creator, owner, location, key, requiredPermissions) { }

    /// <summary>
    /// Archetype and EFCore Ctor.
    /// </summary>
    protected Focus()
      : base() { }
  }
}