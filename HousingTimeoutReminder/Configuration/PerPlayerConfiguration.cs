using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Text;

using Dalamud.Utility;

using Microsoft.VisualBasic;

using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
/// <summary>
/// The configuration that applies per character and world.
/// </summary>
[Serializable]
public class PerPlayerConfiguration {
  /// <summary>
  /// The name of the player and owner of the configuration.
  /// </summary>
  [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
  public string? OwnerName { get; set; } = null;
  /// <summary>
  /// The name of the player and owner of the configuration.
  /// </summary>
  [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
  public PlayerId? PlayerId { get; set; } = null;
  /// <summary>
  /// The name of the player and owner of the configuration.
  /// </summary>
  [JsonIgnore]
  public string DisplayName => PlayerId?.ToString() ?? (OwnerName is not null ? string.Concat(OwnerName, "@unknown") : "unknown");
  /// <summary>
  /// Test for edge cases where the player config is broken;
  /// </summary>
  [JsonIgnore]
  public bool IsBroken => OwnerName is null && PlayerId is null;
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
  /// Checks if the player config is valid.
  /// </summary>
  [JsonIgnore]
  public bool IsValid => !string.IsNullOrEmpty(OwnerName) && OwnerName != "Unknown" && (FreeCompanyEstate.IsValid() || PrivateEstate.IsValid() || Apartment.IsValid());

  /// <summary>
  /// The return booleans if the user hasn't visited their property in the days set.
  /// <see cref="FreeCompany"/>: The bool if player is late for their FC House.
  /// <see cref="Private"/>: The bool if player is late for their Private House.
  /// <see cref="Apartment"/>: The bool if player is late for their Apartment.
  /// </summary>
  [JsonIgnore]
  public (bool FreeCompany, bool Private, bool Apartment) IsLate { get; set; } = (false, false, false);

  /// <summary>
  /// The return booleans if the user has dismissed the warning for the property.
  /// <see cref="FreeCompany"/>: The bool if player has dismissed warning for their FC House.
  /// <see cref="Private"/>: The bool if player has dismissed warning for their Private House.
  /// <see cref="Apartment"/>: The bool if player has dismissed warning for their Apartment.
  /// </summary>
  [JsonIgnore]
  public (bool FreeCompany, bool Private, bool Apartment) IsDismissed { get; set; } = (false, false, false);

  /// <summary>
  /// Ensures the config directory exists on the file system.
  /// </summary>
  public static void EnsureConfigDirectory() {
    if (Directory.Exists(Configuration.ConfigDirectory)) {
      Directory.CreateDirectory(Configuration.ConfigDirectory);
    }
  }
}
