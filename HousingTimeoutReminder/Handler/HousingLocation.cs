using FFXIVClientStructs.FFXIV.Client.Game;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Houses;
using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

public class HousingLocation : HouseBase, IEquatable<HouseBase> {
  /// <summary>
  /// Gets a <see cref="bool" /> indicating if the player is inside.
  /// If this is not applicable it is null.
  /// </summary>
  public bool IsInside { get; private init; }

  /// <inheritdoc />
  public override long LastVisit {
    get {
      return GetDateTimeNow();
    }
    set {}
  }

  /// <summary>
  /// Convert the <paramref name="territory" /> to the <see cref="District" />
  /// </summary>
  /// <param name="territory">The ID for the territory the player is in.</param>
  /// <return>The district the player is in.</return>
  private static District ConvertToDistrict(ushort territory) {
    return territory switch {
      345 or 346 or 347 or 386 or 424 or 610 => District.Goblet,
      282 or 283 or 284 or 384 or 423 or 608 => District.Mist,
      342 or 343 or 344 or 385 or 425 or 609 => District.LavenderBeds,
      980 or 981 or 982 or 983 or 984 or 999 => District.Empyreum,
      649 or 650 or 651 or 652 or 653 or 655 => District.Shirogane,
      _ => District.Unknown,
    };
  }

  /// <summary>
  /// Gets the current date time in unix seconds
  /// </summary>
  private static long GetDateTimeNow()
    => DateTime.Now.ToUnixTimeSeconds();

  /// <summary>
  /// Gets a <see cref="HousingManager" /> instance for the current location.
  /// </summary>
  /// <param name="territory"></param>
  /// <returns>An instanced <see cref="HousingManager" /> if at a housing plot, otherwise a blank instance.</returns>
  public static HousingLocation GetCurrentLocation(ushort territory) {
    try {
      unsafe {
        HousingManager* manager = HousingManager.Instance();
        return new HousingLocation {
          District = ConvertToDistrict(territory),
          IsInside = manager->IsInside(),
          Room = manager->GetCurrentRoom(),
          Plot = manager->GetCurrentPlot(),
          Ward = manager->GetCurrentWard(),
        };
      }
    } catch (Exception exception) {
      Svc.Log.Error(exception, "Failed to get current housing location.");
    }

    return new HousingLocation {
      District = ConvertToDistrict(territory),
    };
  }

  /// <inheritdoc />
  public override string ToString()
    => JsonConvert.SerializeObject(this, Formatting.None);

  /// <inheritdoc />
  [SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
  public override int GetHashCode()
    => base.GetHashCode();

  /// <summary>
  /// Indicates whether this instance and a specified instance of <see cref="HousingLocation" /> are equal.
  /// </summary>
  /// <param name="other">The <see cref="HousingLocation" /> to compare to.</param>
  /// <returns><see langworld="true" /> if the instances are equal; otherwise <see langworld="false" />.</returns>
  private bool Equals(HousingLocation? other)
    => other is not null
       && this.IsInside.Equals(other.IsInside)
       && this.District.Equals(other.District)
       && this.Room.Equals(other.Room)
       && this.Plot.Equals(other.Plot)
       && this.Ward.Equals(other.Ward);

  /// <inheritdoc />
  public override bool Equals(object? obj)
    => (obj is HouseBase houseBase && this.Equals(other: houseBase))
       || (obj is HousingLocation housingLocation && this.Equals(other: housingLocation));

  /// <summary>
  /// Compares two instances for equality.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator ==(HousingLocation? left, HouseBase? right)
    => left is not null && right is not null && left.LastVisit == right.LastVisit;

  /// <summary>
  /// Compares two instances for inequality.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left does not equal the right; <see langword="false" /> otherwise.</returns>
  public static bool operator !=(HousingLocation? left, HouseBase? right)
    => left is not null && right is not null && !(left == right);

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is greater than the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator >(HousingLocation? left, HouseBase? right)
    => left is not null && right is not null && !(left <= right);

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is greater than or equal to the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator >=(HousingLocation? left, HouseBase? right)
    => left is not null && right is not null && left.LastVisit >= right.LastVisit;

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is less than the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator <(HousingLocation? left, HouseBase? right)
    => left is not null && right is not null && !(left >= right);

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is less than or equal to the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator <=(HousingLocation? left, HouseBase? right)
    => left is not null && right is not null && left.LastVisit <= right.LastVisit;
}
