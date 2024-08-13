using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

using Dalamud.Configuration;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin;

using ECommons.DalamudServices;
using ECommons.GameHelpers;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
/// <summary>
/// The main plugin configuration file.
/// </summary>
[Serializable]
public class Config : IPluginConfiguration {
  /// <summary>
  /// The version of the Configuration.
  /// </summary>
  public int Version { get; set; } = 1;

  /// <summary>
  /// The list of player configurations.
  /// </summary>
  public List<PerPlayerConfig> PlayerConfigs { get; set; } = [];

  /// <summary>
  /// The configuration directory for the plugin.
  /// </summary>
  public static string ConfigDirectory => Svc.PluginInterface.GetPluginConfigDirectory();

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

  public void ResetAll() {
    foreach (var playerConfig in PlayerConfigs) {
      playerConfig.IsDismissed.Reset();
    }
  }

  /// <summary>
  /// Gets the current player config that exists.
  /// </summary>
  public static PerPlayerConfig? GetPlayerConfiguration() {
    if (Svc.ClientState.LocalPlayer is IPlayerCharacter player) {
      return System.PluginConfig.PlayerConfigs.Find(x => x.OwnerName == player.Name.TextValue);
    }

    return null;
  }

  /// <summary>
  /// Migrates the older config versions to newer ones.
  /// </summary>
  [SuppressMessage("Major Code Smell", "S907:\"goto\" statement should not be used", Justification = "Per Compiler Error CS0163")]
  public void Migrate() {
    switch (this.Version) {
      case 0:
        Svc.Log.Verbose("Migrating from v0.");
        this.MigrateToV1();
        goto default;
      case 1:
        Svc.Log.Verbose("Migrated from v1.");
        goto default;
      default:
        Svc.Log.Info("Migrated Config.");
        this.TryUpdateBrokenNames();
        return;
    }
  }

  /// <summary>
  /// A wrapper function to migrate from version 0 to version 1 of the config.
  /// </summary>
  public void MigrateToV1() {
    for (int index = 0; index < this.PlayerConfigs.Count; index++) {
      if (this.PlayerConfigs[index].OwnerName is string ownerName && this.PlayerConfigs[index].PlayerID is null) {
        if (System.IsLoggedIn) {
          var playerName = System.GetCurrentPlayerName();
          if (playerName is not null && ownerName == playerName.Split('@')[0]) {
            this.PlayerConfigs[index].PlayerID = System.GetCurrentPlayerID();
            this.PlayerConfigs[index].OwnerName = null;
          } else {
            this.PlayerConfigs[index].PlayerID = new PlayerID(ownerName);
            this.PlayerConfigs[index].OwnerName = null;
          }
        } else {
          this.PlayerConfigs[index].PlayerID = new PlayerID(ownerName);
          this.PlayerConfigs[index].OwnerName = null;
        }
      } else if (this.PlayerConfigs[index].OwnerName == "Unknown" && this.PlayerConfigs[index].PlayerID is null) {
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
  /// <returns>Returns true if it's being read from and written to. Returns false if it's not being read from and written to.</returns>
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
  private static void WaitUntilFileIsFree() {
    try {
      Svc.Log.Info("Starting wait task.");
      var waitedMoments = 0;
      Task.Run(async () => {
        while (IsFileInUseReadWrite(Svc.PluginInterface.ConfigFile)) {
          await Task.Delay(5000);
          waitedMoments++;
        }
      }).Wait();
      Svc.Log.Info($"Ended wait task. Waited {waitedMoments} time(s).");
    } catch (Exception exception) {
      Svc.Log.Error(exception, "Failed to start or run task to wait until config file is free.");
    }
  }

  /// <summary>
  /// Saves the plugin config via the instanced <see cref="DalamudPluginInterface"/>.
  /// </summary>
  public void Save() {
#if DEBUG
    Svc.Log.Info("Debug StackTrace:\n" + Environment.StackTrace);
#endif
    WaitUntilFileIsFree();
    Svc.PluginInterface.SavePluginConfig(this);
  }

  /// <summary>
  /// Adds a new <see cref="PerPlayerConfig"/> to the player config list,
  /// from the currently logged in player.
  /// </summary>
  /// <returns>Returns the same new player config, else returns null if
  /// the player is not logged in.</returns>
  internal static PerPlayerConfig? AddNewPlayerFromCurrent() {
    if (System.GetCurrentPlayerID() is not { } playerID) {
      return null;
    }

    var playerConfig = new PerPlayerConfig { PlayerID = playerID };
    System.PluginConfig.PlayerConfigs.Add(playerConfig);
    return playerConfig;
  }

  /// <summary>
  /// Gets the <see cref="PerPlayerConfig"/> from a matching <see cref="PlayerID">,
  /// if the player config doesn't exist it will create abstract new one from
  /// the current player config.
  /// </summary>
  /// <returns>Returns a player config for the current player config.</returns>
  internal static PerPlayerConfig? GetCurrentPlayerConfig() {
    if (System.GetCurrentPlayerID() is { } playerID) {
      return System.PluginConfig.PlayerConfigs.Find(x => x.PlayerID?.Equals(playerID) == true) ?? AddNewPlayerFromCurrent();
    }

    return null;
  }

  /// <summary>
  /// Adds a new <see cref="PerPlayerConfig"/> to the player config list,
  /// from a player identification.
  /// </summary>
  /// <param name="playerID">A player identification.</param>
  /// <returns>Returns the same new player config, else returns null if
  /// the player is not logged in.</returns>
  internal static PerPlayerConfig AddNewPlayerConfig(PlayerID playerID) {
    var playerConfig = new PerPlayerConfig { PlayerID = playerID };
    System.PluginConfig.PlayerConfigs.Add(playerConfig);
    return playerConfig;
  }

  /// <summary>
  /// Gets the <see cref="PerPlayerConfig"/> from a matching <see cref="PlayerID">,
  /// if the player config doesn't exist it will create abstract new one from
  /// the current player config.
  /// </summary>
  /// <param name="playerID">A player identification.</param>
  /// <returns>Returns a player config for the current player config.</returns>
  internal static PerPlayerConfig GetPlayerConfig(PlayerID playerID) {
    return System.PluginConfig.PlayerConfigs.Find(x => x.PlayerID?.Equals(playerID) == true) ?? AddNewPlayerConfig(playerID);
  }

  /// <summary>
  /// Tries to upgrade old config names.
  /// TODO: Actually make this work.
  /// </summary>
  internal void TryUpdateBrokenNames() {
    if (System.GetCurrentPlayerID() is not { } playerID) {
      return;
    }

    var playerConfig = System.PluginConfig.PlayerConfigs.Find(x => x.PlayerID?.Equals(playerID) == true);
    var updateFromOld = false;

    if (playerConfig is null) {
      playerConfig = System.PluginConfig.PlayerConfigs.Find(x => playerID.Equals(x.OwnerName));
      updateFromOld = true;
    }

    if (playerConfig is null) {
      return;
    }

    if (System.IsLoggedIn) {
      if (updateFromOld) {
        MigrateToV1();
      }
    } else if (playerConfig.IsBroken) {
      Svc.Log.Error("Player config is broken.");
    }
  }
}
