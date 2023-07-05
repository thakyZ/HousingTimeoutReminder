using System;
using System.Collections.Generic;

using Dalamud.Configuration;
using Dalamud.Plugin;

using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
/// <summary>
/// The main plugin configuration file.
/// </summary>
[Serializable]
public class Configuration : IPluginConfiguration {
  /// <summary>
  /// The version of the Configuration.
  /// </summary>
  public int Version { get; set; } = 0;

  /// <summary>
  /// The list of player configs.
  /// </summary>
  public List<PerPlayerConfiguration> PlayerConfigs { get; set; } = new();

  /// <summary>
  /// The default days to wait for when to notify the player that they haven't visited their property for some time.
  /// </summary>
  public ushort DaysToWait { get; set; } = 28;

  /// <summary>
  /// The position of the warning dialog on the screen.
  /// </summary>
  public Position WarningPosition { get; set; } = new Position();

  /// <summary>
  /// An instanced version of the <see cref="DalamudPluginInterface"/>.
  /// </summary>
  [JsonIgnore]
  private DalamudPluginInterface pluginInterface { get; set; } = null!;

  /// <summary>
  /// Gets the current player config that exists.
  /// </summary>
  public static PerPlayerConfiguration? GetPlayerConfiguration() {
    return Services.Config.PlayerConfigs.Find(x => x.OwnerName == Plugin.GetCurrentPlayerName());
  }

  /// <summary>
  /// Initializes the plugin config.
  /// </summary>
  /// <param name="pluginInterface">An instanced version of the <see cref="DalamudPluginInterface"/>.</param>
  public void Initialize(DalamudPluginInterface pluginInterface) {
    this.pluginInterface = pluginInterface;

    if (DaysToWait > 30) {
      DaysToWait = 30;
    }
    if (DaysToWait < 1) {
      DaysToWait = 1;
    }

    switch (Version) {
      default: break;
    }
  }

  /// <summary>
  /// Saves the plugin config via the instanced <see cref="DalamudPluginInterface"/>.
  /// </summary>
  public void Save() {
    this.pluginInterface.SavePluginConfig(this);
  }
}
