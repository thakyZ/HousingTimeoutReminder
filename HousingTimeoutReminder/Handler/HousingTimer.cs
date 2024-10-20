using System;
using System.Threading.Tasks;

using ECommons.DalamudServices;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;
/// <summary>
/// A static housing timer function library.
/// </summary>
public static class HousingTimer {
  /// <summary>
  /// Gets the offset from two different <see cref="long" /> unix time stamps.
  /// </summary>
  /// <param name="lastVisit">The input unix time stamp.</param>
  /// <returns>The offset from the <see cref="lastVisit"/> with the amount of
  /// days added by <see cref="Config.DaysToWait"/>.</returns>
  public static long GetOffset(long lastVisit) {
    return DateTimeOffset.FromUnixTimeSeconds(lastVisit).AddDays(System.PluginConfig.DaysToWait).ToUnixTimeSeconds();
  }

  /// <summary>
  /// The method to get if <see cref="HousingManager"/> is ready or not.
  /// </summary>
  /// <return><see langword="true"/> if the housing message is ready.</return>
  public static bool TestFunctionsNotNull(ushort territory) {
    return HousingManager.ConvertToDistrict(territory) != District.Unknown;
  }

  /// <summary>
  /// Checks the location of the player and returns <see langword="true"/> if successful.
  /// Inside House:
  ///     Apartment: null; ApartmentWing: null; Plot: 53; Ward: 26; Yard: null;
  /// On the Yard:
  ///     Apartment: null; ApartmentWing: null; Plot: null; Ward: 26; Yard: 53;
  /// </summary>
  /// <param name="playerID">An player identification instance.</param>
  /// <param name="territory">The ID for the territory the player is in.</param>
  /// <return>Returns <see langword="true"/> if successful.</return>
  public static bool CheckLocation(PlayerID playerID, ushort territory) {
    try {
      var playerConfig = Config.GetPlayerConfig(playerID);
      var loc = HousingManager.GetCurrentLocation(territory);

      if (loc?.IsApartment == true && playerConfig.Apartment.Enabled) {
        var apartment = HousingManager.From(playerID, HousingType.Apartment);
        if (apartment?.Equals(loc) == true) {
          playerConfig.Apartment.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
          Update();
          return true;
        }
      }

      if (playerConfig.PrivateEstate.Enabled) {
        var privateEstate = HousingManager.From(playerID, HousingType.PrivateEstate);
        if (privateEstate?.Equals(loc) == true) {
          playerConfig.PrivateEstate.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
          Update();
          return true;
        }
      }

      if (playerConfig.FreeCompanyEstate.Enabled) {
        var freeCompanyEstate = HousingManager.From(playerID, HousingType.FreeCompanyEstate);
        if (freeCompanyEstate?.Equals(loc) == true) {
          playerConfig.FreeCompanyEstate.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
          Update();
          return true;
        }
      }
      return true;
    } catch (Exception exception) {
      Svc.Log.Error(exception, "Failed to check timers.");
    }

    return false;
  }

  /// <summary>
  /// The function to call when changing instance. Checks timers after.
  /// </summary>
  /// <param name="territory">The territory ID as a ushort.</param>
  public static void OnTerritoryChanged(ushort territory) {
    bool test = TestFunctionsNotNull(territory);
    PlayerID? playerID = System.GetCurrentPlayerID();

    if (test && playerID is not null) {
      Svc.Log.Warning("test && playerID is not null");
      CheckLocation(playerID, territory);
    }
#if DEBUG
    Svc.Log.Debug($"TestFunctionsNotNullAsync returned {test}.");
    Svc.Log.Debug(playerID is null
      ? "GetCurrentPlayerID returned null."
      : "GetCurrentPlayerID returned typeof PerPlayerConfig.");
#endif
  }

  /// <summary>
  /// Manually checks the ability to check for housing times when changing
  /// territory.
  /// </summary>
  /// <param name="playerID">An player identification instance.</param>
  /// <param name="territory">The territory id.</param>
  /// <returns><see langword="true"/> if successful and ready,
  /// otherwise <see langword="false"/>.</returns>
  public static bool ManualCheck(PlayerID playerID, ushort territory) {
    try {
      return TestFunctionsNotNull(territory) && CheckLocation(playerID, territory);
    } catch (OperationCanceledException operationCanceledException) {
      Svc.Log.Error(operationCanceledException, "Error when waiting for task to complete.");
      return false;
    } catch (Exception exception) {
      Svc.Log.Error(exception, "Failed to run task to manually check the housing timer.");
      return false;
    }
  }

  /// <summary>
  /// A lock for when the plugin config is saving.
  /// </summary>
  private static bool _isSaving;

  /// <summary>
  /// A check to make sure there is only one async  for saving.
  /// </summary>
  private static int _singletons = 0;

  /// <summary>
  /// Manually saves asynchronously.
  /// </summary>
  internal static void Update() {
    Task.Run(WaitAndSaveAsync);
  }

  /// <summary>
  /// The task to run when saving asynchronously.
  /// </summary>
  /// <returns>A generic <see cref="Task"/> object.</returns>
  private static async Task WaitAndSaveAsync() {
    if (_singletons > 0) {
      return;
    }

    _singletons++;

    while (_isSaving) {
      await Task.Delay(2000);
    }

    _isSaving = true;
    System.PluginConfig.Save();
    await Task.Delay(2000);
    _isSaving = false;
    _singletons--;
  }
}
