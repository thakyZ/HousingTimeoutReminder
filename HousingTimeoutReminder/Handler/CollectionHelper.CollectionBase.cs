using System.Linq;
using ECommons;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

public abstract partial class CollectionHelper {
  public abstract class CollectionBase<TSource, TKey, TValue> : List<TSource> where TSource : KeyValuePairBase<TKey, TValue> where TKey : notnull where TValue : notnull {
    public new TValue? this[int index] {
      get => ValueGetter(this.FirstOrDefault(x => KeyEqualityComparator(x, this.Keys()[index])));
      set => this.AddOrUpdate(this.Keys()[index], value);
    }

    public TValue? this[TKey key] {
      get => ValueGetter(this.FirstOrDefault(x => KeyEqualityComparator(x, key)));
      set => this.AddOrUpdate(key, value);
    }

    protected abstract bool KeyEqualityComparator(TSource source, TKey key);

    protected abstract TValue? ValueGetter(TSource? source);

    protected abstract void EntryUpdater(TSource source, TValue? value);

    protected abstract TSource SourceConstructor(TKey key, TValue value);

    public TKey[] Keys() {
      return [..this.Select(x => x.Key)];
    }

    public TValue[] Values() {
      return [..this.Select(x => x.Value)];
    }

    /// <summary>
    /// Adds an entry to the collection, otherwise sets the existing entry.
    /// </summary>
    /// <param name="key">The key to add an entry of.</param>
    /// <param name="value">The entry to add.</param>
    /// <exception cref="Exception">Throws if provided parameter <paramref name="value" /> is null.</exception>
    protected void AddOrUpdate(TKey key, TValue? value) {
      if (value is null) {
        throw new Exception("Provided value is null.");
      }

      if (this.Any(x => KeyEqualityComparator(x, key))) {
        this.Where(x => KeyEqualityComparator(x, key)).Each(x => EntryUpdater(x, value));
      }

      this.Add(SourceConstructor(key, value));
    }

    /// <summary>
    /// Adds an entry to the collection.
    /// </summary>
    /// <param name="key">The key to add an entry of.</param>
    /// <param name="value">The entry to add.</param>
    /// <exception cref="Exception">
    /// Throws if the key is already in the collection.
    /// <remarks>If you don't want an exception use <see cref="CollectionBase{TSource,TKey,TValue}.TryAdd" />.</remarks>
    /// </exception>
    public void Add(TKey key, TValue value) {
      if (this.Any(x => KeyEqualityComparator(x, key))) {
        throw new Exception($"Entry already exists with value, {key}");
      }

      this.Add(SourceConstructor(key, value));
    }

    /// <summary>
    /// Adds an entry to the collection.
    /// </summary>
    /// <param name="key">The key to add an entry of.</param>
    /// <param name="value">The entry to add.</param>
    internal void AddUnsafe(TKey key, TValue value) {
      this.Add(SourceConstructor(key, value));
    }

    /// <summary>
    /// Adds an entry to the collection.
    /// </summary>
    /// <param name="key">The key to add an entry of.</param>
    /// <param name="value">The entry to add.</param>
    /// <returns><see langword="true" /> if successfully added; otherwise <see langword="false" />.</returns>
    public bool TryAdd(TKey key, TValue value) {
      try {
        this.Add(key, value);
        return true;
      } catch {
        // Do nothing...
      }
      return false;
    }

    /// <summary>
    /// Gets the <see cref="TValue" /> from the provided key in this collection.
    /// </summary>
    /// <param name="key">The key to get an entry of.</param>
    /// <param name="result">The resulting <see cref="TValue" /> entry.</param>
    /// <returns><see langworld="true" /> if the value was properly found; otherwise <see langword="false" />.</returns>
    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? result) {
      if (this[key] is TValue entry) {
        result = entry;
        return true;
      }

      result = default;
      return false;
    }

    /// <summary>
    /// Gets the key pair value in this collection.
    /// </summary>
    /// <param name="key">The key to get an entry of.</param>
    /// <param name="throw">Determines if this method should throw an exception when the entry is not found.</param>
    /// <returns><see langworld="true" /> if the value was properly found; otherwise <see langword="false" />.</returns>
    /// <exception cref="KeyNotFoundException">
    /// If <paramref name="throw"/> is <see langword="true" /> will throw this exception when the entry is not found.
    /// </exception>
    public TSource? GetKeyValuePair(TKey key, [DoesNotReturnIf(true)] bool @throw = false) {
      // ReSharper disable once InvertIf
      if (!this.Any(x => KeyEqualityComparator(x, key))) {
        if (@throw) {
          throw new KeyNotFoundException($"Failed to find a key of {key}.");
        }

        return default;
      }

      return this.First(x => KeyEqualityComparator(x, key));

    }

    /// <summary>
    /// Gets the key pair value in this collection.
    /// </summary>
    /// <param name="key">The key to get an entry of.</param>
    /// <param name="result">The resulting <see cref="TSource" /> entry.</param>
    /// <returns><see langworld="true" /> if the value was properly found; otherwise <see langword="false" />.</returns>
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
    public bool TryGetKeyValuePair(TKey key, [NotNullWhen(true)] out TSource? result) {
      try {
        result = this.GetKeyValuePair(key, true);
        return result is not null;
      } catch {
        // Do nothing...
      }

      result = default;
      return false;
    }
  }
}