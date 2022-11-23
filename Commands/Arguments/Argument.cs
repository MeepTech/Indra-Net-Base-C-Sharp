using System.Collections.Generic;
using System.Linq;

namespace Indra.Net {

  /// <summary>
  /// A command argument/parameter.
  /// </summary>
  public abstract class Argument {

    /// <summary>
    /// A not provided value
    /// </summary>
    public record struct NotProvided();

    /// <summary>
    /// Shortcut for the get-value function 'not provided' return.
    /// </summary>
    public static readonly object UseDefault 
      = new();

    /// <summary>
    /// The argument's primary key.
    /// If this is null, there should be an index.
    /// </summary>
    public string? Key {
      get;
      internal set;
    }

    /// <summary>
    /// Other aliases of this argument
    /// </summary>
    public IEnumerable<string> Aliases {
      get;
      internal set;
    }

    /// <summary>
    /// List of all keys (key + aliases, main Key is first)
    /// </summary>
    public IReadOnlyList<string> Keys
      => Key is not null
       ? Aliases.Prepend(Key).ToList()
       : Aliases.ToList();

    /// <summary>
    /// The default value
    /// </summary>
    public object? Default 
      { get; internal set; }
        = null;

    /// <summary>
    /// If this argument is required
    /// </summary>
    public bool IsRequired {
      get;
      internal set;
    }

    /// <summary>
    /// Make a new type of argument
    /// </summary>
    protected Argument(string key, IEnumerable<string> aliases, bool isRequired = false, object? @default = null) {
      Key = key;
      Aliases = aliases;
      Default = @default;
      IsRequired = isRequired;
    }

    /// <summary>
    /// The logic to get the value for this argument
    /// </summary>
    public abstract object? GetValue(Command commandData);
  }
}