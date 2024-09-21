using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using ECommons;
using ECommons.DalamudServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

  [JsonExtensionData]
  [SuppressMessage("Roslynator", "RCS1169")]
  private IDictionary<string, JToken>? _additionalData = null;

  [OnDeserialized]
  private void OnDeserialized(StreamingContext context)
  {
    if (_additionalData is null) {
#if DEBUG
      Svc.Log.Warning("Apartment _additionalData is null");
#endif
      return;
    }

    if (_additionalData.TryGetValue("Subdistrict", out var value) && value.NotNull(out JToken token) && token.Value<bool>() is bool @bool) {
      this.Plot = (sbyte)(@bool ? -126 : -127);
    }

    if (_additionalData.TryGetValue("ApartmentNumber", out var _value) && _value.NotNull(out JToken _token) && _token.Value<int>() is int @int) {
      this.Room = (short)@int;
    }
  }
}
