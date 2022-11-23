using Indra.Net.Actions;
using System;
using System.Collections.Generic;

namespace Indra.Net {

  public abstract partial class Action {

    /// <summary>
    /// A result with a specific return type.
    /// <typeparam name="TReturn"></typeparam>
    public record Result<TReturn> : Result {

      /// <summary>
      /// Make a new result
      /// </summary>
      public Result(
        TReturn? result,
        bool success = true,
        string? message = null,
        UndoAction? undo = null,
        IEnumerable<Exception>? errors = null
      ) : base(result, success, message, undo, errors) { }

      /// <summary>
      /// Used to return nothing in an empty result..
      /// </summary>
      public Result()
        : base(ReturnValue: null) { }

      /// <summary>
      /// Helper to return everything but a result, or quickly add an error state.
      /// </summary>
      public Result(
        string Message,
        IEnumerable<Exception>? Errors = null,
        bool Success = true,
        UndoAction? undo = null
      ) : this(default, Success, Message, undo, Errors) { }

      /// <summary>
      /// Used to return a result and an undo easily.
      /// </summary>
      public Result(
        TReturn? ReturnValue,
        UndoAction? undo,
        bool Success = true,
        string? Message = null,
        IEnumerable<Exception>? Errors = null
      ) : this(ReturnValue, Success, Message, undo, Errors) { }
      
      /// <summary>
      /// Used to return a result and an undo easily.
      /// </summary>
      public Result(
        TReturn? ReturnValue,
        System.Action undo,
        bool Success = true,
        string? Message = null,
        IEnumerable<Exception>? Errors = null
      ) : this(ReturnValue, Success, Message, undo, Errors) { }

      /// <summary>
      /// Used to return a result and an undo easily.
      /// </summary>
      public Result(
        TReturn? ReturnValue,
        Func<Result?> undo,
        bool Success = true,
        string? Message = null,
        IEnumerable<Exception>? Errors = null
      ) : this(ReturnValue, Success, Message, undo, Errors) { }

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result<TReturn>(bool success)
        => new(result: default, success: success);

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result<TReturn>(TReturn @return)
        => new(result: @return);

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result<TReturn>(UndoAction undo)
        => new(default, default, default, undo, default);

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result<TReturn>(UndoAction.Logic undo)
        => new(default, default, default, undo, default);

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result<TReturn>(Func<Result?> undo)
        => new(default, default, default, undo, default);

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result<TReturn>(System.Action undo)
        => new(default, default, default, undo, default);
    }

    /// <summary>
    /// An action result.
    /// </summary>
    public record Result(
      object? ReturnValue,
      bool Success = true,
      string? Message = null,
      UndoAction? Undo = null,
      IEnumerable<Exception>? Errors = null
    ) {

      /// <summary>
      /// Quick access to void return.
      /// </summary>
      public static Result Void = new Result();

      /// <summary>
      /// Used to return nothing in an empty result..
      /// </summary>
      public Result() 
        : this(default(Result.None)) { }

      /// <summary>
      /// Helper to return everything but a result, or quickly add an error state.
      /// </summary>
      public Result(
        IEnumerable<Exception>? Errors = null,
        bool Success = true, 
        string? Message = null,
        UndoAction? undo = null
      ) : this(default(Result.None), Success, Message, undo, Errors) { }

      /// <summary>
      /// Used to return a result and an undo easily.
      /// </summary>
      public Result(
        object? ReturnValue,
        UndoAction? undo, 
        bool Success = true,
        string? Message = null,
        IEnumerable<Exception>? Errors = null
      ) : this(ReturnValue, Success, Message, undo, Errors) { }

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result(bool @return)
        => new(@return);

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result(int @return)
        => new(ReturnValue: @return);

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result(string @return)
        => new(ReturnValue: @return);

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result(UndoAction undo)
        => new(default(Result.None), default, default, undo, default);

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result(UndoAction.Logic undo)
        => new(default(Result.None), default, default, undo, default);

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result(Func<Result?> undo)
        => new(default(Result.None), default, default, undo, default);

      /// <summary>
      /// Make a result just from an undo.
      /// </summary>
      public static implicit operator Result(System.Action undo)
        => new(default(Result.None), default, default, undo, default);

      /// <summary>
      /// Indicates an empty result.
      /// </summary>
      public record struct None() {
        ///<summary><inheritdoc/></summary>
        public override string ToString() {
          return "~void";
        }
      }
    }
  }
}