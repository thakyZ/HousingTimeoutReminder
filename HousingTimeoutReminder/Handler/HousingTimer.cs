using System;
using System.Threading;
using System.Threading.Tasks;

using Dalamud.Logging;

using XivCommon.Functions.Housing;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;
public class HousingTimer {
  public PerPlayerConfiguration playerConfiguration { get; set; }

  public HousingTimer() {
    if (Configuration.GetPlayerConfiguration() is null) {
      playerConfiguration = Configuration.GetPlayerConfiguration()!;
    } else {
      playerConfiguration = new PerPlayerConfiguration() { OwnerName = "Unknown" };
    }
  }

  public void Load() {
    playerConfiguration = Configuration.GetPlayerConfiguration()!;
  }

  public void Unload() {
    var playerConfig = Configuration.GetPlayerConfiguration();
    if (playerConfig == null || playerConfig is null) {
      PluginLog.Information($"isNull: {playerConfig == null || playerConfig is null}");
    } else {
      playerConfig!.Update(playerConfiguration);
    }
  }

  public bool CheckTime(int type) {
    if (type == 0 && playerConfiguration.FreeCompanyEstate.Enabled) {
      var dateTimeOffset = (DateTimeOffset)DateTime.Now;
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfiguration.FreeCompanyEstate.LastVisit);
      if (dateTimeOffsetLast.ToUnixTimeSeconds() < dateTimeOffset.ToUnixTimeSeconds()) {
        return true;
      }
    } else if (type == 1 && playerConfiguration.PrivateEstate.Enabled) {
      var dateTimeOffset = (DateTimeOffset)DateTime.Now;
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfiguration.PrivateEstate.LastVisit);
      if (dateTimeOffsetLast.ToUnixTimeSeconds() < dateTimeOffset.ToUnixTimeSeconds()) {
        return true;
      }
    } else if (type == 2 && playerConfiguration.Apartment.Enabled) {
      var dateTimeOffset = (DateTimeOffset)DateTime.Now;
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfiguration.Apartment.LastVisit);
      if (dateTimeOffsetLast.ToUnixTimeSeconds() < dateTimeOffset.ToUnixTimeSeconds()) {
        return true;
      }
    }
    return false;
  }
  public (bool, bool, bool) CheckTime() {
    var dateTimeOffset1 = ((DateTimeOffset)DateTime.Now);
    var dateTimeOffsetAfterTime1 = DateTimeOffset.FromUnixTimeSeconds(playerConfiguration.FreeCompanyEstate.LastVisit).AddDays(Services.Config.DaysToWait);
    var dateTimeOffset2 = ((DateTimeOffset)DateTime.Now);
    var dateTimeOffsetAfterTime2 = DateTimeOffset.FromUnixTimeSeconds(playerConfiguration.PrivateEstate.LastVisit).AddDays(Services.Config.DaysToWait);
    var dateTimeOffset3 = ((DateTimeOffset)DateTime.Now);
    var dateTimeOffsetAfterTime3 = DateTimeOffset.FromUnixTimeSeconds(playerConfiguration.Apartment.LastVisit).AddDays(Services.Config.DaysToWait);
    return (playerConfiguration.FreeCompanyEstate.Enabled && dateTimeOffset1.ToUnixTimeSeconds() > dateTimeOffsetAfterTime1.ToUnixTimeSeconds(),
      playerConfiguration.PrivateEstate.Enabled && dateTimeOffset2.ToUnixTimeSeconds() > dateTimeOffsetAfterTime2.ToUnixTimeSeconds(),
      playerConfiguration.Apartment.Enabled && dateTimeOffset3.ToUnixTimeSeconds() > dateTimeOffsetAfterTime3.ToUnixTimeSeconds());
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
    while (Services.PluginInstance.XivCommon.Functions.Housing.Location is null) {
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
  public bool CheckLocation(ushort territory) {
    if (Services.PluginInstance.XivCommon.Functions.Housing.Location is not null) {
      HousingLocation loc = Services.PluginInstance.XivCommon.Functions.Housing.Location;
      if (IsApartment(territory) && playerConfiguration.Apartment.Enabled) {
        ushort apartmentNumber = loc.Apartment ?? 0;
        bool apartmentWing = loc.ApartmentWing != 1;
        if (apartmentNumber == playerConfiguration.Apartment.ApartmentNumber
          && apartmentWing == playerConfiguration.Apartment.Subdistrict
          && loc.Ward == playerConfiguration.Apartment.Ward
          && ConvertToDistrict(territory) == playerConfiguration.Apartment.District && CheckTime(2)) {
          playerConfiguration.Apartment.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
          Update();
          return true;
        }
      } else {
        ushort plot = loc.Plot ?? 0;
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
    }
    Services.PluginInstance.IsLate = CheckTime();
    return true;
  }

  /// <summary>
  /// The function to call when changing instance. Checks timers after.
  /// </summary>
  /// <param name="sender">The object instance of the sender.</param>
  /// <param name="e">The territory ID as a ushort.</param>
  public void OnTerritoryChanged(ushort e) {
    Task.Run(async () => await TestFunctionsNotNullAsync()).ContinueWith((t) => CheckLocation(e));
  }

  public async Task<bool> ManualCheckAsync() {
    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
    var _task = Task.Run(TestFunctionsNotNullAsync);
    bool _taskComplete;
    try {
      var _taskContinue = _task.ContinueWith((value) => value.Result && CheckLocation(Services.ClientState.TerritoryType));
      _taskComplete = await _taskContinue.WaitAsync(cts.Token);
    } catch (Exception ex) when (ex is OperationCanceledException) {
      PluginLog.Error("Errored when waiting for task to complete.");
      PluginLog.Error(ex.Message);
      return false;
    }
    return _taskComplete;
  }

  private static bool _isSaving;

  private static int _singletons = 0;

  private void DoManualSave() {
    Task.Run(WaitAndSaveAsync);
  }

  private async Task WaitAndSaveAsync() {
    if (_singletons > 0) {
      return;
    }

    _singletons += 1;

    while (_isSaving) {
      await Task.Delay(2000);
    }

    _isSaving = true;
    Services.Config.Save();
    await Task.Delay(2000);
    _isSaving = false;
    _singletons -= 1;

    return;
  }

  public void Update() {
    Configuration.GetPlayerConfiguration()!.Update(playerConfiguration);
    DoManualSave();
  }
}
