namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;

public sealed class PlayerConfigKeyValuePairCollection : CollectionHelper.CollectionBase<PlayerConfigKeyValuePair, PlayerConfigEntry, PlayerConfig> {
  /// <inheritdoc />
  protected override bool KeyEqualityComparator(PlayerConfigKeyValuePair keyValuePair, PlayerConfigEntry configEntry)
    => keyValuePair.ConfigEntry.Equals(configEntry);

  /// <inheritdoc />
  protected override PlayerConfig? ValueGetter(PlayerConfigKeyValuePair? keyValuePair)
    => keyValuePair?.Config;

  /// <inheritdoc />
  protected override void EntryUpdater(PlayerConfigKeyValuePair keyValuePair, PlayerConfig? config) {
    if (config is not null) {
      keyValuePair.UpdateEntry(config);
    }
  }

  /// <inheritdoc />
  protected override PlayerConfigKeyValuePair SourceConstructor(PlayerConfigEntry configEntry, PlayerConfig config)
    => new PlayerConfigKeyValuePair(configEntry, config);
}

public class PlayerConfigKeyValuePair : CollectionHelper.KeyValuePairBase<PlayerConfigEntry, PlayerConfig> {
  /// <summary>
  /// Gets the player id associated with this key value pair
  /// </summary>
  public PlayerConfigEntry ConfigEntry => this.Key;

  /// <summary>
  /// Gets the player config entry associated with this key value pair.
  /// </summary>
  public PlayerConfig Config => this.Value;

  public bool IsCurrentPlayer => Systems.IsLoggedIn && this.ConfigEntry.Equals(Plugin.Systems.Config.PlayerEntries[Systems.CurrentPlayerID.Value]);

  /// <inheritdoc />
  public PlayerConfigKeyValuePair(PlayerConfigEntry configEntry, PlayerConfig config) : base(configEntry, config) { }
}
