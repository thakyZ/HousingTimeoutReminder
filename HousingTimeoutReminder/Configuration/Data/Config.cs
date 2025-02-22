using Dalamud.Configuration;
using Newtonsoft.Json;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Converters;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Old;

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
  [JsonProperty(ItemConverterType = typeof(PlayerConfigEntryKeyValuePairCollectionJsonConverter))]
  public PlayerConfigEntryKeyValuePairCollection PlayerEntries { get; set; } = [];

#region Obsolete
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(Config)}.{nameof(PlayerEntries)} instead")]
  public List<PerPlayerConfig>? PlayersConfigs { get; set; } = null;

  /// <summary>
  /// Gets or sets the old property for the number of days to wait until notifications show again.
  /// </summary>
  [Versions(introduced: 0, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(GlobalConfig)}.{nameof(GlobalConfig.DaysToWait)} instead")]
  public int? DaysToWait { get; set; } = null;

  /// <summary>
  /// Gets or sets old property for the position of the warning dialog on the screen.
  /// </summary>
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(GlobalConfig)}.{nameof(GlobalConfig.WarningPosition)} instead")]
  public Position? WarningPosition { get; set; } = null;

  /// <summary>
  /// Gets or sets old property for whether to show all players for notifications, otherwise just the currently logged in player.
  /// </summary>
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(GlobalConfig)}.{nameof(GlobalConfig.ShowForAllPlayers)} instead")]
  public bool? ShowAllPlayers { get; set; } = null;

  /// <inheritdoc cref="PerPlayerConfig.FreeCompanyEstate"/>
  [Versions(introduced: 0, removed: 1)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(GlobalConfig)}.{nameof(PlayerConfig.FreeCompanyEstate)} instead")]
  public HousingPlot? FreeCompanyEstate { get; set; } = null;

  /// <inheritdoc cref="PerPlayerConfig.PrivateEstate"/>
  [Versions(introduced: 0, removed: 1)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(GlobalConfig)}.{nameof(PlayerConfig.PrivateEstate)} instead")]
  public HousingPlot? PrivateEstate { get; set; } = null;

  /// <inheritdoc cref="PerPlayerConfig.Apartment"/>
  [Versions(introduced: 0, removed: 1)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(GlobalConfig)}.{nameof(PlayerConfig.Apartment)} instead")]
  public HousingPlot? Apartment { get; set; } = null;
#endregion Obsolete

  /// <summary>
  /// Ensures a default parameterless constructor.
  /// </summary>
  public Config() {}

  /// <summary>
  /// Saves the current plugin's config instance.
  /// </summary>
  public static void Save() {
    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    if (Svc.PluginInterface is null || Plugin.Systems?.Config is null) {
      return;
    }

    Svc.PluginInterface.SavePluginConfig(Plugin.Systems.Config);
  }

  private static void SaveConfig(Config config) {
    if (Svc.PluginInterface is null) {
      return;
    }

    Svc.PluginInterface.SavePluginConfig(config);
  }
}
