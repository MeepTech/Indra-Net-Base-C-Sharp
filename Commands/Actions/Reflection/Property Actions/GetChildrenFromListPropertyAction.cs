using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Indra.Net.Actions {
  /// <summary>
  /// Get the value of a property on a focus with an action
  /// </summary>
  public class GetChildrenFromListPropertyAction : MethodBasedAction {

    /// <summary>
    /// The get method.
    /// </summary>
    public MethodInfo GetListMethod
      => Method;

    /// <summary>
    /// Flag to enable ids mode
    /// </summary>
    public static FlagsArgument IdsModeFlag {
      get;
    } = new FlagsArgument("ids", new[] { "id", "i" });

    /// <summary>
    /// Flag to enable keys mode
    /// </summary>
    public static FlagsArgument KeysModeFlag {
      get;
    } = new FlagsArgument("keys", new[] { "key", "k" });

    /// <summary>
    /// The index(s) argument
    /// </summary>
    public static ParameterizedArgument IndexArgument {
      get;
    } = new ParameterizedArgument("index", new[] { "indexes", "keys", "ids", "indicies", "i", "key", "item", "items", "id" }, "*", expectedIndex: 0);

    internal GetChildrenFromListPropertyAction(
      string key,
      IEnumerable<string> aliases,
      MethodInfo getMethod
    ) : base(key, aliases, new Argument[] {
      KeysModeFlag,
      IdsModeFlag,
      IndexArgument
    }, getMethod) { }

    ///<summary><inheritdoc/></summary>
    protected override Result ExecuteFor(Command command) {
      var idMode = IdsModeFlag.GetValue(command) as bool? ?? false;
      var keyMode = KeysModeFlag.GetValue(command) as bool? ?? false;
      var indexes = (IndexArgument.GetValue(command) as string)?.Trim() ?? "*";

      List<object> list
        = ((IEnumerable?)(GetListMethod.Invoke(
          command.Focus,
          System.Array.Empty<object>()
        )))?.Cast<object>().ToList()
          ?? Enumerable.Empty<object>().ToList();

      bool multiItemReturn = true;
      if (indexes != "" && indexes != "*") {
        var newList = new List<object>();
        var indecies = indexes.Split(',')
          .Select(i => i.Trim());

        if (indecies.Count() < 2) {
          multiItemReturn = false;
        }

        foreach (var index in indecies) {
          if (int.TryParse(indexes, out var i)) {
            if (list.Count < i) {
              newList.Add(list[i]);
              list = new[] { list[i] }.ToList();
            } else {
              var found = list
                .Cast<object>()
                .Where(e => e is IUnique)
                .Cast<IUnique>()
                .FirstOrDefault(e => e.Key == index || e.Id == index);

              if (found is not null) {
                newList.Add(found);
              } else if (!multiItemReturn) {
                return new Result(
                  Message: $"Failed to find an item with index/key/id: {index}",
                  Success: false
                );
              }
            }
          } else {
            var found = list
              .Cast<object>()
              .Where(e => e is IUnique)
              .Cast<IUnique>()
              .FirstOrDefault(e => e.Key == index || e.Id == index);

            if (found is not null) {
              newList.Add(found);
            } else if (!multiItemReturn) {
              return new Result(
                Message: $"Failed to find an item with index/key/id: {index}",
                Success: false
              );
            }
          }
        }
      }

      if (keyMode) {
        if (idMode) {
          list = list
            .Where(e => e is IUnique)
            .Cast<IUnique>()
            .Select(i => (object)$"{i.Key}|{i.Id}")
            .ToList();
        } else {
          list = list
            .Where(e => e is IUnique)
            .Cast<IUnique>()
            .Select(i => (object)$"{i.Key}")
            .ToList();
        }
      } else if (idMode) {
        list = list
          .Where(e => e is IUnique)
          .Cast<IUnique>()
          .Select(i => (object)$"{i.Id}")
          .ToList();
      }

      return new Result() {
        ReturnValue = multiItemReturn 
          ? list 
          : list.FirstOrDefault(),
        Success = true
      };
    }
  }
}