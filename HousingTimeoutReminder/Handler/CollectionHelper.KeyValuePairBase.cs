using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Converters;
using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

public abstract partial class CollectionHelper {
  public abstract class KeyValuePairBase<TKey, TValue> : IEquatable<KeyValuePairBase<TKey, TValue>> where TKey : notnull where TValue : notnull {
    /// <summary>
    /// Gets the player id associated with this key value pair
    /// </summary>
    public TKey Key { get; }

    /// <summary>
    /// Gets the player config entry associated with this key value pair.
    /// </summary>
    public TValue Value { get; protected set; }

    protected KeyValuePairBase(TKey key, TValue value) {
      this.Key = key;
      this.Value = value;
    }

    internal void UpdateEntry(TValue entry) {
      this.Value = entry;
    }

    public void Deconstruct(out TKey key, out TValue value) {
      key = this.Key;
      value = this.Value;
    }

    /// <summary>
    /// Indicates whether this instance and a specified instance of nullable <see cref="KeyValuePairBase{TKey,TValue}" /> are equal.
    /// </summary>
    /// <param name="other">The <see cref="KeyValuePairBase{TKey,TValue}" /> to compare to.</param>
    /// <returns><see langworld="true" /> if the instances are equal; otherwise <see langworld="false" />.</returns>
    public bool Equals([NotNullWhen(true)] KeyValuePairBase<TKey, TValue>? other)
      => other is not null
         && this.Key.Equals(other.Key)
         && this.Value.Equals(other.Value);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns><see langworld="true" /> if the instances are equal; otherwise <see langworld="false" />.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
      => obj is KeyValuePairBase<TKey, TValue> other && this.Equals(other: other);

    /// <summary>
    /// Returns the hashcode for this instance.
    /// </summary>
    /// <returns>A <see langworld="int" /> containing the unique hashcode for this instance.</returns>
    public override int GetHashCode()
      => HashCode.Combine(this.Key);

    /// <summary>
    /// Returns a Json <see langworld="string" /> that represents the current <see langworld="KeyValuePairBase" />.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
      => JsonConvert.SerializeObject(this, Formatting.None, new KeyValuePairBaseJsonConverter<TKey, TValue>());

    /// <summary>
    /// Compares two instances for equality.
    /// </summary>
    /// <param name="left">The first instance.</param>
    /// <param name="right">The second instance.</param>
    /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
    public static bool operator ==(KeyValuePairBase<TKey, TValue> left, KeyValuePairBase<TKey, TValue> right)
      => left.Equals(right);

    /// <summary>
    /// Compares two instances for inequality.
    /// </summary>
    /// <param name="left">The first instance.</param>
    /// <param name="right">The second instance.</param>
    /// <returns><see langword="true" />, if left does not equal the right; <see langword="false" /> otherwise.</returns>
    public static bool operator !=(KeyValuePairBase<TKey, TValue> left, KeyValuePairBase<TKey, TValue> right)
      => !(left == right);
  }
}