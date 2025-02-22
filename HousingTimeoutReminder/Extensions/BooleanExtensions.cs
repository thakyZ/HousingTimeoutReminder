namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions;

/// <summary>
/// Extension Methods for <see langword="bool" /> instances.
/// </summary>
internal static class BooleanExtensions {
  /// <summary>
  /// Converts a <see langword="bool" /> into a <see langword="float" />.
  /// </summary>
  /// <param name="bool">The boolean value to input</param>
  /// <returns><code>1.0f</code> if parameter, <paramref name="bool"/>, is <see langword="true" />; otherwise <code>0.0f</code>.</returns>
  public static float ToFloat(this bool @bool)
    => @bool ? 1.0f : 0.0f;

  /// <summary>
  /// Toggles the state of a <see langword="bool" />.
  /// </summary>
  /// <param name="bool">The boolean value to toggle</param>
  /// <returns><see langword="true" /> if <paramref name="bool"/> was <see langword="false" />; otherwise <see langword="false" />.</returns>
  public static bool Toggle(this ref bool @bool) {
    @bool = !@bool;
    return @bool;
  }
}
