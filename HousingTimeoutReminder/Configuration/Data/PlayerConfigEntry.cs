using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Converters;
using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;

[Serializable]
[JsonConverter(typeof(PlayerConfigEntryJsonConverter))]
public class PlayerConfigEntry {
  private readonly string? _name;

  /// <summary>
  /// Gets or sets the name of the player who is tied to a player config.
  /// </summary>
  [Versions(introduced: 2)]
  public required string Name {
    get => _name ?? "INVALID CONFIG";
    init => _name = value;
  }

  private readonly string? _world;

  /// <summary>
  /// Gets or sets the name of the world the player who is tied to a player config.
  /// </summary>
  [Versions(introduced: 2)]
  public required string World {
    get => _world ?? "INVALID CONFIG";
    init => _world = value;
  }

  /// <summary>
  /// Gets or set the filename of the player config that is tied to this entry.
  /// </summary>
  [Versions(introduced: 2)]
  public required string? FileName { get; init; }

  public string DisplayName => this.Name + (string.IsNullOrEmpty(this.World) ? "" : $"@{this.World}");

  internal PlayerConfigEntry(string? fileName, string name, string world) {
    this.FileName = fileName;
    this._name = name;
    this._world = world;
  }

  /// <summary>
  /// Ensures a default parameterless constructor.
  /// </summary>
  public PlayerConfigEntry() { }

  /// <summary>
  /// Tests if a value of nullable <see cref="PlayerConfigEntry" /> is equal to this instance.
  /// </summary>
  /// <param name="other">The other key value pair.</param>
  /// <returns><see langworld="true" /> if they are equal, otherwise <see langworld="false" />.</returns>
  private bool Equals([NotNullWhen(true)] PlayerConfigEntry? other)
    => other is not null && this.Name.Equals(other.Name) && this.World.Equals(other.World) && this.FileName?.Equals(other.FileName) == true;

  /// <summary>
  /// Tests if a value of nullable <see cref="IPlayerCharacter" /> is equal to this instance.
  /// </summary>
  /// <param name="other">The other key value pair.</param>
  /// <returns><see langworld="true" /> if they are equal, otherwise <see langworld="false" />.</returns>
  private bool Equals([NotNullWhen(true)] IPlayerCharacter? other)
    => other is not null
       && this.Name.Equals(other.Name.ExtractText())
       && this.World.Equals(other.HomeWorld.GetName());

  /// <inheritdoc />
  public override bool Equals([NotNullWhen(true)] object? obj)
    => (obj is PlayerConfigEntry other && this.Equals(other: other))
       || (obj is IPlayerCharacter chara && this.Equals(other: chara)) ;

  /// <inheritdoc />
  public override int GetHashCode()
    => HashCode.Combine(this.FileName, this.World, this.Name);

  /// <inheritdoc />
  public override string ToString()
    => JsonConvert.SerializeObject(this, Formatting.None, new PlayerConfigEntryJsonConverter());

  /// <summary>
  /// Returns a <see langword="string" /> that represents the current object.
  /// </summary>
  /// <param name="format">
  /// The format in which to supply.
  /// <para><code>@</code> returns {Name}@{World}</para>
  /// <para><code>Json</code> returns <see cref="ToString()"/></para>
  /// </param>
  /// <returns>A <see langword="string" /> that represents the current object.</returns>
  public string ToString(string format) {
    return format switch {
      "@" => this.Name + "@" + this.World,
      // ReSharper disable once PatternIsRedundant
      _ or "Json" => this.ToString(),
    };
  }
}
