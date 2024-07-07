using System;
using System.Threading;
using System.Threading.Tasks;

using XivCommon.Functions.Housing;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;
/// <summary>
/// TODO: Write summary.
/// </summary>
public class HousingTimer {
  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public HousingTimer() {
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="playerConfig"></param>
  /// <returns></returns>
  public bool CheckTime(int type, PerPlayerConfiguration playerConfig) {
    if (type == 0 && playerConfig.FreeCompanyEstate.Enabled) {
      var dateTimeOffset = (DateTimeOffset)DateTime.Now;
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.FreeCompanyEstate.LastVisit);
      if (dateTimeOffsetLast.ToUnixTimeSeconds() < dateTimeOffset.ToUnixTimeSeconds()) {
        return true;
      }
    } else if (type == 1 && playerConfig.PrivateEstate.Enabled) {
      var dateTimeOffset = (DateTimeOffset)DateTime.Now;
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.PrivateEstate.LastVisit);
      if (dateTimeOffsetLast.ToUnixTimeSeconds() < dateTimeOffset.ToUnixTimeSeconds()) {
        return true;
      }
    } else if (type == 2 && playerConfig.Apartment.Enabled) {
      var dateTimeOffset = (DateTimeOffset)DateTime.Now;
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.Apartment.LastVisit);
      if (dateTimeOffsetLast.ToUnixTimeSeconds() < dateTimeOffset.ToUnixTimeSeconds()) {
        return true;
      }
    }
    return false;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="playerConfig"></param>
  /// <returns></returns>
  public (bool, bool, bool) CheckTime(PerPlayerConfiguration playerConfig) {
    var dateTimeOffset1 = ((DateTimeOffset)DateTime.Now);
    var dateTimeOffsetAfterTime1 = DateTimeOffset.FromUnixTimeSeconds(playerConfig.FreeCompanyEstate.LastVisit).AddDays(Services.Config.DaysToWait);
    var dateTimeOffset2 = ((DateTimeOffset)DateTime.Now);
    var dateTimeOffsetAfterTime2 = DateTimeOffset.FromUnixTimeSeconds(playerConfig.PrivateEstate.LastVisit).AddDays(Services.Config.DaysToWait);
    var dateTimeOffset3 = ((DateTimeOffset)DateTime.Now);
    var dateTimeOffsetAfterTime3 = DateTimeOffset.FromUnixTimeSeconds(playerConfig.Apartment.LastVisit).AddDays(Services.Config.DaysToWait);
    return (playerConfig.FreeCompanyEstate.Enabled && dateTimeOffset1.ToUnixTimeSeconds() > dateTimeOffsetAfterTime1.ToUnixTimeSeconds(),
      playerConfig.PrivateEstate.Enabled && dateTimeOffset2.ToUnixTimeSeconds() > dateTimeOffsetAfterTime2.ToUnixTimeSeconds(),
      playerConfig.Apartment.Enabled && dateTimeOffset3.ToUnixTimeSeconds() > dateTimeOffsetAfterTime3.ToUnixTimeSeconds());
  }

  /// <summary>
  /// Gets if the player is in an Apartment per the <paramref name="territory"/> id.
  /// </summary>
  /// <param name="territory">The ID for the territory the player is in.</param>
  /// <return>If the player is in an Apartment</return>
  public bool IsApartment(ushort territory) {
    return territory switch {
      610 or 608 or 609 or 999 or 655 => true,
      _ => false,
    };
  }

  /// <summary>
  /// Convert the <paramref name="territory"/> to the <see cref="District"/>
  /// </summary>
  /// <param name="territory">The ID for the territory the player is in.</param>
  /// <return>The district the player is in.</return>
  public District ConvertToDistrict(ushort territory) {
    return territory switch {
      345 or 346 or 347 or 386 or 424 or 610 => District.Goblet,
      282 or 283 or 284 or 384 or 423 or 608 => District.Mist,
      342 or 343 or 344 or 385 or 425 or 609 => District.LavenderBeds,
      980 or 981 or 982 or 983 or 984 or 999 => District.Empyreum,
      649 or 650 or 651 or 652 or 653 or 655 => District.Shirogane,
      _ => District.Unknown,
    };
  }

  /// <summary>
  /// <see langword="async"/> function to get if <see cref="XivCommon.Functions"/> is <see langword="null"/> or not.
  /// </summary>
  /// <return>Returns delayed bool until function is not <see langword="null"/>.</return>
  public async Task<bool> TestFunctionsNotNullAsync() {
    while (Services.XivCommon.Functions.Housing.Location is null) {
      await Task.Delay(10);
    }
    await Task.Delay(2000);
    return true;
  }

  /// <summary>
  /// Checks the location of the player and returns <see langword="true"/> if successful.
  /// Inside House:
  ///     Apartment: null; ApartmentWing: null; Plot: 53; Ward: 26; Yard: null;
  /// On the Yard:
  ///     Apartment: null; ApartmentWing: null; Plot: null; Ward: 26; Yard: 53;
  /// </summary>
  /// <param name="territory">The ID for the territory the player is in.</param>
  /// <return>Returns <see langword="true"/> if successful.</return>
  public bool CheckLocation(ushort territory, PerPlayerConfiguration playerConfig) {
    if (Services.XivCommon.Functions.Housing.Location is not null) {
      HousingLocation loc = Services.XivCommon.Functions.Housing.Location;
      if (IsApartment(territory) && playerConfig.Apartment.Enabled) {
        ushort apartmentNumber = loc.Apartment ?? 0;
        bool apartmentWing = loc.ApartmentWing != 1;
        if (apartmentNumber == playerConfig.Apartment.ApartmentNumber
          && apartmentWing == playerConfig.Apartment.Subdistrict
          && loc.Ward == playerConfig.Apartment.Ward
          && ConvertToDistrict(territory) == playerConfig.Apartment.District && CheckTime(2, playerConfig)) {
          playerConfig.Apartment.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
          Update();
          return true;
        }
      } else {
        ushort plot = loc.Plot ?? 0;
        if (playerConfig.PrivateEstate.Enabled
          && plot == playerConfig.PrivateEstate.Plot
          && loc.Ward == playerConfig.PrivateEstate.Ward
          && ConvertToDistrict(territory) == playerConfig.PrivateEstate.District && CheckTime(1, playerConfig)) {
          playerConfig.PrivateEstate.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
          Update();
          return true;
        } else if (playerConfig.FreeCompanyEstate.Enabled
          && plot == playerConfig.FreeCompanyEstate.Plot
          && loc.Ward == playerConfig.FreeCompanyEstate.Ward
          && ConvertToDistrict(territory) == playerConfig.FreeCompanyEstate.District && CheckTime(0, playerConfig)) {
          playerConfig.FreeCompanyEstate.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
          Update();
          return true;
        }
      }
    }
    playerConfig.IsLate = CheckTime(playerConfig);
    return true;
  }

  /// <summary>
  /// The function to call when changing instance. Checks timers after.
  /// </summary>
  /// <param name="e">The territory ID as a ushort.</param>
  /// <param name="playerId">The id of the player to check.</param>
  public void OnTerritoryChanged(ushort e) {
    Task.Run(TestFunctionsNotNullAsync).ContinueWith((t) => CheckLocation(e, Configuration.GetCurrentPlayerConfig()));
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="playerConfig"></param>
  /// <returns></returns>
  public async Task<bool> ManualCheckAsync(PerPlayerConfiguration playerConfig) {
    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
    var _task = Task.Run(TestFunctionsNotNullAsync);
    bool _taskComplete;
    try {
      var _taskContinue = _task.ContinueWith((value) => value.Result && CheckLocation(Services.ClientState.TerritoryType, playerConfig));
      _taskComplete = await _taskContinue.WaitAsync(cts.Token);
    } catch (Exception ex) when (ex is OperationCanceledException) {
      Services.PluginLog.Error("Errored when waiting for task to complete.");
      Services.PluginLog.Error(ex.Message);
      return false;
    }
    cts.Dispose();
    return _taskComplete;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  private bool _isSaving;

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  private int _singletons = 0;

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  private void DoManualSave() {
    Task.Run(WaitAndSaveAsync);
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <returns></returns>
  private async Task WaitAndSaveAsync() {
    if (_singletons > 0) {
      return;
    }

    _singletons++;

    while (_isSaving) {
      await Task.Delay(2000);
    }

    _isSaving = true;
    Services.Config.Save();
    await Task.Delay(2000);
    _isSaving = false;
    _singletons--;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public void Update() {
    DoManualSave();
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <returns></returns>
  internal PlayerId GetCurrentPlayerId() {
    return Services.Config.PlayerConfigs.Find(x => x.PlayerId?.Equals(Services.GetCurrentPlayerName()) ?? false)?.PlayerId ?? Configuration.AddNewPlayerFromCurrent().PlayerId!;
  }
}
