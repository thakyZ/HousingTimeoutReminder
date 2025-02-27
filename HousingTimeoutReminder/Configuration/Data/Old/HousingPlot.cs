using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Houses;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

[Serializable]
[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
[Obsolete($"Use {nameof(HouseBase)} instead")]
public class HousingPlot {
  /// <inheritdoc cref="HouseBase.Enabled" />
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(HouseBase)}.{nameof(HouseBase.Enabled)} instead")]
  public bool? Enabled { get; set; } = null;

  /// <inheritdoc cref="HouseBase.LastVisit" />
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(HouseBase)}.{nameof(HouseBase.LastVisit)} instead")]
  public long? LastVisit { get; set; } = null;

  /// <inheritdoc cref="HouseBase.Room" />
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(HouseBase)}.{nameof(HouseBase.Room)} instead")]
  public short? Room { get; set; } = null;

  /// <inheritdoc cref="HouseBase.District" />
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(HouseBase)}.{nameof(HouseBase.District)} instead")]
  public int? District { get; set; } = null;

  /// <inheritdoc cref="HouseBase.Plot" />
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(HouseBase)}.{nameof(HouseBase.Plot)} instead")]
  public sbyte? Plot { get; set; } = null;

  /// <inheritdoc cref="HouseBase.Ward" />
  [Versions(introduced: 1, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(HouseBase)}.{nameof(HouseBase.Ward)} instead")]
  public sbyte? Ward { get; set; } = null;

  /// <summary>
  /// Converts this instance to a <see cref="HouseBase" />.
  /// </summary>
  private HouseBase ToHouseBase() {
    return new InterfaceHouseBaseBaseImpl {
      Enabled = this.Enabled.GetValueOrDefault(),
      LastVisit = this.LastVisit.GetValueOrDefault(),
      District = (District)this.District.GetValueOrDefault(),
      Ward = this.Ward.GetValueOrDefault(),
      Plot = this.Plot.GetValueOrDefault(),
      Room = this.Room.GetValueOrDefault(),
    };
  }

  /// <inheritdoc cref="ToHouseBase" />
  public static implicit operator HouseBase(HousingPlot vector) {
    return vector.ToHouseBase();
  }

  /// <summary>
  /// Converts the provided <see cref="HousingPlot" /> instance to a <see cref="HouseBase" /> instance.
  /// </summary>
  /// <param name="housingPlot">The instance of <see cref="HousingPlot" /> to convert.</param>
  internal static HouseBase ToHouseBaseSafe(HousingPlot? housingPlot) {
    return housingPlot?.ToHouseBase() ?? new InterfaceHouseBaseBaseImpl();
  }

  private class InterfaceHouseBaseBaseImpl : HouseBase;
}
