using Indra.Net.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Indra.Net.Config {

  /// <summary>
  /// Base class for attributes added to arguments on functions with the ActionAttribute applied.
  /// Different sub-classes help get the data in different ways.
  /// </summary>
  public abstract class ArgumentAttribute : Attribute {

    /// <summary>
    /// The associated reflection parameter (if there is one)
    /// </summary>
    protected ParameterInfo? Parameter {
      get;
      private set;
    }

    /// <summary>
    /// The key/name of this argument.
    /// (must be unique on the Action)
    /// If it's null it usually means you're requesting all the items of the given type or a special item.
    /// </summary>
    public string? Key { 
      get; 
      init;
    } = null!; // backing field:

    /// <summary>
    /// The index of the flag you want to use instead of a key
    /// </summary>
    public int Index 
      { get; init; } = -1;

    /// <summary>
    /// If this argument is required.
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    /// If this is put on an IList it will try to use an empty list of the given type as the default value instead of null unless this is true. (defaults to false)
    /// </summary>
    public bool UseNullAsDefaultForList { get; init; }
      = false;

    /// <summary>
    /// The secondary aliases/keys/names of the action
    /// (each must be unique on the Action)
    /// </summary>
    public string[] Aliases {
      get => _aliases;
      init => _aliases = value;
    } // backing field:
    string[] _aliases 
      = Array.Empty<string>();

    /// <summary>
    /// All potential names and aliases for this action
    /// (each must be unique on the Action)
    /// </summary>
    public IReadOnlyList<string> AllNames
      => Key == null
        ? Aliases
        : Aliases.Append(Key).ToList()!;

    /// <summary>
    /// Key based argument
    /// </summary>
    protected ArgumentAttribute(string? keyOverride = null, string[]? aliases = null) {
      Key = keyOverride;
      Aliases = aliases ?? Array.Empty<string>();
    }

    ///<summary>
    /// Order based argument
    ///</summary>
    public ArgumentAttribute(int index) { Index = index; }

    /// <summary>
    /// Used to set up this attribute.
    /// Can be used to hook up services.
    /// </summary>
    internal void _init(IReadOnlyDictionary<string, object> availableServices, ParameterInfo parameter) {
      Parameter = parameter;
      Setup(availableServices);
    }

    /// <summary>
    /// Used to set up this attribute.
    /// Can be used to hook up services.
    /// </summary>
    protected internal virtual void Setup(IReadOnlyDictionary<string, object> availableServices) { }

    /// <summary>
    /// Build and return the argument object
    /// </summary>
    internal Argument _buildArgument() {
      object? @default = null;
      if (!IsRequired && Parameter is not null) {
        @default = Parameter
          .GetCustomAttribute<DefaultValueAttribute>()
            ?.Value
          ?? Parameter.DefaultValue;

        // replace array default with [0] by default.
        if (@default is null && !UseNullAsDefaultForList) {
          if (Parameter.ParameterType.IsAssignableTo(typeof(Array))) {
            @default = Activator.CreateInstance(typeof(List<>)
              .MakeGenericType(Parameter.ParameterType
                  .GetFirstInheritedGenericTypeParameters(typeof(IList<>))
                  .First()));
          } else if (Parameter.ParameterType.IsAssignableTo(typeof(IList))) {
            @default = typeof(Array)
              .GetMethod(nameof(Array.Empty))!
              .MakeGenericMethod(Parameter.ParameterType);
          }
        }
      }

      return ArgumentConstructor(@default);
    }

    /// <summary>
    /// Used to build the correct argument type.
    /// </summary>
    protected abstract Argument ArgumentConstructor(object? defaultValue = null);
  }
}
