namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Config.Data.Houses;

internal class FreeCompanyHouse : IHouse {
  /// <summary>
  /// Whether or not this housing plot is enabled.
  /// </summary>
  public bool Enabled { get; set; }
  /// <summary>
  /// The last visit of the player in Unix epoch timestamp format.
  /// </summary>
  public long LastVisit { get; set; }
  /// <summary>
  /// The district the housing location is in.
  /// </summary>
  public District District { get; set; }
  /// <summary>
  /// The specific ward the property is in.
  /// </summary>
  public sbyte Ward { get; set; }
  /// <summary>
  /// The specific plot the house is on.
  /// </summary>
  public sbyte Plot { get; set; }
  /// <summary>
  /// The room number of the apartment or free company room.
  /// </summary>
  public short Room { get; set; }
}
