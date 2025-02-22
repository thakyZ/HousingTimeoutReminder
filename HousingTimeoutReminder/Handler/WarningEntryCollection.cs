using System.Linq;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

public class WarningEntryCollection : CollectionHelper.CollectionBase<WarningEntryKeyValuePair, int, WarningEntry> {
  public new WarningEntry? this[int index] {
    get => ValueGetter(this.FirstOrDefault(x => KeyEqualityComparator(x, index)));
    set => this.AddOrUpdate(index, value);
  }

  /// <inheritdoc />
  protected override bool KeyEqualityComparator(WarningEntryKeyValuePair keyValuePair, int index)
    => keyValuePair.Index.Equals(index);

  /// <inheritdoc />
  protected override WarningEntry? ValueGetter(WarningEntryKeyValuePair? keyValuePair)
    => keyValuePair?.WarningEntry;

  /// <inheritdoc />
  protected override void EntryUpdater(WarningEntryKeyValuePair keyValuePair, WarningEntry? warningEntry) {
    if (warningEntry is not null) {
      keyValuePair.UpdateEntry(warningEntry);
    }
  }

  public void Add(WarningEntry warningEntry) {
    this.Add(this.Count, warningEntry);
  }

  /// <inheritdoc />
  protected override WarningEntryKeyValuePair SourceConstructor(int index, WarningEntry warningEntry)
    => new WarningEntryKeyValuePair(index, warningEntry);

  public void RestoreAllEntries() {
    foreach (int key in this.Keys()) {
      this.RestoreEntry(key);
    }
  }

  public void RestoreEntry(int index) {
    this[index]?.Restore();
  }
}

public class WarningEntryKeyValuePair : CollectionHelper.KeyValuePairBase<int, WarningEntry> {
  /// <summary>
  /// Gets the player id associated with this key value pair
  /// </summary>
  public int Index => this.Key;

  /// <summary>
  /// Gets the player config entry associated with this key value pair.
  /// </summary>
  public WarningEntry WarningEntry => this.Value;

  /// <inheritdoc />
  public WarningEntryKeyValuePair(int index, WarningEntry warningEntry) : base(index, warningEntry) { }
}