using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

/// <summary>
/// An implementation of the old player identification class as an interface.
/// </summary>
[Serializable]
[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
[Obsolete($"Use {nameof(PlayerConfigEntry)}.{nameof(PlayerConfigEntry.Name)} instead")]
public class PlayerID {
  /// <summary>
  /// The first name of the player character.
  /// </summary>
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(PlayerConfigEntry)}.{nameof(PlayerConfigEntry.Name)} instead")]
  public string? FirstName { get; set; } = null;

  /// <summary>
  /// The last name of the player character.
  /// </summary>
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(PlayerConfigEntry)}.{nameof(PlayerConfigEntry.Name)} instead")]
  public string? LastName { get; set; } = null;

  /// <summary>
  /// The home world ID of the player character
  /// </summary>
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(PlayerConfigEntry)}.{nameof(PlayerConfigEntry.World)} instead")]
  public uint? HomeWorld { get; set; } = null;
}
