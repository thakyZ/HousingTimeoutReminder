using System;
using System.Threading;
using System.Threading.Tasks;

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
    var dateTimeOffsetAfterTime3 = DateTimeOffset.FromUnixTimeSeconds(playerConfiguration.Apartment.LastVisit).AddDays(Services.Config.DaysToWait);
    return (playerConfiguration.FreeCompanyEstate.Enabled && dateTimeOffset1.ToUnixTimeSeconds() > dateTimeOffsetAfterTime1.ToUnixTimeSeconds(),
      playerConfiguration.PrivateEstate.Enabled && dateTimeOffset2.ToUnixTimeSeconds() > dateTimeOffsetAfterTime2.ToUnixTimeSeconds(),
      playerConfiguration.Apartment.Enabled && dateTimeOffset3.ToUnixTimeSeconds() > dateTimeOffsetAfterTime3.ToUnixTimeSeconds());
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
    while (ConvertToDistrict(Services.ClientState.TerritoryType) == District.Unknown) {
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
  public unsafe bool CheckLocation(ushort territory) {
    var loc = HousingManager.GetCurrentLoc();
    if (loc.IsApartment && playerConfiguration.Apartment.Enabled) {
      int apartmentNumber = loc.Room;
      bool apartmentWing = loc.ApartmentWing != 1;
      if (apartmentNumber == playerConfiguration.Apartment.ApartmentNumber
        && apartmentWing == playerConfiguration.Apartment.Subdistrict
        && loc.Ward == playerConfiguration.Apartment.Ward
        && ConvertToDistrict(territory) == playerConfiguration.Apartment.District && CheckTime(2)) {
        playerConfiguration.Apartment.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        Update();
        return true;
      }
    } else if (!loc.IsApartment) {
      int plot = loc.Plot;
      if (playerConfiguration.PrivateEstate.Enabled
        && plot == playerConfiguration.PrivateEstate.Plot
        && loc.Ward == playerConfiguration.PrivateEstate.Ward
        && ConvertToDistrict(territory) == playerConfiguration.PrivateEstate.District && CheckTime(1)) {
        playerConfiguration.PrivateEstate.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        Update();
        return true;
      } else if (playerConfiguration.FreeCompanyEstate.Enabled
        && plot == playerConfiguration.FreeCompanyEstate.Plot
        && loc.Ward == playerConfiguration.FreeCompanyEstate.Ward
        && ConvertToDistrict(territory) == playerConfiguration.FreeCompanyEstate.District && CheckTime(0)) {
        playerConfiguration.FreeCompanyEstate.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        Update();
        return true;
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
    Task.Run(async () => await TestFunctionsNotNullAsync()).ContinueWith((t) => { if (t.Result) CheckLocation(e); });
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
