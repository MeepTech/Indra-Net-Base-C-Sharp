using System;
using System.Linq;

namespace Indra.Net.Config {

  /// <summary>
  /// Makes a parameter for a flag, or all flags.
  /// If a default value is provided, the flag's presence will equal the oposite of the default value.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
  public class FlagsAttribute : ArgumentAttribute {

    /// <summary>
    /// If all flags are grabbed, should they be ordered?
    /// Defaults to true. If false, a hash-set is provided instead.
    /// </summary>
    public bool Ordered {
      get;
      init;
    } = true;

    ///<summary><inheritdoc/></summary>
    public FlagsAttribute(params string[] keys)
      : base(keys.Any() ? keys[0] : null, keys[1..]) { }

    ///<summary><inheritdoc/></summary>
    public FlagsAttribute(int index)
      : base(index) { }

    ///<summary><inheritdoc/></summary>
    protected override Argument ArgumentConstructor(object? defaultValue = null)
      => Key is null && Index == -1
        ? new AllFlagsArgument(Ordered)
        : new FlagsArgument(
          Key,
          Aliases,
          IsRequired,
          defaultValue ?? false,
          Index >= 0
            ? Index
            : null);
  }
}
