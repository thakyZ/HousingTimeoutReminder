using System;
using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

/// <summary>
/// Class containing am apartment.
/// </summary>
[Serializable]
public class Apartment : IWardProperty {

  /// <inheritdoc />
  public short Room { get; set; }

  /// <summary>
  /// The Apartment plot number based off of if the apartment is in a subdistrict.
  /// </summary>
  public sbyte Plot { get; set; }

  /// <inheritdoc />
  public bool Enabled { get; set; }

  /// <inheritdoc />
  public long LastVisit { get; set; }

  /// <inheritdoc />
  public District District { get; set; }

  /// <inheritdoc />
  public sbyte Ward { get; set; }

  /// <inheritdoc />
  public bool IsValid() { return !District.Equals(District.Unknown) && (Ward > 0) && (Room > 0); }

  /// <inheritdoc />
  public void Reset() {
    this.Plot = default;
    this.Room = default;
    this.Enabled = default;
    this.LastVisit = default;
    this.District = default;
    this.Ward = default;
  }
}
