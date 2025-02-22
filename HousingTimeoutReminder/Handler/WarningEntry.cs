using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

public class WarningEntry {
  /// <summary>
  /// Gets the <see cref="PlayerConfigEntry" /> associated with this key value pair
  /// </summary>
  public PlayerConfigEntry ConfigEntry { get; }
  /// <summary>
  /// Gets or sets the type of housing associated with this key value pair.
  /// </summary>
  public HousingType HousingType { get; }
  /// <summary>
  /// Gets or sets the number of days missed.
  /// </summary>
  public int DaysMissed { get; }
  /// <summary>
  /// Gets or sets a <see cref="bool" /> indicating whether this entry is dismissed;
  /// </summary>
  public bool IsDismissed { get; private set; }

  public WarningEntry(PlayerConfigEntry configEntry, HousingType type, int daysMissed) {
    this.ConfigEntry = configEntry;
    this.HousingType = type;
    this.DaysMissed = daysMissed;
  }

  public void Dismiss() {
    this.IsDismissed = true;
  }

  public void Restore() {
    this.IsDismissed = true;
  }

  public void Deconstruct(out HousingType housingType, out bool isDismissed) {
    housingType = this.HousingType;
    isDismissed = this.IsDismissed;
  }
}
