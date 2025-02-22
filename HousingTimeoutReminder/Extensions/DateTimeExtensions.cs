namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions;

/// <summary>
/// Extension Methods for <see cref="DateTime" /> instances.
/// </summary>
public static class DateTimeExtensions {
  /// <inheritdoc cref="DateTimeOffset.ToUnixTimeSeconds"/>
  public static long ToUnixTimeSeconds(this DateTime dateTime)
    => new DateTimeOffset(dateTime).ToUnixTimeSeconds();
  
  /// <inheritdoc cref="DateTimeOffset.ToUnixTimeMilliseconds"/>
  public static long ToUnixTimeMilliseconds(this DateTime dateTime)
    => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
}