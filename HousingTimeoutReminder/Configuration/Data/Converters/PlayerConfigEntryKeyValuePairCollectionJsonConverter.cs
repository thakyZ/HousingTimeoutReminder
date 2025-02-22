using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Converters;

public class PlayerConfigEntryKeyValuePairCollectionJsonConverter : JsonConverter<PlayerConfigEntryKeyValuePairCollection> {
  /// <inheritdoc />
  public override void WriteJson(JsonWriter writer, PlayerConfigEntryKeyValuePairCollection? value, JsonSerializer serializer) {
    writer.WriteStartObject();

    if (value is not null) {
      foreach (PlayerConfigEntryKeyValuePair keyPairValue in value) {
        writer.WritePropertyName(keyPairValue.PlayerID.ToString());
        serializer.Serialize(writer, keyPairValue.ConfigEntry);
      }
    }

    writer.WriteEndObject();
  }

  /// <inheritdoc />
  public override PlayerConfigEntryKeyValuePairCollection? ReadJson(JsonReader reader, Type objectType, PlayerConfigEntryKeyValuePairCollection? existingValue, bool hasExistingValue, JsonSerializer serializer) {
    if (reader.TokenType == JsonToken.Null) {
      Svc.Log.Warning("Null token type when parsing a {0}.", nameof(PlayerConfigEntryKeyValuePairCollection));
      return null;
    }

    var jsonIndex = 0;
    PlayerConfigEntryKeyValuePairCollection returnObject = [];

    var keyValueObj = JObject.Load(reader);

    foreach (KeyValuePair<string, JToken?> prop in keyValueObj) {
      if (prop.Value is null) {
        Svc.Log.Warning("Json Value at index, {0}, is null while parsing {1}.", jsonIndex, nameof(PlayerConfigEntryKeyValuePairCollection));
        jsonIndex++;
        continue;
      }

      if (!prop.Value.HasValues) {
        Svc.Log.Warning("Json Value at index, {0}, does not have values while parsing {1}.", jsonIndex, nameof(PlayerConfigEntryKeyValuePairCollection));
        jsonIndex++;
        continue;
      }

      if (prop.Value is not JObject) {
        Svc.Log.Warning("Json Value at index, {0}, is not type of {1} while parsing {2}.", jsonIndex, nameof(JObject), nameof(PlayerConfigEntryKeyValuePairCollection));
        jsonIndex++;
        continue;
      }

      PlayerConfigEntry? dependencyVersionRange = prop.Value.ToObject<PlayerConfigEntry>();

      if (dependencyVersionRange is null) {
        Svc.Log.Warning("Failed to parse Json Value at index, {0}, as {1} while parsing {2}.", jsonIndex, nameof(PlayerConfigEntry), nameof(PlayerConfigEntryKeyValuePairCollection));
        jsonIndex++;
        continue;
      }

      if (!ulong.TryParse(prop.Key, out ulong playerID)) {
        Svc.Log.Warning("Failed to parse Json key, {1} as {2} at index, {0}, while parsing {3}.", jsonIndex, prop.Key, nameof(UInt64), nameof(List<PlayerConfigEntryKeyValuePair>));
        jsonIndex++;
        continue;
      }

      returnObject.Add(new PlayerConfigEntryKeyValuePair(playerID, dependencyVersionRange));
    }

    return returnObject;
  }
}