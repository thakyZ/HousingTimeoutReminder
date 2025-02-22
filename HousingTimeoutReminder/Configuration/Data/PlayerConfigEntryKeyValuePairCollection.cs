namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;

public sealed class PlayerConfigEntryKeyValuePairCollection : CollectionHelper.CollectionBase<PlayerConfigEntryKeyValuePair, ulong, PlayerConfigEntry> {
  /// <inheritdoc />
  protected override bool KeyEqualityComparator(PlayerConfigEntryKeyValuePair keyValuePair, ulong playerID)
    => keyValuePair.PlayerID.Equals(playerID);

  /// <inheritdoc />
  protected override PlayerConfigEntry? ValueGetter(PlayerConfigEntryKeyValuePair? keyValuePair)
    => keyValuePair?.ConfigEntry;

  /// <inheritdoc />
  protected override void EntryUpdater(PlayerConfigEntryKeyValuePair keyValuePair, PlayerConfigEntry? configEntry) {
    if (configEntry is not null) {
      keyValuePair.UpdateEntry(configEntry);
    }
  }

  /// <inheritdoc />
  protected override PlayerConfigEntryKeyValuePair SourceConstructor(ulong playerID, PlayerConfigEntry configEntry)
    => new PlayerConfigEntryKeyValuePair(playerID, configEntry);
}

public class PlayerConfigEntryKeyValuePair : CollectionHelper.KeyValuePairBase<ulong, PlayerConfigEntry> {
  /// <summary>
  /// Gets the player id associated with this key value pair
  /// </summary>
  public ulong PlayerID => this.Key;

  /// <summary>
  /// Gets the player config entry associated with this key value pair.
  /// </summary>
  public PlayerConfigEntry ConfigEntry => this.Value;

  public bool IsCurrentPlayer => Systems.IsLoggedIn && this.PlayerID == Systems.CurrentPlayerID;

  /// <inheritdoc />
  public PlayerConfigEntryKeyValuePair(ulong playerID, PlayerConfigEntry configEntry) : base(playerID, configEntry) { }
}
