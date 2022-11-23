using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Indra.Net {

  /// <summary>
  /// An action that can be executed via a command.
  /// </summary>
  public abstract partial class Action {

    /// <summary>
    /// The primary key for this action.
    /// Must be unique per-focus.
    /// </summary>
    public string Key 
      { get; private set; }

    /// <summary>
    /// The aliases for this action, additional keys.
    /// Each must also be unique for this focus.
    /// </summary>
    public IEnumerable<string> Aliases 
      { get; private set; }

    /// <summary>
    /// The focus this action is bound to
    /// </summary>
    public Focus? Focus {
      get;
      private set;
    }

    /// <summary>
    /// If this focus is bound to an action.
    /// </summary>
    public bool IsBound
      => Focus is not null;

    /// <summary>
    /// The various arguments this action is designed to accept.
    /// </summary>
    public IReadOnlyDictionary<string, Argument> Arguments { get; }

    /// <summary>
    /// Available permissions for this action.
    /// </summary>
    public Permissions AvailablePermissions
      => _permissions ??= _buildAvailablePermissions();
    Permissions? _permissions = null;

    /// <summary>
    /// used to make a new type of action.
    /// </summary>
    protected Action(string key, IEnumerable<string> aliases, IEnumerable<Argument> arguments) {
      Arguments = arguments.ToDictionary(a => a.Key);
      Key = key.ToLower();
      Aliases = aliases;
    }

    /// <summary>
    /// Used to execute the action.
    /// </summary>
    public Result? Execute(Command command) {
      Result? result = new(Success: false, Message: "Uninitialized");
      try {
        result = ExecuteFor(command) ?? new(ReturnValue: null);
      } catch (Exception ex) {
        result = new(Success: false, Message: ex.Message, Errors: new[] { ex });
      } finally {}

      return result;
    }

    /// <summary>
    /// Used to execute the action.
    /// </summary>
    protected abstract Result? ExecuteFor(Command command);

    /// <summary>
    /// Executed on binding.
    /// </summary>
    protected virtual void OnBind() { }

    /// <summary>
    /// Bind a copy of this action to a given focus.
    /// Fails if this action is already bound!
    /// </summary>
    public Action BindTo(Focus target) {
      if (IsBound) {
        throw new InvalidOperationException($"Action is already bound to target focus: {Focus!.Id}");
      } else {
        Action @new = this.Copy();
        @new.Focus = target;
        @new._permissions = null;
        @new.OnBind();

        return @new;
      }
    }

    /// <summary>
    /// Used to make a copy of an action.
    /// </summary>
    /// <returns></returns>
    protected virtual Action Copy() {
      return (Action)MemberwiseClone();
    }

    /// <summary>
    /// Permissions for an action.
    /// </summary>
    public record struct Permissions (
      Permission FullAccess,
      IReadOnlyDictionary<Permission.ModeType, Permission> ByMode,
      IReadOnlyDictionary<Argument, Permission> ByArgument
    ) : IEnumerable<Permission> {

      ///<summary><inheritdoc/></summary>
      public IEnumerator<Permission> GetEnumerator()
        => ByMode.Values.Concat(ByArgument.Values).Prepend(FullAccess).GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    }

    /// <summary>
    /// Get all the available permissions for this function
    /// </summary>
    Permissions _buildAvailablePermissions() {
      string focusQuery = IsBound 
        ? Focus!.Key 
        : Command.CurrentObjectFocusQueryKey;

      Permission fullAccess = new Permission(Key, focusQuery, Permission.ModeType.All);

      Dictionary<Permission.ModeType, Permission> modePermissions
        = Enum.GetValues<Permission.ModeType>()
          .ToDictionary(
            mode => mode,
            mode => new Permission(Key, Command.CurrentObjectFocusQueryKey, mode)
          );

      Dictionary<Argument, Permission> argumentPermissions
        = Arguments.ToDictionary(
            a => a.Value,
            a => new Permission(
              Key,
              Command.CurrentObjectFocusQueryKey,
              Permission.ModeType.Argument,
              a.Key
            )
          );

      return new(fullAccess, modePermissions, argumentPermissions);
    }

    /// <summary>
    /// Decode a command line string that starts with a slash to it's action parts.
    /// </summary>
    public static Command DecodeFromSlashCommand(string commandLineText) {
      commandLineText = commandLineText.TrimStart().TrimStart('/');
      var firstSpace = commandLineText.IndexOf(' ');
      if (firstSpace == -1) {
        firstSpace = commandLineText.Length;
      }

      var lastDotBeforeFirstSpace = commandLineText
        [..firstSpace]
        .IndexOf(".");

      if (lastDotBeforeFirstSpace == -1) {
        throw new ArgumentException($"No Action Specified using a . before the first space in the command line.");
      }

      var focusQuery = commandLineText[..lastDotBeforeFirstSpace].Trim();
      var actionKey = commandLineText[(lastDotBeforeFirstSpace + 1)..];
      List<string> flags = new();
      string remainder = "";

      if (firstSpace == commandLineText.Length) {
        return new(
          focusQuery,
          actionKey.Trim(),
          flags,
          flags.ToHashSet(),
          remainder
        );
      }

      remainder = commandLineText[firstSpace..];
      var trimmedRemainder = remainder.TrimStart();

      while (trimmedRemainder.StartsWith("--")) {
        remainder = trimmedRemainder;
        var nextStart = remainder.IndexOf(" ");
        flags.Add(remainder.Substring(2, nextStart).Trim());
        remainder = remainder[nextStart..];
        trimmedRemainder = remainder.TrimStart();
      }

      return new(
        focusQuery,
        actionKey.Trim(),
        flags,
        flags.ToHashSet(),
        remainder
      );
    }

    /// <summary>
    /// ICommandData for an basic action command
    /// </summary>
    /// <param name="FocusQuery">The query string used to find the focus/foci</param>
    /// <param name="ActionKey">The action to be preformed on the focus/foci</param>
    /// <param name="OrderedFlags">Flags provided to the command call in order</param>
    /// <param name="AllFlags">A hash set of the flags provided to the command call</param>
    /// <param name="ArgumentString">the full unprocessed string of arguments</param>
    public record Command(
      IActor Actor,
      Place Location,
      string FocusQuery,
      string ActionKey,
      IReadOnlyList<string> OrderedFlags,
      ISet<string> AllFlags,
      string ArgumentString
    ) : Indra.Net.Command(
      Actor,
      Location,
      FocusQuery + "." + ActionKey,
      OrderedFlags,
      AllFlags,
      ArgumentString
    );
  }
}