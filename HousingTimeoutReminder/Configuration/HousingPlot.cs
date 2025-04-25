using System;
using ECommons.DalamudServices;
using ECommons;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
/// <summary>
/// Class containing a generic housing plot.
/// </summary>
[Serializable]
public class HousingPlot : WardProperty {
  /// <inheritdoc />
  public override short Room {
    get => 0;
    set => _ = value;
  }

  /// <inheritdoc />
  public override bool IsValid => !this.District.Equals(District.Unknown) && (this.Ward > 0) && (this.Plot > 0);

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
      Svc.Log.Warning("HousingPlot _additionalData is null");
#endif
      return;
    }
  }
}
