using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using ECommons;
using ECommons.DalamudServices;
using Lumina.Excel.Sheets;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

/// <summary>
/// Class containing am apartment.
/// </summary>
[Serializable]
public class Apartment : WardProperty {
  /// <summary>
  /// The Apartment plot number based off of if the apartment is in a subdistrict.
  /// </summary>
  public override sbyte Plot { get; set; }

  /// <inheritdoc />
  public override bool IsValid => !this.District.Equals(District.Unknown) && (this.Ward > 0) && (this.Room > 0);

  /// <inheritdoc />
  public override void Reset() {
    this.Plot = default;
    this.Room = default;
    this.Enabled = default;
    this.LastVisit = default;
    this.District = default;
    this.Ward = default;
    this.IsDismissed = default;
  }

  [JsonExtensionData]
  [SuppressMessage("Roslynator", "RCS1169")]
  private IDictionary<string, JToken>? _additionalData;

  [OnDeserialized]
  private void OnDeserialized(StreamingContext context)
  {
    if (_additionalData is null) {
#if DEBUG
      Svc.Log.Warning("Apartment _additionalData is null");
#endif
      return;
    }

    if (_additionalData.TryGetValue("Subdistrict", out var subdistrict)
        && subdistrict.NotNull(out JToken subdistrictToken)
        && subdistrictToken.Value<bool>() is bool @bool) {
      this.Plot = (sbyte)(@bool ? -126 : -127);
    }

    if (_additionalData.TryGetValue("ApartmentNumber", out var apartmentNumber)
        && apartmentNumber.NotNull(out JToken apartmentNumberToken)
        && apartmentNumberToken.Value<int>() is int @int) {
      this.Room = (short)@int;
    }
  }
}
