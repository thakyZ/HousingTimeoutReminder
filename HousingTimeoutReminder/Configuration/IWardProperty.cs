using ECommons.DalamudServices;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

/// <summary>
/// Interface for housing Properties.
/// </summary>
public interface IWardProperty {
  /// <summary>
  /// Whether or not this housing plot is enabled.
  /// </summary>
  public bool Enabled { get; set; }

  /// <summary>
  /// The last visit of the player in Unix epoch timestamp format.
  /// </summary>
  public long LastVisit { get; set; }

  /// <summary>
  /// The room number of the apartment or free company room.
  /// </summary>
  public short Room { get; set; }

  /// <summary>
  /// The district the housing location is in.
  /// </summary>
  public District District { get; set; }

  /// <summary>
  /// The specific plot the house is on.
  /// </summary>
  public sbyte Plot { get; set; }

  /// <summary>
  /// A boolean whether or not this Apartment is in the subdistrict of the ward.
  /// </summary>
  [JsonIgnore]
  public bool IsSubdistrict => SubdistrictID switch { 1 => false, 2 => true, _ => false, };

  /// <summary>
  /// The specific subdistrict the house is in.
  /// </summary>
  [JsonIgnore]
  public byte SubdistrictID => Plot switch { -127 => 1, -126 => 2, _ => 0, };

  /// <summary>
  /// The specific division the house is in.
  /// </summary>
  [JsonIgnore]
  public byte Division => SubdistrictID != 0 ? SubdistrictID : (byte)(Plot > 30 ? 2 : 1);

  /// <summary>
  /// The specific ward the property is in.
  /// </summary>
  public sbyte Ward { get; set; }

  /// <summary>
  /// Checks if this instance of IWardProperty is valid.
  /// </summary>
  /// <returns>Returns true if valid, false otherwise.</returns>
  public bool IsValid { get; }

  /// <summary>
  /// Resets the instance of an <see href="IWardProperty" />
  /// </summary>
  /// <returns>Returns true if valid, false otherwise.</returns>
  public void Reset();
}
