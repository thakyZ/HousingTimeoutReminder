using System;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder {
  /// <summary>
  /// Class containing am apartment.
  /// </summary>
  [Serializable]
  public class Apartment : WardProperty {
    /// <summary>
    /// A boolean whether or not this appartment is in the subdistrict of the ward.
    /// </summary>
    public bool Subdistrict { get; set; }
    /// <summary>
    /// The appartment number the player owns.
    /// </summary>
    public ushort ApartmentNumber { get; set; }
    public override bool IsValid() { return !District.Equals(District.Unknown) && (Ward > 0) && (ApartmentNumber > 0); }
  }
}
