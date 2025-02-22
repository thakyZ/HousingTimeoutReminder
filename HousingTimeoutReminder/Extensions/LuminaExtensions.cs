using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions;

/// <summary>
/// Extension Methods for the namespace <see langword="Lumina.Excel" />.
/// </summary>
internal static class LuminaExtensions {
  /// <summary>
  /// Gets the name of the <see cref="World" />.
  /// </summary>
  /// <param name="rowRefWorld">The current instance of a row reference.</param>
  public static string? GetName(this RowRef<World> rowRefWorld)
    => rowRefWorld.ValueNullable?.Name.ExtractText() ?? null;
}