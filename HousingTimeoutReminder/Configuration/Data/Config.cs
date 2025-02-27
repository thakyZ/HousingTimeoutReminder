using System.Runtime.Serialization;
using Dalamud.Configuration;
using Newtonsoft.Json;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Converters;
using Newtonsoft.Json.Linq;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;

[Serializable]
[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
public partial class Config : IPluginConfiguration {
  /// <summary>
  /// Defined current version of the plugin's config.
  /// </summary>
  internal const int CURRENT_VERSION = 2;

  /// <summary>
  /// Defined legacy version of the plugin's config.
  /// </summary>
  internal const ulong DUMMY_LEGACY_CONFIG_ID = 0;

  /// <summary>
  /// Gets or sets the currently loaded config instance's version.
  /// </summary>
  [Versions(introduced: 0)]
  public int Version { get; set; } = CURRENT_VERSION;

  /// <summary>
  /// Private instance of the <see cref="GlobalConfig" /> to ensure it exists.
  /// </summary>
  private GlobalConfig? _global;

  /// <summary>
  /// Gets or sets the currently loaded config instance's <see cref="GlobalConfig" />.
  /// </summary>
  [Versions(introduced: 2)]
  public GlobalConfig Global {
    get => _global ??= new GlobalConfig();
    set => _global = value;
  }

  /// <summary>
  /// Gets or sets the currently loaded config instance's list of <see cref="PlayerConfig" /> entries.
  /// </summary>
  [JsonConverter(typeof(PlayerConfigEntryKeyValuePairCollectionJsonConverter))]
  public PlayerConfigEntryKeyValuePairCollection PlayerEntries { get; set; } = [];

  [JsonIgnore]
  public PlayerConfigKeyValuePairCollection PlayerConfigEntries { get; set; } = [];

#region Obsolete
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(Config)}.{nameof(PlayerEntries)} instead")]
  public List<PerPlayerConfig>? PlayerConfigs { get; set; }

  /// <summary>
  /// Gets or sets the old property for the number of days to wait until notifications show again.
  /// </summary>
  [Versions(introduced: 0, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(GlobalConfig)}.{nameof(GlobalConfig.DaysToWait)} instead")]
  public int? DaysToWait { get; set; }

  /// <summary>
  /// Gets or sets old property for the position of the warning dialog on the screen.
  /// </summary>
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(GlobalConfig)}.{nameof(GlobalConfig.WarningPosition)} instead")]
  public Position? WarningPosition { get; set; }

  /// <summary>
  /// Gets or sets old property for whether to show all players for notifications, otherwise just the currently logged in player.
  /// </summary>
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(GlobalConfig)}.{nameof(GlobalConfig.ShowForAllPlayers)} instead")]
  public bool? ShowAllPlayers { get; set; }

  /// <inheritdoc cref="PerPlayerConfig.FreeCompanyEstate"/>
  [Versions(introduced: 0, removed: 1)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(GlobalConfig)}.{nameof(PlayerConfig.FreeCompanyEstate)} instead")]
  public HousingPlot? FreeCompanyEstate { get; set; }

  /// <inheritdoc cref="PerPlayerConfig.PrivateEstate"/>
  [Versions(introduced: 0, removed: 1)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(GlobalConfig)}.{nameof(PlayerConfig.PrivateEstate)} instead")]
  public HousingPlot? PrivateEstate { get; set; }

  /// <inheritdoc cref="PerPlayerConfig.Apartment"/>
  [Versions(introduced: 0, removed: 1)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(GlobalConfig)}.{nameof(PlayerConfig.Apartment)} instead")]
  public HousingPlot? Apartment { get; set; }
#endregion Obsolete

  /// <summary>
  /// Ensures a default parameterless constructor.
  /// </summary>
  public Config() {}

  internal void LoadPlayerConfigs() {
    // ReSharper disable once SuggestVarOrType_SimpleTypes
    foreach (var entry in this.PlayerEntries) {
      try {
        if (Plugin.Systems.PlayerManager.LoadPlayerConfig(entry.ConfigEntry) is PlayerConfig config) {
          this.PlayerConfigEntries.Add(entry.ConfigEntry, config);
        } else {
          Svc.Log.Warning("Failed to load config for player, {0}, at path {1}", entry.ConfigEntry.DisplayName, entry.ConfigEntry.FileName ?? "null");
        }
      } catch (Exception exception) {
        Svc.Log.Error(exception, "Failed to load config for player, {0}, at path {1}", entry.ConfigEntry.DisplayName, entry.ConfigEntry.FileName ?? "null");
      }
    }
  }

  /// <summary>
  /// Saves the current plugin's config instance.
  /// </summary>
  public static void Save() {
    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    if (Plugin.Systems?.Config is null) {
      return;
    }

    SaveConfig(Plugin.Systems.Config);
  }

  public static void SavePlayerConfigs() {
    // ReSharper disable once SuggestVarOrType_SimpleTypes
    foreach ((PlayerConfigEntry entry, PlayerConfig config) in Plugin.Systems.Config.PlayerConfigEntries) {
      try {
        PlayerManager.SavePlayerConfig(entry, config);
      } catch (Exception exception) {
        Svc.Log.Error(exception, "Failed to save config for player, {0}, at path {1}", entry.DisplayName, entry.FileName ?? "null");
      }
    }
  }
}
