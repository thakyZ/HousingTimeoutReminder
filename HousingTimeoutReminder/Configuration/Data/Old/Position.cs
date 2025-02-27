using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

[Serializable]
[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
[Obsolete($"Use {nameof(Vector2)} instead")]
public class Position {
  /// <summary>
  /// Gets or sets the old property for the XUnit coordinate on the screen.
  /// </summary>
  [Versions(introduced: 0, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(Vector2)}.{nameof(Vector2.X)} instead")]
  public float? X { get; set; } = null;

  /// <summary>
  /// Gets or sets the old property for the YUnit coordinate on the screen.
  /// </summary>
  [Versions(introduced: 0, removed: 2)]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  [Obsolete($"Use {nameof(Vector2)}.{nameof(Vector2.Y)} instead")]
  public float? Y { get; set; } = null;

  /// <summary>
  /// Converts this instance to a <see cref="Vector2"/> class.
  /// </summary>
  /// <returns>Returns <see cref="Vector2"/> from this instance.</returns>
  public Vector2 ToSelfVector2() {
    return new Vector2(this.X.GetValueOrDefault(), this.Y.GetValueOrDefault());
  }

  /// <inheritdoc cref="ToSelfVector2"/>
  public static implicit operator Vector2(Position vector) {
    return vector.ToSelfVector2();
  }
}
