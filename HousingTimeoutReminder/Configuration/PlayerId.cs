using System;

using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
/// <summary>
/// TODO: Write summary.
/// </summary>
[Serializable]
public sealed class PlayerId : IEquatable<PlayerId> {
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
  public string? HomeWorldName => this.HomeWorldIsSet ? Services.GetHomeWorldFromId(this.HomeWorld) : "unknown";

  /// <summary>
  /// Gets whether this player Id property is new.
  /// </summary>
  [JsonIgnore]
  public bool IsNew => this.FirstName == "Unknown" && this.LastName == "" && !this.HomeWorldIsSet;

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public PlayerId() {
    this.FirstName = "Unknown";
    this.LastName = "";
    this.HomeWorld = uint.MaxValue;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="firstName"></param>
  /// <param name="lastName"></param>
  /// <param name="homeWorld"></param>
  public PlayerId(string firstName, string lastName, uint? homeWorld) {
    this.FirstName = firstName;
    this.LastName = lastName;
    this.HomeWorld = homeWorld ?? uint.MaxValue;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="firstName"></param>
  /// <param name="lastName"></param>
  /// <param name="homeWorld"></param>
  public PlayerId(string firstName, string lastName, uint homeWorld) {
    this.FirstName = firstName;
    this.LastName = lastName;
    this.HomeWorld = homeWorld;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="firstLastName"></param>
  public PlayerId(string firstLastName) {
    this.FirstName = firstLastName.Split(' ')[0];
    this.LastName = firstLastName.Split(' ')[1];
    this.HomeWorld = uint.MaxValue;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <returns></returns>
  public override string ToString() {
    return $"{this.FirstName} {this.LastName}@{this.HomeWorldName}";
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="objPlayerId"></param>
  /// <returns></returns>
  public bool Equals(PlayerId? objPlayerId) {
    if (objPlayerId is null) return false;
    var homeWorldBoolean = !(this.HomeWorldIsSet && objPlayerId.HomeWorldIsSet) || this.HomeWorld == objPlayerId.HomeWorld;
    return this.FirstName == objPlayerId.FirstName && this.LastName == objPlayerId.LastName && homeWorldBoolean;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="strPlayerId"></param>
  /// <returns></returns>
  public bool Equals(string? strPlayerId) {
    if (strPlayerId is null) return false;
    if (!strPlayerId.Contains('@')) {
      return $"{this.FirstName} {this.LastName}" == strPlayerId;
    }
    return this.ToString() == strPlayerId;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public override bool Equals(object? obj) {
    if (obj is PlayerId objPlayerId) {
      this.Equals(objPlayerId: objPlayerId);
    } else if (obj is string strPlayerId) {
      this.Equals(strPlayerId: strPlayerId);
    }
    return false;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <returns></returns>
  public override int GetHashCode() {
    return this.FirstName.GetHashCode() | this.LastName.GetHashCode() | this.HomeWorld.GetHashCode();
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="homeWorld"></param>
  public void SetHomeWorld(uint? homeWorld) {
    this.HomeWorld = homeWorld ?? uint.MaxValue;
  }
}
