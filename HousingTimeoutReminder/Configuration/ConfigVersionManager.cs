using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Old;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

// Place this after the namespace to avoid conflict.
using Config = Config;

internal static class ConfigVersionManager {
#pragma warning disable CS0618 // Type or member is obsolete
  public static void DoMigration(Config config) {
    if (config.Version <= 0) {
      config.Version = 1;

      config.PlayersConfigs ??= [];
      config.PlayersConfigs.Add(new PerPlayerConfig {
        Apartment = config.Apartment,
        PrivateEstate = config.PrivateEstate,
        FreeCompanyEstate = config.FreeCompanyEstate,
        PlayerID = null,
      });
    }

    if (config.Version <= 1) {
      config.Version = 2;

      if (config.PlayersConfigs is not null) {
        foreach (PerPlayerConfig playerConfig in config.PlayersConfigs) {
          string newName = playerConfig.PlayerID is PlayerID playerID
            ? $"{playerID.FirstName} {playerID.LastName}" : "Legacy Data";
          string newWorld = Systems.GetWorldName(playerConfig.PlayerID?.HomeWorld) ?? "";
          var entry = new Data.PlayerConfigEntry {
            FileName = $"{Config.DUMMY_LEGACY_CONFIG_ID}_{newName}@{newWorld}.json",
            Name = newName,
            World = newWorld,
          };
          config.PlayerEntries.AddUnsafe(Config.DUMMY_LEGACY_CONFIG_ID, entry);
          PlayerManager.SavePlayerConfig(entry, new Data.PlayerConfig {
            Apartment = HousingPlot.ToHouseBaseSafe(playerConfig.Apartment),
            PrivateEstate = HousingPlot.ToHouseBaseSafe(playerConfig.PrivateEstate),
            FreeCompanyEstate = HousingPlot.ToHouseBaseSafe(playerConfig.FreeCompanyEstate),
          });
        }
      }

      if (config.WarningPosition is not null) {
        config.Global.WarningPosition = config.WarningPosition;
      }

      if (config.DaysToWait.HasValue) {
        config.Global.DaysToWait = config.DaysToWait.Value;
      }

      if (config.ShowAllPlayers.HasValue) {
        config.Global.ShowForAllPlayers = config.ShowAllPlayers.Value;
      }

      config.Global.NewPlayerHandling = (int)NewPlayerHandlingModes.Ask;
    }

    // insert migration code here

    if (config.Version < Config.CURRENT_VERSION) {
      throw new InvalidOperationException($"Missing migration to version {Config.CURRENT_VERSION}");
    }
  }
#pragma warning restore CS0618 // Type or member is obsolete
}
