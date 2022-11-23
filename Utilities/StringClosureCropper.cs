using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Utility to help crop enclosed sections from a string.
/// </summary>
internal class StringClosureCropper {

  public string Source { get; }

  public (char start, char end)[] DelimiterSets { get; }

  public string CroppedResult
    => _croppedResult ?? _crop().result;
  string _croppedResult;

  public ((char start, char end) delimiters, string contents)[] RemovedSections 
    => _removedSections ?? _crop().removed;
  ((char start, char end) delimiters, string contents)[] _removedSections;

  public StringClosureCropper(string source, params (char start, char end)[] delimiterSets) {
    Source = source;
    DelimiterSets = delimiterSets;
  }

  (string result, ((char start, char end) delimiters, string contents)[] removed) _crop() {
    var delimiters = DelimiterSets.ToDictionary(
      s => s.start,
      s => (char?)s.end
    );

    string result = "";
    List<((char start, char end) delimiters, string contents)> removed = new();

    char? expectedEndChar = null;
    char? currentBeginChar = null;
    int depth = 0;

    foreach (char current in Source) {
      if (expectedEndChar == null) {
        if (delimiters.TryGetValue(current, out expectedEndChar)) {
          currentBeginChar = current;
          removed.Add(((current, default), ""));
          result += "{" + (removed.Count - 1) + "}";
          continue;
        }
        else {
          result += current;
          continue;
        }
      }
      else {
        if (current == expectedEndChar) {
          if (depth == 0) {
            removed[removed.Count - 1]
              = (
               (
                 removed[removed.Count - 1].delimiters.start,
                 current
               ),
               removed[removed.Count - 1].contents
              );

            expectedEndChar = null;
            currentBeginChar = null;
            continue;
          } else {
            depth--;
          }
        }
        else if (current == currentBeginChar) {
          depth++;
        }

        removed[removed.Count - 1]
          = (
            (
              removed[removed.Count - 1].delimiters.start,
              removed[removed.Count - 1].delimiters.end
            ),
            removed[removed.Count - 1].contents + current
          );
        continue;
      }
    }

    _croppedResult = result;
    _removedSections = removed.ToArray();
    return (result, removed.ToArray());
  }
}