using System.IO;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.Logging;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;
using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

// Place this after the namespace to avoid conflict.
using Config = Config;
using TaskAwaiter = System.Runtime.CompilerServices.ConfiguredTaskAwaitable<int>.ConfiguredTaskAwaiter;

internal sealed class PlayerManager {
  private PlayerConfig? _playerConfig;
  private ulong? _playerID;

  /// <summary>
  /// Gets the current <see cref="PlayerConfig" /> as a nullable.
  /// </summary>
  private PlayerConfig? GetCurrentPlayerConfig() {
    return _playerConfig ?? (Systems.IsLoggedIn ? this.GetPlayerConfig(Systems.CurrentPlayerID.Value, Systems.CurrentPlayer) : null);
  }

  /// <summary>
  /// Tries to get the current <see cref="PlayerConfig" />.
  /// </summary>
  /// <param name="result">The output instance of <see cref="PlayerConfig" />.</param>
  /// <returns><see langword="true" /> if found; otherwise <see langword="false" />.</returns>
  public bool TryGetCurrentPlayerConfig([NotNullWhen(true)] out PlayerConfig? result) {
    result = GetCurrentPlayerConfig();
    return result is not null;
  }

  /// <summary>
  /// Tries to get the current <see cref="PlayerConfig" />.
  /// </summary>
  /// <param name="playerConfig">The output instance of <see cref="PlayerConfig" />.</param>
  /// <param name="playerConfigEntry">The output instance of <see cref="PlayerConfigEntry" />.</param>
  /// <returns><see langword="true" /> if found; otherwise <see langword="false" />.</returns>
  public bool TryGetCurrentPlayerConfigAndEntry([NotNullWhen(true)] out PlayerConfig? playerConfig, [NotNullWhen(true)] out PlayerConfigEntry? playerConfigEntry) {
    playerConfig = GetCurrentPlayerConfig();
    playerConfigEntry = GetCurrentPlayerConfigEntry();
    return playerConfig is not null && playerConfigEntry is not null;
  }

  /// <summary>
  /// Gets the player config based on an id and an instance of <see cref="IPlayerCharacter"/>.
  /// </summary>
  /// <param name="playerID">A <see langword="ulong" /> resembling the player's ID.</param>
  /// <param name="player">An instance of <see cref="IPlayerCharacter"/>.</param>
  /// <returns>A new <see cref="PlayerConfig" /> if none found it will create a new one.</returns>
  public PlayerConfig GetPlayerConfig(ulong playerID, IPlayerCharacter player) {
    if (_playerConfig is { } cfg && playerID == _playerID) {
      return cfg;
    }

    _playerID = playerID;

    if (Plugin.Systems.Config.PlayerEntries.TryGetValue(playerID, out PlayerConfigEntry? entry)) {
      _playerConfig = LoadPlayerConfig(entry);
    }

    if (_playerConfig is null) {
      _playerConfig = CreatePlayerConfig();

      entry = new PlayerConfigEntry {
        Name = player.Name.TextValue,
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        World = player.HomeWorld.Value.Name.ExtractText() ?? string.Empty,
        FileName = $"{playerID}_{player.Name.TextValue.Replace(' ', '_')}@{player.HomeWorld.Value.Name.ExtractText()}.json",
      };

      Plugin.Systems.Config.PlayerEntries[playerID] = entry;
      SaveCurrentPlayerConfig(entry);
      Svc.PluginInterface.SavePluginConfig(Plugin.Systems.Config);
    }

    return _playerConfig;
  }

  /// <summary>
  /// Imports a <see cref="PlayerConfig" /> from a player id.
  /// </summary>
  /// <param name="fromPlayerID">A <see langword="ulong" /> resembling the player's ID.</param>
  /// <returns>
  /// <see langword="true" /> if the parameter <pramref name="fromPlayerID" /> is the current id or the config was
  /// successfully imported; otherwise <see langword="false" />.
  /// </returns>
  public bool Import(ulong fromPlayerID) {
    PluginLog.Debug($"Importing {fromPlayerID}");

    if (fromPlayerID == _playerID || _playerID is not ulong currentPlayer) {
      PluginLog.Debug("No use importing from current player");

      // importing from yourself is a noop and should therefore always succeed
      return true;
    }

    PlayerConfig? playerConfig = LoadPlayerConfig(fromPlayerID);

    if (playerConfig is null || _playerConfig is null) {
      List<string> items = [];

      if (playerConfig is null) {
        items.Add("imported config is null");
      }

      if (_playerConfig is null) {
        items.Add("current config is null");
      }

      PluginLog.Debug($"Unable to import: {string.Join(", ", items)}");

      return false;
    }

    PlayerConfigEntry? entry = Plugin.Systems.Config.PlayerEntries[currentPlayer];
    _playerConfig.CopyFrom(playerConfig);
    SaveCurrentPlayerConfig(entry);
    PluginLog.Debug("Import successful");

    return true;
  }

  /// <summary>
  /// Saves the current <see cref="PlayerConfig" /> to file.
  /// </summary>
  public void SaveCurrentPlayerConfig() {
    if (_playerID is not ulong playerID) {
      return;
    }

    PlayerConfigEntry? entry = Plugin.Systems.Config.PlayerEntries[playerID];
    SaveCurrentPlayerConfig(entry);
  }

  /// <summary>
  /// Saves the current <see cref="PlayerConfig" /> to file.
  /// </summary>
  /// <param name="entry">The <see cref="PlayerConfigEntry" /> tied to the <see cref="PlayerConfig" />.</param>
  /// <param name="config">An instance of a <see cref="PlayerConfig" />.</param>
  internal static void SavePlayerConfig(PlayerConfigEntry entry, PlayerConfig config) {
    string dir = GetPlayerConfigDir();

    if (!Directory.Exists(dir)) {
      _ = Directory.CreateDirectory(dir);
    }

    if (entry.FileName is null) {
      return;
    }

    File.WriteAllText(Path.Combine(dir, entry.FileName), JsonConvert.SerializeObject(config));
  }

  /// <summary>
  /// Loads a player config from a defined player ID.
  /// </summary>
  /// <param name="playerID">A <see langword="ulong" /> resembling the player's ID.</param>
  /// <returns>A new instance of <see cref="PlayerConfig" />. or <see langword="null" /> if loading failed.</returns>
  private PlayerConfig? LoadPlayerConfig(ulong playerID) {
    // ReSharper disable once InvertIf
    if (Plugin.Systems.Config.PlayerEntries.TryGetValue(playerID, out PlayerConfigEntry? entry)) {
      PlayerConfig? res = playerID == Config.DUMMY_LEGACY_CONFIG_ID ? LoadLegacyPlayerConfig() : LoadPlayerConfig(entry);

      return res;
    }

    return null;
  }

  /// <summary>
  /// Creates a new <see cref="PlayerConfig" /> from a legacy config options.
  /// </summary>
  /// <returns>A new instance of <see cref="PlayerConfig" />.</returns>
  private PlayerConfig LoadLegacyPlayerConfig() {
    var result = new PlayerConfig {
#pragma warning disable CS0618 // Type or member is obsolete
      FreeCompanyEstate = HousingPlot.ToHouseBaseSafe(Plugin.Systems.Config.FreeCompanyEstate),
      PrivateEstate = HousingPlot.ToHouseBaseSafe(Plugin.Systems.Config.PrivateEstate),
      Apartment = HousingPlot.ToHouseBaseSafe(Plugin.Systems.Config.Apartment),
#pragma warning restore CS0618 // Type or member is obsolete
    };

    return result;
  }

  /// <summary>
  /// Saves the current player config with the provided <see cref="PlayerConfigEntry" />.
  /// </summary>
  /// <param name="entry">An instance of a <see cref="PlayerConfigEntry" />.</param>
  private void SaveCurrentPlayerConfig(PlayerConfigEntry? entry) {
    if (entry is not null && _playerConfig is { } charConfig) {
      SavePlayerConfig(entry, charConfig);
    }
  }

  /// <summary>
  /// Loads the player config from the provided entry.
  /// </summary>
  /// <param name="entry">An instance of a <see cref="PlayerConfigEntry" />.</param>
  /// <returns>A loaded <see cref="PlayerConfig" /> or null if not found or the entry is null.</returns>
  internal PlayerConfig? LoadPlayerConfig(PlayerConfigEntry? entry) {
    // ReSharper disable once InvertIf
    if (entry?.FileName is not null /* can still be null if freshly loaded */) {
      string path = Path.Combine(GetPlayerConfigDir(), entry.FileName);

      // ReSharper disable once InvertIf
      if (File.Exists(path)) {
        try {
          return JsonConvert.DeserializeObject<PlayerConfig>(File.ReadAllText(path));
        } catch (IOException /* file deleted in the meantime. shouldn't happen, but technically can */) { }
      }
    }

    return null;
  }

  /// <inheritdoc cref="LoadPlayerConfig(PlayerConfigEntry?)"/>
  private async Task<PlayerConfig?> LoadPlayerConfigAsync(PlayerConfigEntry? entry) {
    // ReSharper disable once InvertIf
    if (entry?.FileName is not null /* can still be null if freshly loaded */) {
      string path = Path.Combine(GetPlayerConfigDir(), entry.FileName);

      // ReSharper disable once InvertIf
      if (File.Exists(path)) {
        try {
          await using FileStream fs = File.OpenRead(path);
          using var sr = new StreamReader(fs);
          return JsonConvert.DeserializeObject<PlayerConfig>(await sr.ReadToEndAsync());
        } catch (IOException /* file deleted in the meantime. shouldn't happen, but technically can */) { }
      }
    }

    return null;
  }

  /// <summary>
  /// Creates a new player config with the <see cref="PlayerConfig.IsNew" /> property set to <see langword="true" />.
  /// </summary>
  /// <returns>A new instance of a <see cref="PlayerConfig" />.</returns>
  private static PlayerConfig CreatePlayerConfig()
    => new PlayerConfig {
      IsNew = true,
    };

  /// <summary>
  /// Gets the directory path to the player config directory.
  /// </summary>
  /// <returns>A <see langword="string" /> reperesenting the path to the  player config directory.</returns>
  private static string GetPlayerConfigDir()
    => Plugin.PluginConfigDirectory;

  private static bool _locked;

  /// <summary>
  /// Asynchronously delves over each player config.
  /// Should allow everything to be run on the main thread.
  /// <remarks>Thank you, cat2002 💗, for figuring this code out.</remarks>
  /// <seealso href="https://github.com/bcssov/IronyModManager/commit/f8b528957fffa4f6c4c103520a7b132e7af6c721#diff-4ef0625c39dd0f92594a1e18f434db358d338f64b805ca82c072bca54ec2e0b3R760" />
  /// </summary>
  /// <param name="func">The function to delve over all player configs.</param>
  internal void Each(Action<PlayerConfigEntry, PlayerConfig> func) {
    if (_locked) {
      return;
    }

    _locked = true;
    TaskAwaiter task = Task.Run(() => {
      var failed = 0;
      var tasks = new List<Task<bool>>();

      foreach ((ulong playerID, PlayerConfigEntry configEntry) in Plugin.Systems.Config.PlayerEntries) {
        tasks.Add(Task.Run(async () => {
          try {
            if (await this.LoadPlayerConfigAsync(configEntry) is PlayerConfig playerConfig) {
              func(configEntry, playerConfig);
            }
          } catch (Exception ex) {
            Svc.Log.Error(ex, "Failed to run async method on player with name@world {0}@{1} and id {2}", configEntry.Name, configEntry.World, playerID);

            return false;
          }

          return true;
        }));
      }

      while (tasks.Count > 0) {
        int i = Task.WaitAny([..tasks]);
        failed += tasks[i].Result ? 0 : 1;
        tasks.RemoveAt(i);
      }

      return Task.FromResult(failed);
    }).ConfigureAwait(true).GetAwaiter();

    task.UnsafeOnCompleted(() => {
      int failed = task.GetResult();
      Svc.Log.Warning("Failed to run async method on {0} player configs", failed);
      Systems.NumberFailed = failed;
      _locked = false;
    });
  }

  internal static PlayerConfigEntry? GetCurrentPlayerConfigEntry() {
    return Systems.IsLoggedIn ? Plugin.Systems.Config.PlayerEntries[Systems.CurrentPlayerID.Value] : null;
  }
}
