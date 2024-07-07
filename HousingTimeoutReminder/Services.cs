using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Dalamud.Game;
using Dalamud.Game.ClientState.Resolvers;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;

using XivCommon;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
internal class Services {
  [NotNull, AllowNull]
  internal static Plugin Instance { get; private set; }
  [NotNull, AllowNull]
  internal static Configuration Config { get; private set; }
  [NotNull, AllowNull]
  internal static HousingTimer HousingTimer { get; private set; }
  /// <summary>
  /// XIVCommon base instance that allows you to get the housing district location.
  /// </summary>
  [NotNull, AllowNull]
  internal static XivCommonBase XivCommon { get; private set; }
  /// <summary>
  /// The window system of the plugin.
  /// </summary>
  [NotNull, AllowNull]
  public static WindowSystem WindowSystem { get; } = new(Plugin.StaticName.Replace(" ", string.Empty));
  /// <summary>
  /// The ui for warning the player that their house hasn't been visited in a while.
  /// </summary>
  [NotNull, AllowNull]
  public static WarningUI WarningUI { get; } = new();
  /// <summary>
  /// The ui for settings of the plugin.
  /// </summary>
  [NotNull, AllowNull]
  public static SettingsUI SettingsUI { get; } = new();

  [NotNull, AllowNull][PluginService] public static IClientState           ClientState         { get; private set; }
  [NotNull, AllowNull][PluginService] public static IDataManager           DataManager         { get; private set; }
  [NotNull, AllowNull][PluginService] public static ICommandManager        CommandManager      { get; private set; }
  [NotNull, AllowNull][PluginService] public static DalamudPluginInterface PluginInterface     { get; private set; }
  [NotNull, AllowNull][PluginService] public static IFramework             Framework           { get; private set; }
  [NotNull, AllowNull][PluginService] public static IGameGui               GameGui             { get; private set; }
  [NotNull, AllowNull][PluginService] public static IGameNetwork           GameNetwork         { get; private set; }
  [NotNull, AllowNull][PluginService] public static IObjectTable           ObjectTable         { get; private set; }
  [NotNull, AllowNull][PluginService] public static IPluginLog             PluginLog           { get; private set; }
  [NotNull, AllowNull][PluginService] public static ISigScanner            SigScanner          { get; private set; }
  [NotNull, AllowNull][PluginService] public static IGameInteropProvider   GameInteropProvider { get; private set; }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="rowId"></param>
  /// <returns></returns>
  private static T? GetSheetAtRow<T>(uint rowId) where T : ExcelRow {
    var sheet = DataManager.GetExcelSheet<T>();
    if (sheet is null) return null;
    return sheet.First(x => x.RowId == rowId);
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="homeWorld"></param>
  /// <returns></returns>
  internal static string? GetHomeWorldFromId(uint homeWorld) {
    var sheet = GetSheetAtRow<World>(homeWorld);
    if (sheet is null) return null;
    return sheet.Name;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="homeWorld"></param>
  /// <returns></returns>
  internal static string? GetHomeWorldFromId(uint? homeWorld) {
    if (!homeWorld.HasValue) return null;
    var sheet = GetSheetAtRow<World>(homeWorld.Value);
    if (sheet is null) return null;
    return sheet.Name;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="homeWorld"></param>
  /// <returns></returns>
  internal static string? GetHomeWorldFromId(ExcelResolver<World> homeWorld) {
    var sheet = homeWorld.GetWithLanguage(Dalamud.ClientLanguage.English);
    if (sheet is null) return null;
    return sheet.Name;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <returns></returns>
  internal static string? GetCurrentPlayerName() {
    return $"{ClientState.LocalPlayer?.Name.TextValue}@{GetHomeWorldFromId(ClientState.LocalPlayer?.HomeWorld.Id) ?? "unknown"}";
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <returns></returns>
  internal static PlayerId? GetCurrentPlayerId() {
    var firstLastName = ClientState.LocalPlayer?.Name.TextValue;
    if (firstLastName is null) return null;
    var fistLastNameSplit = firstLastName.Split(' ');
    return new PlayerId(fistLastNameSplit[0], fistLastNameSplit[1], ClientState.LocalPlayer?.HomeWorld.Id);
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public static bool IsLoggedIn => ClientState.IsLoggedIn;

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="pluginInterface"></param>
  /// <param name="plugin"></param>
  internal static void Init(DalamudPluginInterface pluginInterface, Plugin plugin) {
    pluginInterface.Create<Services>();
    Instance = plugin;
    Config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
    Config.Initialize();
    Config.Migrate();
    XivCommon = new XivCommonBase(pluginInterface, Hooks.None);
    WindowSystem.AddWindow(SettingsUI);
    WindowSystem.AddWindow(WarningUI);
#if DEBUG
    PluginLog.Debug($"{ClientState.LocalPlayer is null}");
#endif
    HousingTimer = new HousingTimer();
  }
}
