using System;
using System.IO;

using Newtonsoft.Json;

using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

/// <summary>
/// The configuration that applies per character and world.
/// </summary>
[Serializable]
public class PerPlayerConfig {
  /// <summary>
  /// The name of the player and owner of the configuration.
  /// </summary>
  public string? OwnerName { get; set; }

  /// <summary>
  /// The name of the player and owner of the configuration.
  /// </summary>
  public PlayerID? PlayerID { get; set; }

  /// <summary>
  /// The name of the player and owner of the configuration.
  /// </summary>
  [JsonIgnore]
  public string DisplayName {
    get {
      if (PlayerID is not null) {
        return PlayerID.ToString();
      }

      if (OwnerName is not null) {
        return $"{OwnerName}@unknown";
      }

      return "unknown@unknown";
    }
  }

  /// <summary>
  /// Test for edge cases where the player config is broken;
  /// </summary>
  [JsonIgnore]
  public bool IsBroken => OwnerName is null && PlayerID is null;

  /// <summary>
  /// The player's Free Company estate location.
  /// </summary>
  public HousingPlot FreeCompanyEstate { get; set; } = new HousingPlot();

  /// <summary>
  /// The player's Private estate location.
  /// </summary>
  public HousingPlot PrivateEstate { get; set; } = new HousingPlot();

  /// <summary>
  /// The player's Apartment location.
  /// </summary>
  public Apartment Apartment { get; set; } = new Apartment();

  /// <summary>
  /// Checks if the player config is new.
  /// </summary>
  [JsonIgnore]
  public bool IsNew => OwnerName == "Unknown";

  /// <summary>
  /// Checks if this is the current player config.
  /// </summary>
  [JsonIgnore]
  public bool IsCurrentPlayerConfig => System.IsLoggedIn && Config.GetCurrentPlayerConfig() is PerPlayerConfig playerConfig && this.DisplayName.Equals(playerConfig.DisplayName);

  /// <summary>
  /// Checks if the player config is valid.
  /// </summary>
  [JsonIgnore]
  public bool IsValid => !string.IsNullOrEmpty(OwnerName) && OwnerName != "Unknown" && (FreeCompanyEstate.IsValid() || PrivateEstate.IsValid() || Apartment.IsValid());

  /// <summary>
  /// The return booleans if the user hasn't visited their property in the days set.
  /// </summary>
  [JsonIgnore]
  public HousingTimes IsLate { get; set; } = HousingTimes.Blank;

  /// <summary>
  /// The return booleans if the user hasn't visited their property in the days set.
  /// </summary>
  [JsonIgnore]
  public Dismissed IsDismissed { get; set; } = new Dismissed();

  /// <summary>
  /// Ensures the config directory exists on the file system.
  /// </summary>
  public static void EnsureConfigDirectory() {
    if (Directory.Exists(Config.ConfigDirectory)) {
      Directory.CreateDirectory(Config.ConfigDirectory);
    }
  }

  public IWardProperty GetOfType(HousingType type) {
    return type switch {
      HousingType.FreeCompanyEstate => this.FreeCompanyEstate,
      HousingType.PrivateEstate => this.PrivateEstate,
      _ => this.Apartment
    };
  }
}
