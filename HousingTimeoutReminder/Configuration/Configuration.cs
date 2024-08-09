using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

using Dalamud.Configuration;
using Dalamud.Plugin;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
/// <summary>
/// The main plugin configuration file.
/// </summary>
[Serializable]
public class Configuration : IPluginConfiguration {
  /// <summary>
  /// The version of the Configuration.
  /// </summary>
  public int Version { get; set; } = 1;

  /// <summary>
  /// The list of player configurations.
  /// </summary>
  public List<PerPlayerConfiguration> PlayerConfigs { get; set; } = [];

  /// <summary>
  /// The configuration directory for the plugin.
  /// </summary>
  public List<PerPlayerConfiguration> PlayerConfigs { get; set; } = [];

  /// <summary>
  /// The configuration directory for the plugin.
  /// </summary>
  public static string ConfigDirectory => Services.PluginInterface.GetPluginConfigDirectory();

  /// <summary>
  /// The default days to wait for when to notify the player that they haven't visited their property for some time.
  /// </summary>
  public ushort DaysToWait { get; set; } = 28;

  /// <summary>
  /// The position of the warning dialog on the screen.
  /// </summary>
  public Position WarningPosition { get; set; } = new Position();

  /// <summary>
  /// Shows warnings for all players regardless of who is logged in.
  /// </summary>
  public bool ShowAllPlayers { get; set; } = false;

  /// <summary>
  /// Gets the current player config that exists.
  /// </summary>
  public static PerPlayerConfiguration? GetPlayerConfiguration() {
    return Services.Config.PlayerConfigs.Find(x => x.OwnerName == Services.ClientState.LocalPlayer?.Name.TextValue);
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  [SuppressMessage("Major Code Smell", "S907:\"goto\" statement should not be used", Justification = "Per Compiler Error CS0163")]
  public void Migrate() {
    switch (this.Version) {
      case 0:
        Services.PluginLog.Verbose("Migrating from v0.");
        this.MigrateToV1();
        goto default;
      case 1:
        Services.PluginLog.Verbose("Migrated from v1.");
        goto default;
      default:
        Services.PluginLog.Info("Migrated Config.");
        this.TryUpdateBrokenNames();
        return;
    }
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public void MigrateToV1() {
    for (int index = 0; index < this.PlayerConfigs.Count; index++) {
      if (this.PlayerConfigs[index].OwnerName is string ownerName && this.PlayerConfigs[index].PlayerId is null) {
        if (Services.IsLoggedIn) {
          var playerName = Services.GetCurrentPlayerName();
          if (playerName is string strPlayerName && ownerName == strPlayerName.Split('@')[0]) {
            this.PlayerConfigs[index].PlayerId = Services.GetCurrentPlayerId();
            this.PlayerConfigs[index].OwnerName = null;
          } else {
            this.PlayerConfigs[index].PlayerId = new PlayerId(ownerName);
            this.PlayerConfigs[index].OwnerName = null;
          }
        } else {
          this.PlayerConfigs[index].PlayerId = new PlayerId(ownerName);
          this.PlayerConfigs[index].OwnerName = null;
        }
      } else if (this.PlayerConfigs[index].OwnerName == "Unknown" && this.PlayerConfigs[index].PlayerId is null) {
        this.PlayerConfigs.RemoveAt(index);
      } else if (this.PlayerConfigs[index].IsBroken) {
        this.PlayerConfigs.RemoveAt(index);
      }
    }
    this.Version = 1;
    this.Save();
  }

  /// <summary>
  /// Initializes the plugin config.
  /// </summary>
  public void Initialize() {
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
  /// Checks if the file specified is able to be read from and written to.
  /// </summary>
  /// <param name="file">The file to test if it can be read from and written to.</param>
  /// <returns>Returns <see cref="true" /> if it's being read from and written to. Returns <see cref="false" /> if it's not being read from and written to.</returns>
  private static bool IsFileInUseReadWrite(FileInfo file) {
    try {
      using var stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
    } catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32) {
      return true;
    }
    return false;
  }

  /// <summary>
  /// Waits until the config file is free and able to be written to.
  /// </summary>
  private void WaitUntilFileIsFree() {
    try {
      Services.PluginLog.Info("Starting wait task.");
      var waitedMoments = 0;
      Task.Run(async () => {
        while (IsFileInUseReadWrite(Services.PluginInterface.ConfigFile)) {
          await Task.Delay(5000);
          waitedMoments++;
        }
      }).Wait();
      Services.PluginLog.Info($"Ended wait task. Waited {waitedMoments} time(s).");
    } catch (Exception exception) {
      Services.PluginLog.Error(exception, $"Failed to start or run task to wait until config file is free.");
    }
  }

  /// <summary>
  /// Saves the plugin config via the instanced <see cref="DalamudPluginInterface"/>.
  /// </summary>
  public void Save() {
    Services.PluginLog.Info(Environment.StackTrace.ToString());
    WaitUntilFileIsFree();
    Services.PluginInterface.SavePluginConfig(this);
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <returns></returns>
  internal static PerPlayerConfiguration AddNewPlayerFromCurrent() {
    var playerConfig = new PerPlayerConfiguration{PlayerId=Services.GetCurrentPlayerId()};
    Services.Config.PlayerConfigs.Add(playerConfig);
    return playerConfig;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="playerId"></param>
  /// <returns></returns>
  internal static PerPlayerConfiguration GetPlayerConfig(PlayerId playerId) {
    return Services.Config.PlayerConfigs.Find(x => x.PlayerId == playerId) ?? AddNewPlayerFromCurrent();
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <returns></returns>
  internal static PerPlayerConfiguration GetCurrentPlayerConfig() {
    return Services.Config.PlayerConfigs.Find(x => x.PlayerId == Services.GetCurrentPlayerId()) ?? AddNewPlayerFromCurrent();
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <returns></returns>
  internal static PlayerId? GetCurrentPlayerId() {
    return GetCurrentPlayerConfig().PlayerId is not null ? GetCurrentPlayerConfig().PlayerId : Services.GetCurrentPlayerId();
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  internal void TryUpdateBrokenNames() {
    var playerConfig = Services.Config.PlayerConfigs.Find(x => x.PlayerId == Services.GetCurrentPlayerId());
    var updateFromOld = false;
    if (playerConfig is null) {
      playerConfig = Services.Config.PlayerConfigs.Find(x => x.OwnerName == Services.GetCurrentPlayerId()?.ToString());
      updateFromOld = true;
    }
    if (playerConfig is null) return;
    if (Services.IsLoggedIn) {
      if (updateFromOld) {
        MigrateToV1();
      }
    } else if (playerConfig.IsBroken) {
      Services.PluginLog.Error("Player config is broken.");
    }
  }
}
