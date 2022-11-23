using System;

namespace Indra.Net.Config {
  /// <summary>
  /// Get the whole argument string as a single string parameter
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
  public class FullCommandAttribute : ArgumentAttribute {

    /// <summary>
    /// Get the whole argument string
    /// </summary>
    public FullCommandAttribute()
      : base(null) { }

    ///<summary><inheritdoc/></summary>
    protected override Argument ArgumentConstructor(object? defaultValue = null)
      => new FullCommandArgument();
  }
}
