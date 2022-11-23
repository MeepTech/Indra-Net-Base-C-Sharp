using System;
using System.Collections.Generic;
using System.Linq;

namespace Indra.Net.Utilities {
  internal static class ReflectionExtensions {

    /// <summary>
    /// Get the generic arguments from a type this inherits from
    /// </summary>
    public static IEnumerable<Type> GetFirstInheritedGenericTypeParameters(this Type type, Type genericParentType) {
      foreach (Type intType in type.GetParentTypes()) {
        if (intType.IsGenericType && intType.GetGenericTypeDefinition() == genericParentType) {
          return intType.GetGenericArguments();
        }
      }

      return Enumerable.Empty<Type>();
    }

    /// <summary>
    /// Get all parent types and interfaces 
    /// </summary>
    public static IEnumerable<Type> GetParentTypes(this Type type) {
      // is there any base type?
      if (type == null) {
        yield break;
      }

      // return all implemented or inherited interfaces
      foreach (var i in type.GetInterfaces()) {
        yield return i;
      }

      // return all inherited types
      var currentBaseType = type.BaseType;
      while (currentBaseType != null) {
        yield return currentBaseType;
        currentBaseType = currentBaseType.BaseType;
      }
    }
  }
}
