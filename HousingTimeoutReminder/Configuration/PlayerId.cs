using System;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

/// <summary>
/// A player identification class.
/// </summary>
[Serializable]
public sealed class PlayerID : IEquatable<PlayerID> {
  /// <summary>
  /// The first name of the player character.
  /// </summary>
  public string FirstName { get; set; }

  /// <summary>
  /// The last name of the player character.
  /// </summary>
  public string LastName { get; set; }

  /// <summary>
  /// The home world ID of the player character
  /// </summary>
  public uint HomeWorld { get; set; }

  /// <summary>
  /// Gets whether the home world ID is properly set.
  /// </summary>
  [JsonIgnore]
  public bool HomeWorldIsSet => this.HomeWorld != uint.MaxValue;

  /// <summary>
  /// The home world display name of the player character.
  /// </summary>
  [JsonIgnore]
  public string? HomeWorldName => this.HomeWorldIsSet ? System.GetHomeWorldFromID(this.HomeWorld) : "unknown";

  /// <summary>
  /// Gets whether this player identity property is new.
  /// </summary>
  [JsonIgnore]
  public bool IsNew => this.FirstName == "Unknown" && this.LastName.Length == 0 && !this.HomeWorldIsSet;

  /// <summary>
  /// Creates a blank instance of the class.
  /// </summary>
  public PlayerID() {
    this.FirstName = "Unknown";
    this.LastName = "";
    this.HomeWorld = uint.MaxValue;
  }

  /// <summary>
  /// Creates an instance of the class based off of a player character instance.
  /// </summary>
  /// <param name="player">A player character instance.</param>
  public PlayerID(IPlayerCharacter player) {
    var nameSplit = player.Name.TextValue.Split(' ', 2);
    this.FirstName = nameSplit[0];
    this.LastName = nameSplit[1];
    this.HomeWorld = player.HomeWorld.Id;
  }

  /// <summary>
  /// Creates an instance of the class based off of the player's
  /// First name, last name and optionally home world.
  /// </summary>
  /// <param name="firstName">A player character's first name.</param>
  /// <param name="lastName">A player character's last name.</param>
  /// <param name="homeWorld">(Optional) A player character's home world id.</param>
  public PlayerID(string firstName, string lastName, uint? homeWorld) {
    this.FirstName = firstName;
    this.LastName = lastName;
    this.HomeWorld = homeWorld ?? uint.MaxValue;
  }

  /// <summary>
  /// Creates an instance of the class based off of  the player's
  /// First name, last name and home world.
  /// </summary>
  /// <param name="firstName">A player character's first name.</param>
  /// <param name="lastName">A player character's last name.</param>
  /// <param name="homeWorld">(Optional) A player character's home world id.</param>
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
    this.HomeWorld = uint.MaxValue;
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
  /// <param name="objPlayerID">The instance of the other PlayerId</param>
  /// <returns>True if the equal, false otherwise.</returns>
  public bool Equals(PlayerID? objPlayerID) {
    if (objPlayerID is null) {
      return false;
    }

    bool isHomeWorld = !(this.HomeWorldIsSet && objPlayerID.HomeWorldIsSet) || this.HomeWorld == objPlayerID.HomeWorld;

    return this.FirstName == objPlayerID.FirstName && this.LastName == objPlayerID.LastName && isHomeWorld;
  }

  /// <summary>
  /// Compares an instance of a <see cref="PlayerID" />.<see cref="ToString()" /> output.
  /// </summary>
  /// <param name="strPlayerID">The other <see cref="PlayerID" />'s output of the ToString() method.</param>
  /// <returns>True if the equal, false otherwise.</returns>
  public bool Equals(string? strPlayerID) {
    if (strPlayerID is null) {
      return false;
    }

    if (!strPlayerID.Contains('@')) {
      return $"{this.FirstName} {this.LastName}" == strPlayerID;
    }

    return this.ToString() == strPlayerID;
  }

  /// <inheritdoc />
  public override bool Equals(object? obj) {
    if (obj is PlayerID objPlayerID) {
      this.Equals(objPlayerID: objPlayerID);
    } else if (obj is string strPlayerID) {
      this.Equals(strPlayerID: strPlayerID);
    }
    return false;
  }

  /// <inheritdoc />
  public override int GetHashCode() {
    return this.FirstName.GetHashCode() | this.LastName.GetHashCode() | this.HomeWorld.GetHashCode();
  }

  /// <summary>
  /// Sets the home world of this instance via a nullable home world id.
  /// </summary>
  /// <param name="homeWorld">A nullable home world id.</param>
  public void SetHomeWorld(uint? homeWorld) {
    this.HomeWorld = homeWorld ?? uint.MaxValue;
  }

  /// <summary>
  /// A blank <see cref="PlayerID"/> instance.
  /// </summary>
  public static PlayerID Blank => new();

}
