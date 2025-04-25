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
  /// Gets a <see langword="bool" /> that determines if any of the enabled housing properties are late.
  /// </summary>
  [JsonIgnore]
  public bool HasLateProperties => this.FreeCompanyEstate.IsLate || this.PrivateEstate.IsLate || this.Apartment.IsLate;

  /// <summary>
  /// Checks if this is the current player config.
  /// </summary>
  [JsonIgnore]
  public bool IsCurrentPlayerConfig => System.IsLoggedIn && Config.PlayerConfiguration?.DisplayName.Equals(this.DisplayName) == true;

  /// <summary>
  /// Resets the dismissed states of all properties.
  /// </summary>
  public void ResetWarnings() {
    this.FreeCompanyEstate.IsDismissed = false;
    this.PrivateEstate.IsDismissed = false;
    this.Apartment.IsDismissed = false;
  }

  /// <summary>
  /// Checks if the player config is valid.
  /// </summary>
  [JsonIgnore]
  public bool IsValid => FreeCompanyEstate.IsValid && PrivateEstate.IsValid && Apartment.IsValid;

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
  private IDictionary<string, JToken>? _additionalData;

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
