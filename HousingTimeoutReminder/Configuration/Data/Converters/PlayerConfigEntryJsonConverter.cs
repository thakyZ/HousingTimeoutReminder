using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Converters;

public class PlayerConfigEntryJsonConverter : JsonConverter<PlayerConfigEntry> {
  /// <inheritdoc />
  public override void WriteJson(JsonWriter writer, PlayerConfigEntry? value, JsonSerializer serializer) {
    if (value is not null) {
      writer.WriteStartObject();
      writer.WritePropertyName(nameof(PlayerConfigEntry.Name));
      writer.WriteValue(value.Name);
      writer.WritePropertyName(nameof(PlayerConfigEntry.World));
      writer.WriteValue(value.World);
      writer.WritePropertyName(nameof(PlayerConfigEntry.FileName));
      writer.WriteValue(value.FileName);
      writer.WriteEndObject();
    } else {
      writer.WriteNull();
    }
  }

  /// <inheritdoc />
  public override PlayerConfigEntry? ReadJson(JsonReader reader, Type objectType, PlayerConfigEntry? existingValue, bool hasExistingValue, JsonSerializer serializer) {
    if (reader.TokenType == JsonToken.Null) {
      Svc.Log.Warning("Null token type when parsing a {1}.", nameof(PlayerConfigEntry));
      return null;
    }

    string? name = null;
    string? world = null;
    string? fileName = null;
    var jsonIndex = 0;

    while (reader.Read()) {
      if (reader.TokenType == JsonToken.PropertyName) {
        var propertyName = (string?)reader.Value;

        if (propertyName is null) {
          Svc.Log.Warning("Failed to parse property name at index {0} when parsing a {1}.", jsonIndex, nameof(PlayerConfigEntry));
          jsonIndex++;
          continue;
        }

        switch (propertyName) {
          case nameof(PlayerConfigEntry.Name):
            name = reader.ReadAsString();
            break;
          case nameof(PlayerConfigEntry.World):
            world = reader.ReadAsString();
            break;
          case nameof(PlayerConfigEntry.FileName):
            fileName = reader.ReadAsString();
            break;
          default:
            Svc.Log.Warning("Invalid property found named \"{0}\" at index {1} when parsing a {2}.", propertyName, jsonIndex, nameof(PlayerConfigEntry));
            break;
        }
      } else if (reader.TokenType == JsonToken.EndObject) {
        break;
      } else {
        Svc.Log.Warning("Invalid token type {0} at index {1} when parsing a {1}.", reader.TokenType, jsonIndex, nameof(PlayerConfigEntry));
      }

      jsonIndex++;
    }

    return name is not null && world is not null && fileName is not null
      ? new PlayerConfigEntry { FileName = fileName, Name = name, World = world }
      : null;
  }
}