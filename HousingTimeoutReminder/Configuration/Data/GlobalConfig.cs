namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;

[Serializable]
[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
public class GlobalConfig {
  /// <summary>
  /// Gets or sets the number of days to wait until notifications show again.
  /// </summary>
  [Versions(introduced: 2)]
  public int DaysToWait { get; set; } = 7;

  /// <summary>
  /// Gets or sets whether to show all players for notifications, otherwise just the currently logged in player.
  /// </summary>
  [Versions(introduced: 2)]
  public bool ShowForAllPlayers { get; set; } = false;

  /// <summary>
  /// Gets or sets the position of the warning dialog on the screen.
  /// </summary>
  [Versions(introduced: 2)]
  public Vector2 WarningPosition { get; set; }

  private int? _newPlayerHandling;
  /// <summary>
  /// Gets or sets the method to handle new players.
  /// </summary>
  [Versions(introduced: 2)]
  public int NewPlayerHandling {
    get => _newPlayerHandling ??= (int)NewPlayerHandlingModes.Blank;
    set => _newPlayerHandling = value;
  }

  /// <summary>
  /// Ensures a default parameterless constructor.
  /// </summary>
  public GlobalConfig() {}
}
