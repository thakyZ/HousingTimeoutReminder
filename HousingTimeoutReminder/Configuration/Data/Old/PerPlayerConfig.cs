using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

[Serializable]
[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
[Obsolete($"Use {nameof(PlayerConfigEntry)} instead")]
public class PerPlayerConfig {
  /// <summary>
  /// Gets or sets the old property for the owner name of the player's config.
  /// </summary>
  [Versions(introduced: 0, removed: 1)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(PlayerConfigEntry)}.{nameof(PlayerConfigEntry.Name)} instead")]
  public string? OwnerName { get; set; } = null;

  /// <summary>
  /// Gets or sets the old property for the <see cref="PlayerID" /> of the player's config.
  /// </summary>
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(PlayerConfigEntry)}.{nameof(PlayerConfigEntry.Name)} instead")]
  public PlayerID? PlayerID { get; set; } = null;

  /// <summary>
  /// Gets or sets the old property for the free company estate config of this player's config.
  /// </summary>
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(PlayerConfig)}.{nameof(PlayerConfig.FreeCompanyEstate)} instead")]
  public HousingPlot? FreeCompanyEstate { get; set; } = null;

  /// <summary>
  /// Gets or sets the old property for the private estate config of this player's config.
  /// </summary>
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(PlayerConfig)}.{nameof(PlayerConfig.PrivateEstate)} instead")]
  public HousingPlot? PrivateEstate { get; set; } = null;

  /// <summary>
  /// Gets or sets the old property for the apartment config of this player's config.
  /// </summary>
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(PlayerConfig)}.{nameof(PlayerConfig.Apartment)} instead")]
  public HousingPlot? Apartment { get; set; } = null;
}
