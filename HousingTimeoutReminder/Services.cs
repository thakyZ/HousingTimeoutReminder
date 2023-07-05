using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Network;
using Dalamud.IoC;
using Dalamud.Plugin;

using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
internal class Services {
  public static Plugin PluginInstance { get; set; } = null!;
  public static Configuration Config { get; set; } = null!;
  public static HousingTimer HousingTimer { get; set; } = null!;

  [PluginService] public static ClientState ClientState { get; private set; } = null!;
  [PluginService] public static CommandManager CommandManager { get; private set; } = null!;
  [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
  [PluginService] public static Framework Framework { get; private set; } = null!;
  [PluginService] public static GameGui GameGui { get; private set; } = null!;
  [PluginService] public static GameNetwork GameNetwork { get; private set; } = null!;
  [PluginService] public static ObjectTable ObjectTable { get; private set; } = null!;
  [PluginService] public static SigScanner SigScanner { get; private set; } = null!;
}
