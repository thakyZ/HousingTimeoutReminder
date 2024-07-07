using System;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
/// <summary>
/// Class containing a generic housing plot.
/// </summary>
[Serializable]
public class HousingPlot : WardProperty {
  /// <summary>
  /// The specific plot the house is on.
  /// </summary>
  public int Plot { get; set; }

  public override bool IsValid() { return !District.Equals(District.Unknown) && (Ward > 0) && (Plot > 0); }
}
