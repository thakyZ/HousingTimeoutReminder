using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Config.Data.Houses;

/// <summary>
/// Interface for housing entries.
/// </summary>
public interface IHouse {
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
  /// <summary>
  /// The specific subdistrict the house is in.
  /// </summary>

  [JsonIgnore]
  public byte SubdistrictID => Plot switch { -127 => 1, -126 => 2, _ => 0, };
  /// <summary>
  /// A boolean whether or not this Apartment is in the subdistrict of the ward.
  /// </summary>

  [JsonIgnore]
  public bool IsSubdistrict => SubdistrictID switch { 1 => true, 2 => false, _ => false, };
  /// <summary>
  /// The specific division the house is in.
  /// </summary>
  [JsonIgnore]
  public byte Division => SubdistrictID != 0 ? SubdistrictID : (byte)(Plot > 30 ? 2 : 1);
}
