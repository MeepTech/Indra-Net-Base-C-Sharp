using System;
using System.Collections.Generic;
using System.Reflection;

namespace Indra.Net.Config {

  /// <summary>
  /// Request a service to be injected by the server.
  /// Some will require permissions!
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
  public class ServiceAttribute : ArgumentAttribute {
    object _service = null!;

    ///<summary><inheritdoc/></summary>
    public ServiceAttribute(string? key = null)
      : base(key) { }

    ///<summary><inheritdoc/></summary>
    public override object? GetValue(ref Command commandData) {
      return _service;
    }

    ///<summary><inheritdoc/></summary>
    protected override void Setup(ParameterInfo parameter, Dictionary<string, object> availableServices) {
      base.Setup(parameter, availableServices);

      string key = Key ?? parameter.GetType().Name.ToLower();
      _service = availableServices[key];
    }
  }
}
