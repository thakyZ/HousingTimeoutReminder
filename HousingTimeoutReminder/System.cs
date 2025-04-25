using System.Linq;
using System.Threading.Tasks;

using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;

using ECommons.DalamudServices;

using Lumina.Excel;
using Lumina.Excel.Sheets;

using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public static class System {
  /// <summary>
  /// A static instance of the main plugin system.
  /// </summary>
  internal static Plugin PluginInstance { get; set; }
  /// <summary>
  /// The plugin configuration instance.
  /// </summary>
  internal static Config PluginConfig { get; set; }
  /// <summary>
  /// The window system of the plugin.
  /// </summary>
  public static WindowSystem WindowSystem { get; set; } = new(Plugin.Name.Replace(" ", string.Empty));
  /// <summary>
  /// The ui for warning the player that their house hasn't been visited
  /// in a while.
  /// </summary>
  public static WarningUI WarningUI { get; } = new();
  /// <summary>
  /// The ui for settings of the plugin.
  /// </summary>
  public static SettingsUI SettingsUI { get; } = new();
#if DEBUG
  /// <summary>
  /// The ui for debug window of the plugin.
  /// </summary>
  public static DebugUI DebugUI { get; } = new();
#endif

  /// <summary>
  /// Initializes the System class.
  /// This is required because we want to create specific classes after we
  /// initialize another class, in the plugin constructor.
  /// </summary>
  internal static void Init() {
    PluginConfig = (Svc.PluginInterface.GetPluginConfig() as Config ?? new Config()).Migrate();

    WindowSystem.AddWindow(SettingsUI);
    WindowSystem.AddWindow(WarningUI);
#if DEBUG
    WindowSystem.AddWindow(DebugUI);
#endif
  }

  /// <summary>
  /// Gets the row with sheet type specification
  /// </summary>
  /// <typeparam name="T">Type of a <see cref="ExcelRow"/>.</typeparam>
  /// <param name="rowID">The row number in the sheet</param>
  /// <returns>
  /// The row of the specified sheet, or null if that row does not exist.
  /// </returns>
  private static T? GetSheetAtRow<T>(uint rowID) where T : struct, IExcelRow<T> {
    return Svc.Data.GetExcelSheet<T>(ClientLanguage.English).FirstOrDefault(x => x.RowId == rowID);
  }

  /// <summary>
  /// Gets the name of a home world by the id of the home world.
  /// </summary>
  /// <param name="homeWorld">The id of the home world.</param>
  /// <returns>The name of the home world, or null if it doesn't exist
  /// (edge-case).</returns>
  internal static string? GetHomeWorldFromID(uint homeWorld) {
    return GetSheetAtRow<World>(homeWorld)?.Name.ExtractText();
  }

  /// <summary>
  /// Gets the name of a home world by the id of the home world.
  /// </summary>
  /// <param name="homeWorld">A nullable id of the home world.</param>
  /// <returns>The name of the home world, or null if it doesn't exist
  /// (edge-case), or if the input parameter is null.</returns>
  internal static string? GetHomeWorldFromID(uint? homeWorld) {
    if (homeWorld is null) {
      return null;
    }

    return GetSheetAtRow<World>(homeWorld.Value)?.Name.ExtractText();
  }

  internal static PlayerID? CachedCurrentPlayerId { get; set; }

  /// <summary>Gets the current player name of the logged in character.</summary>
  /// <returns>The full name plus separator plus the home world name.
  /// If the home world is unable to be obtained it will be "unknown" instead.
  /// If the player does not exist or is not logged in it returns null.</returns>
  internal static string? GetCurrentPlayerName() {
    return GetCurrentPlayerID()?.ToString();
  }

  /// <summary>Gets the current player identification information.</summary>
  /// <returns>The current player identification information.
  /// If the player does not exist or is not logged in it returns null.</returns>
  internal static PlayerID? GetCurrentPlayerID() {
    if (CachedCurrentPlayerId is not null && IsLoggedIn)
      return CachedCurrentPlayerId;
    return null;
  }

  /// <summary>
  /// Check if the user is logged into any character.
  /// </summary>
  public static bool IsLoggedIn => Svc.ClientState.IsLoggedIn;
}
