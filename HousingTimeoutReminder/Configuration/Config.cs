using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Dalamud.Configuration;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin;
using ECommons.DalamudServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
/// <summary>
/// The main plugin configuration file.
/// </summary>
[Serializable]
public class Config : IPluginConfiguration {
  internal const int CurrentVersion = 1;

  /// <summary>
  /// The version of the Configuration.
  /// </summary>
  public int Version { get; set; } = 1;

  /// <summary>
  /// The list of player configurations.
  /// </summary>
  public List<PerPlayerConfig> PlayerConfigs { get; set; } = [];

  /// <summary>
  /// The list of player configurations.
  /// </summary>
  [JsonIgnore]
  public List<PerPlayerConfig> PlayerConfigsWithWarnings => [.. PlayerConfigs.Where((PerPlayerConfig p) => p.HasLateProperties)];

  /// <summary>
  /// The configuration directory for the plugin.
  /// </summary>
  public static string ConfigDirectory => Svc.PluginInterface.GetPluginConfigDirectory();

  /// <summary>
  /// The default days to wait for when to notify the player that they haven't visited their property for some time.
  /// </summary>
  public ushort DaysToWait { get; set; } = 28;

  /// <summary>
  /// Gets the amount of days to wait in Unix Time (Seconds).
  /// </summary>
  public long DateToWaitSeconds => DateTimeOffset.MinValue.AddDays(DaysToWait).ToUnixTimeSeconds() - DateTimeOffset.MinValue.ToUnixTimeSeconds();

  /// <summary>
  /// The position of the warning dialog on the screen.
  /// </summary>
  public Position WarningPosition { get; set; } = new();

  /// <summary>
  /// Shows warnings for all players regardless of who is logged in.
  /// </summary>
  public bool ShowAllPlayers { get; set; } = false;

  public void ResetAll() {
    foreach (var playerConfig in PlayerConfigs) {
      playerConfig.ResetWarnings();
    }
  }

  /// <summary>
  /// Gets the current player config that exists.
  /// </summary>
  public static PerPlayerConfig? PlayerConfiguration => System.PluginConfig.PlayerConfigs.FirstOrDefault(x => x.PlayerID == System.GetCurrentPlayerID());

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
      //Svc.Log.Info("Starting wait task.");
      var waitedMoments = 0;
      Task.Run(async () => {
        while (IsFileInUseReadWrite(Svc.PluginInterface.ConfigFile)) {
          await Task.Delay(5000);
          waitedMoments++;
        }
      }).Wait();
      //Svc.Log.Info($"Ended wait task. Waited {waitedMoments} time(s).");
    } catch (Exception exception) {
      Svc.Log.Error(exception, "Failed to start or run task to wait until config file is free.");
    }
  }

  /// <summary>
  /// Saves the plugin config via the instanced <see cref="DalamudPluginInterface"/>.
  /// </summary>
  public void Save() {
#if DEBUG
    //Svc.Log.Info("Debug StackTrace:\n" + Environment.StackTrace);
#endif
    WaitUntilFileIsFree();
    Svc.PluginInterface.SavePluginConfig(this);
  }

  /// <summary>
  /// Adds a new <see cref="PerPlayerConfig" /> to the player config list,
  /// from the currently logged in player.
  /// </summary>
  /// <returns>Returns the same new player config, else returns null if
  /// the player is not logged in.</returns>
  internal static PerPlayerConfig? AddNewPlayerFromCurrent() {
    if (System.GetCurrentPlayerID() is not PlayerID playerID) {
      return null;
    }

    var playerConfig = new PerPlayerConfig { PlayerID = playerID };
    System.PluginConfig.PlayerConfigs.Add(playerConfig);
    return playerConfig;
  }

  /// <summary>
  /// Adds a new <see cref="PerPlayerConfig" /> to the player config list,
  /// from a player identification.
  /// </summary>
  /// <param name="playerID">A player identification.</param>
  /// <returns>Returns the same new player config, else returns null if
  /// the player is not logged in.</returns>
  internal static PerPlayerConfig AddNewPlayerConfig(PlayerID playerID) {
    var playerConfig = new PerPlayerConfig { PlayerID = playerID };
    if (!System.PluginConfig.PlayerConfigs.Any(x => x.PlayerID == playerID)) {
      System.PluginConfig.PlayerConfigs.Add(playerConfig);
      return playerConfig;
    }

    return System.PluginConfig.PlayerConfigs.First(x => x.PlayerID == playerID);
  }

  /// <summary>
  /// Gets the <see cref="PerPlayerConfig" /> from a matching <see cref="PlayerID" />,
  /// if the player config doesn't exist it will create abstract new one from
  /// the current player config.
  /// </summary>
  /// <param name="playerID">A player identification.</param>
  /// <returns>Returns a player config for the current player config.</returns>
  internal static PerPlayerConfig GetPlayerConfig(PlayerID playerID) {
    return System.PluginConfig.PlayerConfigs.FirstOrDefault(x => x.PlayerID == playerID) ?? AddNewPlayerConfig(playerID);
  }

  [JsonExtensionData]
  [SuppressMessage("Roslynator", "RCS1169")]
  private IDictionary<string, JToken>? _additionalData;

  [OnDeserialized]
  private void OnDeserialized(StreamingContext context) {
    if (_additionalData is null) {
#if DEBUG
      Svc.Log.Warning("Config _additionalData is null");
#endif
      return;
    }
  }
}

public static class ConfigExtensions {
  /// <summary>
  /// Migrates the older config versions to newer ones.
  /// </summary>
  public static Config Migrate(this Config config) {
    if (config.Version == Config.CurrentVersion) return config;

    while (config.Version < Config.CurrentVersion) {
      switch (config.Version) {
        case 0:
          Svc.Log.Verbose("Migrating from v0.");
          config = config.MigrateToV1().TryUpdateBrokenNames().Migrate();
          break;
        case 1:
          Svc.Log.Verbose("Migrated from v1.");
          break;
      }
    }

    Svc.Log.Info("Migrated Config.");
    return config;
  }

  /// <summary>
  /// A wrapper function to migrate from version 0 to version 1 of the config.
  /// </summary>
  public static Config MigrateToV1(this Config config) {
    /*for (int index = 0; index < config.PlayerConfigs.Count; index++) {
      switch (config.PlayerConfigs[index].OwnerName)
      {
        case string ownerName when config.PlayerConfigs[index].PlayerID is null:
          if (System.IsLoggedIn) {
            var playerID = System.GetCurrentPlayerID();
            config.PlayerConfigs[index].PlayerID = playerID ?? new PlayerID(ownerName);
          } else {
            config.PlayerConfigs[index].PlayerID = new PlayerID(ownerName);
          }
          config.PlayerConfigs[index].OwnerName = null;
          break;
        case null when config.PlayerConfigs[index].PlayerID is null:
          config.PlayerConfigs.RemoveAt(index);
          break;
        default:
          if (config.PlayerConfigs[index].IsBroken) {
            config.PlayerConfigs.RemoveAt(index);
          }
          break;
      }
    }*/
    config.Version = 1;
    return config;
  }

  /// <summary>
  /// Tries to upgrade old config names.
  /// </summary>
  internal static Config TryUpdateBrokenNames(this Config config) {
    if (System.GetCurrentPlayerID() is not PlayerID playerID) {
      return config;
    }

    if (!config.PlayerConfigs.Exists(x => x.PlayerID is not null && x.PlayerID.HomeWorld is null)) {
      return config;
    }

    int index = config.PlayerConfigs.FindIndex(x => x.PlayerID is not null && x.PlayerID.HomeWorld is null);
    config.PlayerConfigs[index].PlayerID = playerID;
    return config;
  }
}
