using System;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
/// <summary>
/// Class containing a generic housing plot.
/// </summary>
[Serializable]
public class HousingPlot : IWardProperty {
  /// <inheritdoc />
  public bool Enabled { get; set; }

  /// <inheritdoc />
  public long LastVisit { get; set; }

  /// <inheritdoc />
  public District District { get; set; } = District.Unknown;

  /// <inheritdoc />
  public sbyte Plot { get; set; }

  /// <inheritdoc />
  public sbyte Ward { get; set; }

  /// <inheritdoc />
  public short Room { get => 0; set { _ = value; } }

  /// <inheritdoc />
  public bool IsValid() { return !District.Equals(District.Unknown) && (Ward > 0) && (Plot > 0); }

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
