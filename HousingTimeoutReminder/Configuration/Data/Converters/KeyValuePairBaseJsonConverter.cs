using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Converters;

public class KeyValuePairBaseJsonConverter<TKey, TValue> : JsonConverter<CollectionHelper.KeyValuePairBase<TKey, TValue>> where TKey : notnull where TValue : notnull {
  /// <inheritdoc />
  public override void WriteJson(JsonWriter writer, CollectionHelper.KeyValuePairBase<TKey, TValue>? value, JsonSerializer serializer) {
    if (value is null) {
      writer.WriteNull();
      return;
    }
    writer.WriteStartObject();
    writer.WritePropertyName(nameof(CollectionHelper.KeyValuePairBase<TKey, TValue>.Key));
    writer.WriteValue(value.Key);
    writer.WritePropertyName(nameof(CollectionHelper.KeyValuePairBase<TKey, TValue>.Value));
    serializer.Serialize(writer, value.Value, typeof(PlayerConfigEntry));
    writer.WriteEndObject();
  }

  /// <summary>
  /// Reads the JSON representation of the object.
  /// Do not call this, it is and should be unused.
  /// </summary>
  /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
  /// <param name="objectType">Type of the object.</param>
  /// <param name="existingValue">The existing value of object being read. If there is no existing value then <c>null</c> will be used.</param>
  /// <param name="hasExistingValue">The existing value has a value.</param>
  /// <param name="serializer">The calling serializer.</param>
  /// <returns>The object value.</returns>
  public override CollectionHelper.KeyValuePairBase<TKey, TValue> ReadJson(JsonReader reader, Type objectType, CollectionHelper.KeyValuePairBase<TKey, TValue>? existingValue, bool hasExistingValue, JsonSerializer serializer) {
    Svc.Log.Error("Attempted to read a {0} while converting it from Json.", nameof(CollectionHelper.KeyValuePairBase<TKey, TValue>));
    throw new NotImplementedException();
  }
}