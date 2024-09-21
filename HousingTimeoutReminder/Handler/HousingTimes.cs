

using System;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

/// <summary>
/// A readonly data struct for the player's housing information.
/// </summary>
public readonly struct HousingTimes {
  /// <summary>
  /// The instance of a <see cref="PlayerID"/>
  /// </summary>
  private readonly PlayerID? _playerID;

  /// <summary>
  /// Gets the instance of a <see cref="PerPlayerConfig"/> from this struct's <see cref="_playerID"/>.
  /// </summary>
  private PerPlayerConfig? PlayerConfig => _playerID is not null ? Config.GetPlayerConfig(_playerID) : null;

  private readonly bool _freeCompanyEstate = false;
  /// <summary>
  /// A bool specifying if the player's Free Company estate is past the
  /// check date.
  /// </summary>
  public bool FreeCompanyEstate => PlayerConfig?.FreeCompanyEstate.Enabled == true && _freeCompanyEstate;

  private readonly bool _privateEstate = false;
  /// <summary>
  /// A bool specifying if the player's private estate is past the
  /// check date.
  /// </summary>
  public bool PrivateEstate => PlayerConfig?.PrivateEstate.Enabled == true && _privateEstate;

  private readonly bool _apartment = false;
  /// <summary>
  /// A bool specifying if the player's apartment is past the
  /// check date.
  /// </summary>
  public bool Apartment => PlayerConfig?.Apartment.Enabled == true && _apartment;

  /// <summary>
  /// The constructor of this struct.
  /// </summary>
  public HousingTimes(PlayerID playerID, DateTimeOffset current, long freeCompanyAfter,  long peAfter, long apAfter) {
    this._playerID = playerID;
    long currentUnixTime = current.ToUnixTimeSeconds();
    this._freeCompanyEstate = currentUnixTime > freeCompanyAfter;
    this._privateEstate     = currentUnixTime > peAfter;
    this._apartment         = currentUnixTime > apAfter;
  }

  /// <summary>
  /// The internal constructor of this struct.
  /// For use with <see cref="Blank"/>
  /// </summary>
  private HousingTimes(bool @default = true) {
    this._freeCompanyEstate = @default;
    this._privateEstate     = @default;
    this._apartment         = @default;
  }

  /// <summary>
  /// A blank <see cref="HousingTimes"/> instance.
  /// </summary>
  public static HousingTimes Blank => new(false);
}
