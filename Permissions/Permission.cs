using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Indra.Net {

  /// <summary>
  /// A permission that can be required in order to do, see, edit, make, or interact with something.
  /// </summary>
  public class Permission {

    /// <summary>
    /// The name ot use for permissions relating to the focus itself 
    /// (it's an empty string)
    /// </summary>
    public const string FocusSelfPermissionsName = "";

    /// <summary>
    /// Used to seperate the modes from the rest of the permission key and eachother
    /// </summary>
    public const char ModeSeperatorCharachter = '/';

    /// <summary>
    /// Seperates the a from the rest of the parameter name in an argument permission mode
    /// </summary>
    public const char ArgumentModePrefixSeperatorCharachter = '-';

    /// <summary>
    /// Various modes permissions can be used for by default.
    /// </summary>
    public enum ModeType {
      None = '\0',
      See = 's',
      Do = 'd',
      Be = 'b',
      Undo = 'u',
      Let = 'l',
      Mod = 'm',
      Own = 'o',
      Argument = 'a',
      All = '*'
    }

    /// <summary>
    /// The name part of the permission key.
    /// This is usually an action name.
    /// </summary>
    public string Name 
      { get; private set; }

    /// <summary>
    /// The selector for the focus/foci of this permission.
    /// </summary>
    public string FocusQuery 
      { get; private set; }

    /// <summary>
    /// The mode of the permission.
    /// </summary>
    public ModeType Mode {
      get; private set;
    } = ModeType.None;

    /// <summary>
    /// If this is an argument mode permission, what's the key?
    /// </summary>
    public string? ArgumentModeKey {
      get; private set;
    } = null;

    /// <summary>
    /// The full key for this permission.
    /// </summary>
    public string Key
      => _key ??= (Mode == ModeType.Argument
        ? BuildKey(Name, FocusQuery, new[] { Mode }, new[] { ArgumentModeKey! })
        : BuildKey(Name, FocusQuery, Mode));
    string _key = null!;

    internal Permission(string key, string focusQuery, ModeType mode, string? argumentModeKey = null) {
      Name = key;
      FocusQuery = focusQuery;
      Mode = mode;
      ArgumentModeKey = argumentModeKey;
    }

    ///<summary><inheritdoc/></summary>
    public override string ToString()
      => Key;

    /// <summary>
    /// Used to build a permission key's true form.
    /// </summary>
    public static string BuildKey(
      string name = FocusSelfPermissionsName,
      string focusQuery = Command.CurrentObjectFocusQueryKey,
      params ModeType[] modes
    ) => BuildKey(name, focusQuery, (IEnumerable<ModeType>)modes);

    /// <summary>
    /// Used to build a permission key's true form.
    /// </summary>
    public static string BuildKey(
      [NotNull] string name = FocusSelfPermissionsName,
      string? focusQuery = Command.CurrentObjectFocusQueryKey,
      IEnumerable<ModeType>? modes = null,
      string[]? argumentModeKeys = null
    ) {
      string key = "";

      if (focusQuery is not null) {
        key = focusQuery;
      }

      if (name is not null) {
        key += Command.ObjectActionSeperatorCharacter + name;
      }

      if (modes?.Any() ?? false) {
        int argumentModeIndex = 0;
        key += ModeSeperatorCharachter + string.Join(
          ModeSeperatorCharachter,
          modes.Select(m => 
            m == ModeType.Argument 
              ? (char)ModeType.Argument + "-" + argumentModeKeys![argumentModeIndex++].ToLower() 
              : m.ToString().ToLower()));
      }

      return key;
    }
  }
}
