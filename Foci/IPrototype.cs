using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;
using Indra.Net.Config;

namespace Indra.Net {

  /// <summary>
  /// If this type is prototypeable.
  /// Prototypes can have their permissions globalized to areas for all items with the same prototype.
  /// </summary>
  public interface IPrototypeable : ICreateable {

    /// <summary>
    /// The prototype this was made from or thing this was copied from.
    /// </summary>
    public Focus? Base 
      { get; internal set; }

    /// <summary>
    /// The chain of sub types of this prototyped object.
    /// TODO: Each of these must be unique!
    /// </summary>
    public IReadOnlyList<string> SubTypes { get; }

    /// <summary>
    /// An action used to clone the object.
    /// </summary>
    [Action("p", "proto", "pt", "ptype", "proto-type")]
    public Action.Result<Prototype> Prototype(
      IActor actor,
      IPlace location,
      [Service] DbContext dbContext,
      [Flags] IReadOnlyList<string> allOrderedFlags = default!,
      [Arg] YamlMappingNode allArgs = default!
    );
  }
}
