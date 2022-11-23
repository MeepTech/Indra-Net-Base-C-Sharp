using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;
using Indra.Net.Config;

namespace Indra.Net {

  /// <summary>
  /// A createable thing.
  /// </summary>
  public interface ICreateable<TBase> : IFocus<TBase>, ICreateable {

    /// <summary>
    /// Action used to initialize the current object.
    /// </summary>
    [Action("m", "new", Key = "make")]
    public Action.Result<TBase> Create(
      IActor actor,
      IPlace location,
      [Service] DbContext dbContext,
      [Flags] IReadOnlyList<string>? allOrderedFlags = null,
      [Arg] YamlMappingNode? allArgs = null
    );

    /// <summary>
    /// An action used to destroy the object.
    /// </summary>
    [Action("d", "delete")]
    public Action.Result Destroy(
      IActor actor,
      IPlace location,
      [Service] DbContext dbContext,
      [Flags] IReadOnlyList<string>? allOrderedFlags = null,
      [Arg] YamlMappingNode? allArgs = null
    );

    /// <summary>
    /// An action used to clone the object.
    /// </summary>
    [Action("c", "copy")]
    public Action.Result<TBase> Clone(
      IActor actor,
      IPlace location,
      [Service] DbContext dbContext,
      [Flags] IReadOnlyList<string>? allOrderedFlags = null,
      [Arg]YamlMappingNode? allArgs = null
    );
  }

  /// <summary>
  /// A createable thing.
  /// </summary>
  public interface ICreateable : IFocus {

    /// <summary>
    /// The original creator of this thing
    /// </summary>
    public IActor Creator { 
      get;
      internal set;
    }

    /// <summary>
    /// The current owner of this thing
    /// </summary>
    public IActor Owner { 
      get;
      internal set;
    }

    /// <summary>
    /// Build the default key of the createable thing.
    /// </summary>
    public string BuildDefaultKey();
  }
}
