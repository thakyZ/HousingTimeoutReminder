using System;

using CSHousingManager = FFXIVClientStructs.FFXIV.Client.Game.HousingManager;

using ECommons.DalamudServices;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

public sealed class HousingManager: IEquatable<HousingManager> {

  public bool? IsInside { get; }
  public bool IsApartment { get; }
  public int ApartmentWing {
    get {
      if (!IsApartment) {
        return 0;
      }

      if (Plot == -127) {
        return 1;
      }

      if (Plot == -126) {
        return 2;
      }

      return 0;
    }
  }
  public short Room { get; }
  public sbyte Plot { get; }
  public sbyte Ward { get; }
  public byte Division { get; }
  public District District { get; }

  private HousingManager(bool isApartment, short room, sbyte plot, sbyte ward, byte division, ushort district, bool? isInside = null) {
    this.IsInside = isInside;
    this.IsApartment = isApartment;
    this.Room = room;
    this.Plot = plot;
    this.Ward = ward;
    this.Division = division;
    this.District = ConvertToDistrict(district);
  }

  private HousingManager(bool isApartment, short room, sbyte plot, sbyte ward, byte division, District district, bool? isInside = null) {
    this.IsInside = isInside;
    this.IsApartment = isApartment;
    this.Room = room;
    this.Plot = plot;
    this.Ward = ward;
    this.Division = division;
    this.District = district;
  }

  /// <summary>
  /// Convert the <paramref name="territory"/> to the <see cref="District"/>
  /// </summary>
  /// <param name="territory">The ID for the territory the player is in.</param>
  /// <return>The district the player is in.</return>
  public static District ConvertToDistrict(ushort territory) {
    return territory switch {
      345 or 346 or 347 or 386 or 424 or 610 => District.Goblet,
      282 or 283 or 284 or 384 or 423 or 608 => District.Mist,
      342 or 343 or 344 or 385 or 425 or 609 => District.LavenderBeds,
      980 or 981 or 982 or 983 or 984 or 999 => District.Empyreum,
      649 or 650 or 651 or 652 or 653 or 655 => District.Shirogane,
      _ => District.Unknown,
    };
  }

  public static HousingManager GetCurrentLocation(ushort territory) {
    try {
      unsafe {
        var manager = CSHousingManager.Instance();
        var isInside = manager->IsInside();
        short room = manager->GetCurrentRoom();
        sbyte plot = manager->GetCurrentPlot();
        var isApartment = isInside && plot <= -127;
        sbyte ward = manager->GetCurrentWard();
        byte division = manager->GetCurrentDivision();
        return new HousingManager(isApartment, room, (sbyte)(plot + 1), (sbyte)(ward + 1), division, territory, isInside);
      }
    } catch (Exception exception) {
      Svc.Log.Error(exception, "Failed to get current housing location.");
    }
    return Blank;
  }

  public static HousingManager From(PerPlayerConfig playerConfig, HousingType type) {
    if (type == HousingType.FreeCompanyEstate && playerConfig.FreeCompanyEstate is IWardProperty fcHousingPlot) {
      return new HousingManager(false, 0, fcHousingPlot.Plot, fcHousingPlot.Ward, fcHousingPlot.Division, fcHousingPlot.District);
    }

    if (type == HousingType.PrivateEstate && playerConfig.PrivateEstate is IWardProperty peHousingPlot) {
      return new HousingManager(false, 0, peHousingPlot.Plot, peHousingPlot.Ward, peHousingPlot.Division, peHousingPlot.District);
    }

    if (type == HousingType.Apartment && playerConfig.Apartment is IWardProperty apartment) {
      return new HousingManager(true, apartment.Room, apartment.Plot, apartment.Ward, apartment.Division, apartment.District);
    }

    return Blank;
  }

  public bool Equals(HousingManager? otherManager) {
      return this.IsApartment == otherManager?.IsApartment &&
        this.ApartmentWing == otherManager.ApartmentWing &&
        this.Room == otherManager.Room &&
        this.Plot == otherManager.Plot &&
        this.Ward == otherManager.Ward &&
        this.Division == otherManager.Division &&
        (int)this.District == (int)otherManager.District;
  }

  public override bool Equals(object? obj) {
    if (obj is HousingManager otherManager) {
      return Equals(otherManager);
    }

    return false;
  }

  public override int GetHashCode() {
    return HashCode.Combine(
      this.IsApartment ? 1 : 0, this.ApartmentWing,
      (int)this.Room, (int)this.Plot, (int)this.Ward,
      (int)this.Division, (int)this.District
    );
  }

  private static HousingManager Blank => new(false, 0, 0, 0, 0, 1);
}
