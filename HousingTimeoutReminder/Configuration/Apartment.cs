using System;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
/// <summary>
/// Class containing am apartment.
/// </summary>
[Serializable]
public class Apartment : WardProperty {
  /// <summary>
  /// A boolean whether or not this Apartment is in the subdistrict of the ward.
  /// </summary>
  public bool Subdistrict { get; set; }

  /// <summary>
  /// The Apartment number the player owns.
  /// </summary>
  public ushort ApartmentNumber { get; set; }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <returns></returns>
  public override bool IsValid() { return !District.Equals(District.Unknown) && (Ward > 0) && (ApartmentNumber > 0); }
}
