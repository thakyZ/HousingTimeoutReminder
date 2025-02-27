using System.Runtime.CompilerServices;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions;

/// <summary>
/// Extension Methods for <see langword="string" /> instances.
/// </summary>
internal static class StringExtensions {
  /// <summary>
  /// Returns a string with another provided string appended.
  /// </summary>
  /// <param name="string">The current instance of the string.</param>
  /// <param name="value">The instance of a string to append.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string Append(this string @string, string value) {
    return @string + value;
  }
  /// <summary>
  /// Returns a string with another provided string prepended.
  /// </summary>
  /// <param name="string">The current instance of the string.</param>
  /// <param name="value">The instance of a string to prepend.</param>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string Prepend(this string @string, string value) {
    return value + @string;
  }
}
