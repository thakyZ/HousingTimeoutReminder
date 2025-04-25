using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

using Dalamud.Game.ClientState.Objects.SubKinds;

using ECommons.DalamudServices;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

/// <summary>
/// A player identification class.
/// </summary>
[Serializable]
public sealed class PlayerID : IEquatable<PlayerID> {
  /// <summary>
  /// The first name of the player character.
  /// </summary>
  public string? FirstName { get; set; }

  /// <summary>
  /// The last name of the player character.
  /// </summary>
  public string? LastName { get; set; }

  /// <summary>
  /// The home world ID of the player character
  /// </summary>
  [MemberNotNullWhen(true, nameof(HomeWorldIsSet))]
  public uint? HomeWorld { get; set; }

  /// <summary>
  /// Gets whether the home world ID is properly set.
  /// </summary>
  [JsonIgnore]
  public bool HomeWorldIsSet => this.HomeWorld.HasValue;

  /// <summary>
  /// The home world display name of the player character.
  /// </summary>
  [JsonIgnore]
  public string? HomeWorldName => this.HomeWorldIsSet ? System.GetHomeWorldFromID(this.HomeWorld) : null;

  [JsonIgnore]
  public string FirstLastName => $"{this.FirstName} {this.LastName}";

  /// <summary>
  /// Creates a blank instance of the class.
  /// </summary>
  public PlayerID() {}

  /// <summary>
  /// Creates an instance of the class based off of a player character instance.
  /// </summary>
  /// <param name="player">A player character instance.</param>
  public PlayerID(IPlayerCharacter player) {
    var nameSplit = player.Name.TextValue.Split(' ', 2);
    this.FirstName = nameSplit[0];
    this.LastName = nameSplit[1];
    this.HomeWorld = player.HomeWorld.RowId;
  }

  /// <summary>
  /// Creates an instance of the class based off of the player's
  /// First name, last name and optionally home world.
  /// </summary>
  /// <param name="firstName">A player character's first name.</param>
  /// <param name="lastName">A player character's last name.</param>
  /// <param name="homeWorld">(Optional) A player character's home world id.</param>
  public PlayerID(string firstName, string lastName, uint? homeWorld = null) {
    this.FirstName = firstName;
    this.LastName = lastName;
    this.HomeWorld = homeWorld;
  }

  /// <summary>
  /// Creates an instance of the class based off of  the player's
  /// First name, last name and home world.
  /// </summary>
  /// <param name="firstName">A player character's first name.</param>
  /// <param name="lastName">A player character's last name.</param>
  /// <param name="homeWorld">A player character's home world id.</param>
  public PlayerID(string firstName, string lastName, uint homeWorld) {
    this.FirstName = firstName;
    this.LastName = lastName;
    this.HomeWorld = homeWorld;
  }

  /// <summary>
  /// Creates an instance of the class based off of  the player's display name.
  /// </summary>
  /// <param name="firstLastName">A player character's first name and last name.</param>
  public PlayerID(string firstLastName) {
    this.FirstName = firstLastName.Split(' ')[0];
    this.LastName = firstLastName.Split(' ')[1];
    this.HomeWorld = null;
  }

  /// <summary>
  /// Transforms the instance of this class to a readable format.
  /// </summary>
  /// <returns>A string containing "{first name} {last name}@{home world name}".</returns>
  public override string ToString() {
    return $"{this.FirstName} {this.LastName}@{this.HomeWorldName}";
  }

  /// <summary>
  /// Compares another instance of <see cref="PlayerID" /> to this instance.
  /// </summary>
  /// <param name="object">The instance of the other PlayerId</param>
  /// <returns>True if the equal, false otherwise.</returns>
  public bool Equals(PlayerID? @object) {
    if (@object is null) {
      return false;
    }

    return this.FirstName == @object.FirstName && this.LastName == @object.LastName && this.HomeWorld == @object.HomeWorld;
  }

  /// <summary>
  /// Compares an instance of a <see cref="PlayerID" />.<see cref="ToString()" /> output.
  /// </summary>
  /// <param name="string">The other <see cref="PlayerID" />'s output of the ToString() method.</param>
  /// <returns>True if the equal, false otherwise.</returns>
  public bool Equals(string? @string) {
    if (@string is null) {
      return false;
    }

    if (!@string.Contains('@')) {
      return $"{this.FirstName} {this.LastName}" == @string;
    }

    return this.ToString() == @string;
  }

  /// <inheritdoc />
  public override bool Equals(object? obj) {
    return obj switch {
      PlayerID @object => this.Equals(@object: @object),
      string @string => this.Equals(@string: @string),
      _ => false
    };
  }

  /// <inheritdoc />
  public override int GetHashCode() {
    return HashCode.Combine(this.FirstName?.GetHashCode(), this.LastName?.GetHashCode(), this.HomeWorld?.GetHashCode());
  }

  /// <summary>
  /// Sets the home world of this instance via a nullable home world id.
  /// </summary>
  /// <param name="newHomeWorld">A nullable home world id.</param>
  public void SetHomeWorld(uint? newHomeWorld) {
    this.HomeWorld = newHomeWorld ?? uint.MaxValue;
  }

  public static bool operator ==(PlayerID? objA, object? objB) {
    if (objA is null) {
      return objB is null;
    }

    if (objB is PlayerID idB) {
      return objA.Equals(idB);
    }

    return false;
  }

  public static bool operator !=(PlayerID? objA, object? objB) {
    return !(objA == objB);
  }

  [JsonExtensionData]
  [SuppressMessage("Roslynator", "RCS1169")]
  private IDictionary<string, JToken>? _additionalData = null;

  [OnDeserialized]
  private void OnDeserialized(StreamingContext context)
  {
    if (_additionalData is null) {
#if DEBUG
      Svc.Log.Warning("PlayerID _additionalData is null");
#endif
      return;
    }

#if DEBUG
    Svc.Log.Info($"_additionalData.Count: {_additionalData.Count}");
    foreach (string key in _additionalData.Keys) {
      Svc.Log.Info($"key: {key}");
    }
#endif
  }
}
