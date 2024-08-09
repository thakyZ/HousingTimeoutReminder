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

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
internal class Services {
  [NotNull, AllowNull]
  internal static Plugin Instance { get; private set; }
  [NotNull, AllowNull]
  internal static Configuration Config { get; private set; }
  [NotNull, AllowNull]
  internal static HousingTimer HousingTimer { get; private set; }
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
  /// <summary>
  /// The ui for debug window of the plugin.
  /// </summary>
  [NotNull, AllowNull]
  public static DebugUI DebugUI { get; } = new();

  [PluginService]
  [NotNull, AllowNull]
  public static IClientState ClientState { get; private set; }
  [PluginService]
  [NotNull, AllowNull]
  public static ICommandManager CommandManager { get; private set; }
  [PluginService]
  [NotNull, AllowNull]
  public static IDalamudPluginInterface PluginInterface { get; private set; }
  [PluginService]
  [NotNull, AllowNull]
  public static IFramework Framework { get; private set; }
  [PluginService]
  [NotNull, AllowNull]
  public static IGameGui GameGui { get; private set; }
  [PluginService]
  [NotNull, AllowNull]
  public static IGameNetwork GameNetwork { get; private set; }
  [PluginService]
  [NotNull, AllowNull]
  public static IObjectTable ObjectTable { get; private set; }
  [PluginService]
  [NotNull, AllowNull]
  public static IPluginLog Log { get; private set; }
  [PluginService]
  [NotNull, AllowNull]
  public static ISigScanner SigScanner { get; private set; }
  [PluginService]
  [NotNull, AllowNull]
  public static IGameInteropProvider GameInteropProvider { get; private set; }

  internal static void Init(IDalamudPluginInterface pluginInterface, Plugin plugin) {
    pluginInterface.Create<Services>();
    Instance = plugin;
    Config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
    Config.Initialize();
    WindowSystem.AddWindow(SettingsUI);
    WindowSystem.AddWindow(WarningUI);
#if DEBUG
    WindowSystem.AddWindow(DebugUI);
#endif
    Log.Info($"{ClientState.LocalPlayer is null}");
    HousingTimer = new HousingTimer();
  }
}
