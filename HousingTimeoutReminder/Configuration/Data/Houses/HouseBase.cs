using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Houses;

/// <summary>
/// Base class for housing entries.
/// </summary>
[Serializable]
[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
public abstract class HouseBase {
  /// <summary>
  /// Gets or sets whether this housing plot is enabled.
  /// </summary>
  [Versions(introduced: 3)]
  public bool Enabled { get; set; } = false;

  /// <summary>
  /// Gets or sets the last visit of the player in Unix epoch timestamp format.
  /// </summary>
  [Versions(introduced: 3)]
  public virtual long LastVisit { get; set; } = 0;

  /// <summary>
  /// Gets or sets the district the housing location is in.
  /// </summary>
  [Versions(introduced: 3)]
  public District District { get; set; } = District.Unknown;

  /// <summary>
  /// Gets or sets the specific ward the property is in.
  /// </summary>
  [Versions(introduced: 3)]
  public sbyte Ward { get; set; } = 1;

  /// <summary>
  /// Gets or sets the specific plot the house is on.
  /// </summary>
  [Versions(introduced: 3)]
  public sbyte Plot { get; set; } = 1;

  /// <summary>
  /// Gets or sets the room number of the apartment or free company room.
  /// </summary>
  [Versions(introduced: 3)]
  public short Room { get; set; } = 1;

  /// <summary>
  /// The max number of wards.
  /// TODO: Update ward counts when necessary.
  /// </summary>
  public const int WARD_MAX = 32;

  /// <summary>
  /// The max number of plots.
  /// </summary>
  public const int PLOT_MAX = 60;

  /// <summary>
  /// The max number of apartments.
  /// TODO: Update apartment counts when necessary.
  /// </summary>
  public const int APARTMENT_MAX = 90;

  /// <summary>
  /// Gets the specific sub-district the house is in.
  /// </summary>
  [JsonIgnore]
  public byte SubDistrictID => Plot switch { -127 => 1, -126 => 2, _ => 0 };

#if DEBUG
  /// <summary>
  /// Gets the specific apartment wing the apartment is in.
  /// </summary>
  [JsonIgnore]
  [Obsolete($"Please use {nameof(SubDistrictID)} instead.")]
  public int ApartmentWing => this.SubDistrictID;
#endif

  /// <summary>
  /// Gets a <see cref="bool" /> indicating whether this apartment is in the sub-district of the ward.
  /// </summary>
  [JsonIgnore]
  public bool IsSubDistrict => this.SubDistrictID switch { 1 => true, 2 => false, _ => false };

  /// <summary>
  /// Gets a <see cref="bool" /> indicating whether this housing plot is an apartment.
  /// </summary>
  [JsonIgnore]
  public bool IsApartment => this.SubDistrictID != 0;

  /// <summary>
  /// Gets the specific division the house is in.
  /// </summary>
  [JsonIgnore]
  public byte Division => this.IsApartment ? this.SubDistrictID : (byte)(Plot > 30 ? 2 : 1);

  public int GetDaysMissed() => (DateTimeOffset.FromUnixTimeSeconds(this.LastVisit) - DateTime.Now).Days;

  public DateTimeOffset GetLastVisit() {
    return DateTimeOffset.FromUnixTimeSeconds(this.LastVisit);
  }

  public DateTimeOffset GetNextVisit() {
    return DateTimeOffset.FromUnixTimeSeconds(this.LastVisit).AddDays(Plugin.Systems.Config.Global.DaysToWait);
  }

  public bool IsValid => this.Room is > 0 and <= APARTMENT_MAX && this.Plot is > 0 and <= PLOT_MAX && this.Ward is > 0 and <= WARD_MAX && this.District != District.Unknown;

#region Comparison Operators
  /// <summary>
  /// Indicates whether this instance and a specified instance of <see cref="HouseBase" /> are equal.
  /// </summary>
  /// <param name="other">The <see cref="HouseBase" /> to compare to.</param>
  /// <returns><see langworld="true" /> if the instances are equal; otherwise <see langworld="false" />.</returns>
  public bool Equals(HouseBase? other)
    => other is not null
       && this.District.Equals(other.District)
       && this.Room.Equals(other.Room)
       && this.Plot.Equals(other.Plot)
       && this.Ward.Equals(other.Ward);

  /// <inheritdoc />
  public override bool Equals(object? obj)
    => obj is HouseBase houseBase && this.Equals(other: houseBase);

  /// <inheritdoc />
  [SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
  public override int GetHashCode()
    => base.GetHashCode();

#region Self Comparison
  /// <summary>
  /// Compares two instances for equality.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator ==(HouseBase? left, HouseBase? right)
    => left is not null && right is not null && left.LastVisit == right.LastVisit;

  /// <summary>
  /// Compares two instances for inequality.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left does not equal the right; <see langword="false" /> otherwise.</returns>
  public static bool operator !=(HouseBase? left, HouseBase? right)
    => left is not null && right is not null && !(left == right);
#endregion Self Comparison
#region Int64 Comparison
  /// <summary>
  /// Compares two instances for equality.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator ==(HouseBase? left, long right)
    => left is not null && left.LastVisit == right;

  /// <summary>
  /// Compares two instances for equality.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator !=(HouseBase? left, long right)
    => left is not null && !(left == right);

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is greater than the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator >(HouseBase? left, long right)
    => left is not null && !(left <= right);

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is greater than or equal to the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator >=(HouseBase? left, long right)
    => left is not null && left.LastVisit >= right;

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is less than the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator <(HouseBase? left, long right)
    => left is not null && !(left >= right);

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is less than or equal to the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator <=(HouseBase? left, long right)
    => left is not null && left.LastVisit <= right;
#endregion Int64 Comparison
#region DateTime Comparison
  /// <summary>
  /// Compares two instances for equality.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator ==(HouseBase? left, DateTime right)
    => left is not null && left == right.ToUnixTimeSeconds();

  /// <summary>
  /// Compares two instances for equality.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator !=(HouseBase? left, DateTime right)
    => left is not null && left != right.ToUnixTimeSeconds();

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is greater than the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator >(HouseBase? left, DateTime right)
    => left is not null && left <= right.ToUnixTimeSeconds();

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is greater than or equal to the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator >=(HouseBase? left, DateTime right)
    => left is not null && left >= right.ToUnixTimeSeconds();

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is less than the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator <(HouseBase? left, DateTime right)
    => left is not null && left < right.ToUnixTimeSeconds();

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is less than or equal to the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator <=(HouseBase? left, DateTime right)
    => left is not null && left <= right.ToUnixTimeSeconds();
#endregion DateTime Comparison
#region Housing Location Comparison
  /// <summary>
  /// Compares two instances for equality.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator ==(HouseBase? left, HousingLocation? right)
    => left is not null && right is not null && left.LastVisit == right.LastVisit;

  /// <summary>
  /// Compares two instances for inequality.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left does not equal the right; <see langword="false" /> otherwise.</returns>
  public static bool operator !=(HouseBase? left, HousingLocation? right)
    => left is not null && right is not null && !(left == right);

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is greater than the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator >(HouseBase? left, HousingLocation? right)
    => left is not null && right is not null && !(left <= right);

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is greater than or equal to the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator >=(HouseBase? left, HousingLocation? right)
    => left is not null && right is not null && left.LastVisit >= right.LastVisit;

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is less than the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator <(HouseBase? left, HousingLocation? right)
    => left is not null && right is not null && !(left >= right);

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is less than or equal to the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator <=(HouseBase? left, HousingLocation? right)
    => left is not null && right is not null && left.LastVisit <= right.LastVisit;
#endregion Housing Location Comparison
#endregion Comparison Operators
}
