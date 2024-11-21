using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dalamud.Configuration;

using ECommons.DalamudServices;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Config.Data;
public class Config : IPluginConfiguration {
  /// <summary>
  /// Defined current version of the plugin's config.
  /// </summary>
  internal const int CurrentVersion = 2;
  /// <summary>
  /// Gets or sets the currently loaded config instance's version.
  /// </summary>
  public int Version { get; set; }
  /// <summary>
  /// Private instance of the <see cref="GlobalConfig"/> to ensure it exists.
  /// </summary>
  private GlobalConfig? _global;
  /// <summary>
  /// Gets or sets  the currently loaded config instance's <see cref="GlobalConfig"/>.
  /// </summary>
  public GlobalConfig Global {
    get => _global ??= new GlobalConfig();
    set => _global = value;
  }
  /// <summary>
  /// Gets or sets the currently loaded config instance's list of <see cref="PlayerConfig"/> entries.
  /// </summary>
  public Dictionary<ulong, PlayerConfigEntry> Players { get; set; } = [];

  /// <summary>
  /// Saves the current plugin's config instance.
  /// </summary>
  public static void Save() {
    Svc.PluginInterface.SavePluginConfig(Plugin.Systems.Config);
  }
}
