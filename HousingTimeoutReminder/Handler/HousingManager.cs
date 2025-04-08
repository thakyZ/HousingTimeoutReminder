using System;

using CSHousingManager = FFXIVClientStructs.FFXIV.Client.Game.HousingManager;

using ECommons.DalamudServices;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

public sealed class HousingManager: IEquatable<HousingManager> {
  /// <summary>
  /// A boolean determining if the player is inside.
  /// If this is not applicable it is null.
  /// </summary>
  public bool? IsInside { get; }
  /// <summary>
  /// A boolean determining if the house is an apartment.
  /// </summary>
  public bool IsApartment => ApartmentWing != 0;
  /// <summary>
  /// The apartment wing. (or division)
  /// Determines the division the house is in but only if it is an apartment.
  /// </summary>
  public int ApartmentWing => Plot switch { -126 => 1, -127 => 2, _ => 0, };
  /// <summary>
  /// The room of the apartment.
  /// </summary>
  public short Room { get; }
  /// <summary>
  /// The plot the house is in.
  /// </summary>
  public sbyte Plot { get; }
  /// <summary>
  /// The ward the house is in.
  /// </summary>
  public sbyte Ward { get; }
  /// <summary>
  /// The division of the ward the house is in.
  /// </summary>
  public int Division => ApartmentWing != 0 ? ApartmentWing : (Plot > 30 ? 2 : 1);
  /// <summary>
  /// The housing district of the house.
  /// </summary>
  public District District { get; }

  /// <summary>
  /// The default constructor.
  /// </summary>
  /// <param name="plot">
  ///   <para>The plot of the house.</para>
  ///   <para>Is -176 or -177 if is an apartment.</para>
  /// </param>
  /// <param name="ward">The ward the house is in.</param>
  /// <param name="district">A variable representing the housing district as an <see cref="District"/> enum.</param>
  /// <param name="room">
  ///   <para>The room of the apartment complex.</para>
  ///   <para>Is 0 if is not an apartment.</para>
  /// </param>
  /// <param name="isInside">Optional argument if the player is inside the house.</param>
  private HousingManager(sbyte plot, sbyte ward, District district, short room = 0, bool? isInside = null) {
    this.IsInside = isInside;
    this.Room = room;
    this.Plot = plot;
    this.Ward = ward;
    this.District = district;
  }

  /// <summary>
  /// The default constructor but with primitives.
  /// </summary>
  /// <param name="plot">
  ///   <para>The plot of the house.</para>
  ///   <para>Is -176 or -177 if is an apartment.</para>
  /// </param>
  /// <param name="ward">The ward the house is in.</param>
  /// <param name="district">A variable representing the housing district as an ushort.</param>
  /// <param name="room">
  ///   <para>The room of the apartment complex.</para>
  ///   <para>Is 0 if is not an apartment.</para>
  /// </param>
  /// <param name="isInside">Optional argument if the player is inside the house.</param>
  private HousingManager(sbyte plot, sbyte ward, ushort district, short room = 0, bool? isInside = null)
    : this(plot, ward,ConvertToDistrict(district), room, isInside) { }

  /// <summary>
  /// The default constructor.
  /// </summary>
  private HousingManager() : this(0, 0, 0u, 0, null) { }

  /// <summary>
  /// Convert the <paramref name="territory" /> to the <see cref="District" />
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

  /// <summary>
  /// Gets a <see cref="HousingManager" /> instance for the current location.
  /// </summary>
  /// <param name="territory"></param>
  /// <returns>An instanced <see cref="HousingManager" /> if at a housing plot, otherwise a <see cref="Blank" /> instance.</returns>
  public static HousingManager? GetCurrentLocation(ushort territory) {
    try {
      unsafe {
        var manager = CSHousingManager.Instance();
        var isInside = manager->IsInside();
        short room = manager->GetCurrentRoom();
        sbyte plot = manager->GetCurrentPlot();
        sbyte ward = manager->GetCurrentWard();
        return new HousingManager((sbyte)(plot + 1), (sbyte)(ward + 1), territory, room, isInside);
      }
    } catch (Exception exception) {
      Svc.Log.Error(exception, "Failed to get current housing location.");
    }

    return null;
  }

  /// <summary>
  /// Creates a new instance of a <see cref="HousingManager" /> from a two parameters.
  /// </summary>
  /// <param name="playerID">The instance of a player id to fetch the player config.</param>
  /// <param name="type">The type of house to manage.</param>
  /// <returns>A new instance of a <see cref="HousingManager" />.</returns>
  public static HousingManager? From(PlayerID playerID, HousingType type) {
    var playerConfig = Config.GetPlayerConfig(playerID);

    return type switch {
      HousingType.FreeCompanyEstate when playerConfig.FreeCompanyEstate is IWardProperty fcHousingPlot =>
        new HousingManager(fcHousingPlot.Plot, fcHousingPlot.Ward, fcHousingPlot.District),
      HousingType.PrivateEstate when playerConfig.PrivateEstate is IWardProperty peHousingPlot => new HousingManager(
        peHousingPlot.Plot, peHousingPlot.Ward, peHousingPlot.District),
      HousingType.Apartment when playerConfig.Apartment is IWardProperty apartment => new HousingManager(apartment.Plot,
        apartment.Ward, apartment.District, apartment.Room),
      _ => null
    };
  }

  /// <summary>
  /// Compares this instance of a <see cref="HousingManager"/> with another or null.
  /// </summary>
  /// <param name="otherManager">The other <see cref="HousingManager"/> or null</param>
  /// <returns><see langword="true"/> if they are the same and not null, otherwise <see langword="false"/></returns>
  public bool Equals(HousingManager? otherManager) {
      return this.Room == otherManager?.Room &&
        this.Plot == otherManager.Plot &&
        this.Ward == otherManager.Ward &&
        this.Division == otherManager.Division &&
        (int)this.District == (int)otherManager.District;
  }

  /// <inheritdoc/>
  public override bool Equals(object? obj) {
    if (obj is HousingManager otherManager) {
      return Equals(otherManager);
    }

    return false;
  }

  /// <inheritdoc/>
  public override int GetHashCode() {
    return HashCode.Combine((int)this.Room, (int)this.Plot, (int)this.Ward, (int)this.District);
  }
}
