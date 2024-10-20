using System;
using System.IO;

using Newtonsoft.Json;

using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;
using ECommons.DalamudServices;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using ECommons;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

/// <summary>
/// The configuration that applies per character and world.
/// </summary>
[Serializable]
public class PerPlayerConfig : IInterface {
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
      if (this.PlayerID is not null) {
        return this.PlayerID.ToString();
      }

      return "unknown@unknown";
    }
  }

  /// <summary>
  /// Test for edge cases where the player config is broken;
  /// </summary>
  [JsonIgnore]
  public bool IsBroken => PlayerID is null;

  /// <summary>
  /// The player's Free Company estate location.
  /// </summary>
  public HousingPlot FreeCompanyEstate { get; set; } = new();

  /// <summary>
  /// The player's Private estate location.
  /// </summary>
  public HousingPlot PrivateEstate { get; set; } = new();

  /// <summary>
  /// The player's Apartment location.
  /// </summary>
  public Apartment Apartment { get; set; } = new();

  /// <summary>
  /// Checks if this is the current player config.
  /// </summary>
  [JsonIgnore]
  public bool IsCurrentPlayerConfig => System.IsLoggedIn && Config.GetCurrentPlayerConfig() is PerPlayerConfig playerConfig && this.DisplayName.Equals(playerConfig.DisplayName);

  /// <summary>
  /// Checks if the player config is valid.
  /// </summary>
  [JsonIgnore]
  public bool IsValid => FreeCompanyEstate.IsValid() && PrivateEstate.IsValid() && Apartment.IsValid();

  /// <summary>
  /// The return booleans if the user hasn't visited their property in the days set.
  /// </summary>
  public bool IsLate(HousingType housingType) {
    var lastVisit = housingType switch {
      HousingType.FreeCompanyEstate => this.FreeCompanyEstate.LastVisit,
      HousingType.PrivateEstate => this.PrivateEstate.LastVisit,
      HousingType.Apartment => this.Apartment.LastVisit,
      _ => -1
    };
    if (lastVisit == -1) {
      return false;
    }
    return DateTimeOffset.FromUnixTimeSeconds(lastVisit).AddDays(System.PluginConfig.DaysToWait).ToUnixTimeSeconds() >= DateTimeOffset.Now.ToUnixTimeSeconds();
  }

  /// <summary>
  /// The return booleans if the user hasn't visited their property in the days set.
  /// </summary>
  [JsonIgnore]
  public Dismissed IsDismissed { get; set; } = new();

  /// <summary>
  /// Ensures the config directory exists on the file system.
  /// </summary>
  public static void EnsureConfigDirectory() {
    if (Directory.Exists(Config.ConfigDirectory)) {
      Directory.CreateDirectory(Config.ConfigDirectory);
    }
  }

  /// <summary>
  /// Gets the config for the type of housing.
  /// </summary>
  /// <param name="type">The type of housing to get the config for.</param>
  /// <returns>The config of the <see cref="HousingType" />.</returns>
  public IWardProperty GetOfType(HousingType type) {
    return type switch {
      HousingType.FreeCompanyEstate => this.FreeCompanyEstate,
      HousingType.PrivateEstate => this.PrivateEstate,
      _ => this.Apartment,
    };
  }

  [JsonExtensionData]
  [SuppressMessage("Roslynator", "RCS1169")]
  private IDictionary<string, JToken>? _additionalData = null;

  [OnDeserialized]
  private void OnDeserialized(StreamingContext context)
  {
    if (_additionalData is null) {
#if DEBUG
      Svc.Log.Warning("PerPlayerConfig _additionalData is null");
#endif
      return;
    }

#if DEBUG
    if (_additionalData.TryGetValue("OwnerName", out var value) && value.NotNull(out JToken token) && token.Value<string>() is string @string) {
      this.PlayerID = new PlayerID(@string);
      _additionalData.Remove("OwnerName");
    }
#endif
  }
}
