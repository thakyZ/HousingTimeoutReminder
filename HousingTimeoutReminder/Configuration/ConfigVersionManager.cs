using System.Linq;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

internal static class ConfigVersionManager {
#pragma warning disable CS0618 // Type or member is obsolete
  public static void DoMigration(PluginConfig config) {
    if (config.Version <= 0) {
      Svc.Log.Verbose("Migrating config to v{0}.", config.Version + 1);
      config.Version = 1;

      config.PlayerConfigs ??= [];
      config.PlayerConfigs.Add(new PerPlayerConfig {
        Apartment = config.Apartment,
        PrivateEstate = config.PrivateEstate,
        FreeCompanyEstate = config.FreeCompanyEstate,
        PlayerID = null,
      });
      config.Apartment = null;
      config.PrivateEstate = null;
      config.FreeCompanyEstate = null;
    }

    if (config.Version <= 1) {
      Svc.Log.Verbose("Migrating config to v{0}.", config.Version + 1);
      config.Version = 2;

      if (config.PlayerConfigs is not null) {
        foreach (PerPlayerConfig playerConfig in config.PlayerConfigs) {
          string newName = playerConfig.PlayerID is PlayerID playerID ? $"{playerID.FirstName} {playerID.LastName}" : "Legacy Data";
          Svc.Log.Verbose("Migrating player config, {0}.", newName);
          string newWorld = Systems.GetWorldName(playerConfig.PlayerID?.HomeWorld) ?? "";
          ulong index = PluginConfig.DUMMY_LEGACY_CONFIG_ID;
          while (config.PlayerEntries.Any(x => x.PlayerID == index)) {
            index++;
          }
          var entry = new PlayerConfigEntry {
            FileName = $"{index}_{newName}@{newWorld}.json",
            Name = newName,
            World = newWorld,
          };
          Svc.Log.Verbose("To file name {0}.", entry.FileName);
          config.PlayerEntries.AddUnsafe(index, entry);
          PlayerManager.SavePlayerConfig(entry, new PlayerConfig {
            Apartment = HousingPlot.ToHouseBaseSafe(playerConfig.Apartment),
            PrivateEstate = HousingPlot.ToHouseBaseSafe(playerConfig.PrivateEstate),
            FreeCompanyEstate = HousingPlot.ToHouseBaseSafe(playerConfig.FreeCompanyEstate),
          });
        }

        config.PlayerConfigs.Clear();
        config.PlayerConfigs = null;
      }

      if (config.WarningPosition is not null) {
        config.Global.WarningPosition = config.WarningPosition;
        config.WarningPosition = null;
      }

      if (config.DaysToWait.HasValue) {
        config.Global.DaysToWait = config.DaysToWait.Value;
        config.DaysToWait = null;
      }

      if (config.ShowAllPlayers.HasValue) {
        config.Global.ShowForAllPlayers = config.ShowAllPlayers.Value;
        config.ShowAllPlayers = null;
      }

      config.Global.NewPlayerHandling = (int)NewPlayerHandlingModes.Ask;
    }

    // insert migration code here

    if (config.Version < PluginConfig.CURRENT_VERSION) {
      throw new InvalidOperationException($"Missing migration to version {PluginConfig.CURRENT_VERSION}");
    }
  }
#pragma warning restore CS0618 // Type or member is obsolete
}
