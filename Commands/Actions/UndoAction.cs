using System;
using System.Linq;

namespace Indra.Net.Actions {

  /// <summary>
  /// An undo action for an executed action.
  /// </summary>
  public class UndoAction : Action {

    /// <summary>
    /// Undo the given action with this logic.
    /// </summary>
    public delegate Result? Logic();

    Logic _logic {
      get;
    }

    /// <summary>
    /// Make an undo action from some logic.
    /// </summary>
    public UndoAction(Logic logic) : base(Enumerable.Empty<Argument>()) {
      _logic = logic;
    }

    ///<summary><inheritdoc/></summary>
    public override Result? Execute(Command command) {
      return _logic();
    }
    /// <summary>
    /// Make an undo action from some logic.
    /// </summary>
    public static implicit operator UndoAction(System.Action logic) {
      return new UndoAction(() => { 
        logic(); 
        return Result.Void; 
      });
    }

    /// <summary>
    /// Make an undo action from some logic.
    /// </summary>
    public static implicit operator UndoAction(Func<Result?> logic) {
      return new UndoAction(() => logic());
    }


    /// <summary>
    /// Make an undo action from some logic.
    /// </summary>
    public static implicit operator UndoAction(Logic logic) {
      return new UndoAction(logic);
    }
  }
}