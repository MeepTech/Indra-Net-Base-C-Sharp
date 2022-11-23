using System;
using System.Linq;

namespace Indra.Net.Config {

  /// <summary>
  /// Yaml parsed positional attribute, or all of the yaml as an object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
  public class ArgAttribute : ArgumentAttribute {

    /// <summary>
    /// Make a yaml attribute with just an index
    /// </summary>
    /// <param name="index"></param>
    public ArgAttribute(int index) : base(index) { }

    /// <summary>
    /// Make a yaml attribute from a set of keys or all keys.
    /// </summary>
    /// <param name="keys"></param>
    public ArgAttribute(params string[] keys)
      : base(keys.Any() ? keys[0] : null, keys[1..]) { }

    ///<summary><inheritdoc/></summary>
    protected override Argument ArgumentConstructor(object? defaultValue = null) 
      => Key is null && Index == -1
        ? new AllParametersArgument(defaultValue)
        : new ParameterizedArgument(
          Key,
          Aliases,
          defaultValue,
          IsRequired,
          Index >= 0
            ? Index
            : null);
  }
}
