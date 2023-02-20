using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dalamud.Data;
using Dalamud.Logging;

using Lumina.Excel.GeneratedSheets;

using NekoBoiNick.HousingTimeoutReminder;

using XivCommon.Functions.Housing;

namespace HousingTimeoutReminder.Handler {
  public class HousingTimer {
    public PerPlayerConfiguration playerConfiguration { get; set; }
    public HousingTimer() {
    }

    public void Load(string playerName) {
      playerConfiguration = Services.pluginConfig.PlayerConfigs.Find(x => x.OwnerName.Equals(playerName));
    }

    public void Unload() {
      Services.pluginConfig.PlayerConfigs.Find(x => x.OwnerName.Equals(playerConfiguration.OwnerName)).Update(playerConfiguration);
      playerConfiguration = null;
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
      var dateTimeOffsetAfterTime1 = DateTimeOffset.FromUnixTimeSeconds(playerConfiguration.FreeCompanyEstate.LastVisit).AddDays(Services.pluginConfig.DaysToWait);
      var dateTimeOffset2 = ((DateTimeOffset)DateTime.Now);
      var dateTimeOffsetAfterTime2 = DateTimeOffset.FromUnixTimeSeconds(playerConfiguration.PrivateEstate.LastVisit).AddDays(Services.pluginConfig.DaysToWait);
      var dateTimeOffset3 = ((DateTimeOffset)DateTime.Now);
      var dateTimeOffsetAfterTime3 = DateTimeOffset.FromUnixTimeSeconds(playerConfiguration.Apartment.LastVisit).AddDays(Services.pluginConfig.DaysToWait);
      return (playerConfiguration.FreeCompanyEstate.Enabled && dateTimeOffset1.ToUnixTimeSeconds() > dateTimeOffsetAfterTime1.ToUnixTimeSeconds(),
        playerConfiguration.PrivateEstate.Enabled && dateTimeOffset2.ToUnixTimeSeconds() > dateTimeOffsetAfterTime2.ToUnixTimeSeconds(),
        playerConfiguration.Apartment.Enabled && dateTimeOffset3.ToUnixTimeSeconds() > dateTimeOffsetAfterTime3.ToUnixTimeSeconds());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="territory"></param>
    /// <returns></returns>
    public bool IsApartment(ushort territory) {
      return territory switch {
        610 or 608 or 609 or 999 or 655 => true,
        _ => false,
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="territory"></param>
    /// <returns></returns>
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
    /// 
    /// Inside House:
    ///     Apartment: null; ApartmentWing: null; Plot: 53; Ward: 26; Yard: null;
    /// On the Yard:
    ///     Apartment: null; ApartmentWing: null; Plot: null; Ward: 26; Yard: 53;
    /// </summary>
    /// <param name="territory"></param>
    public void CheckLocation(ushort territory) {
      if (Services.plugin.XivCommon.Functions.Housing.Location is not null) {
        HousingLocation loc = Services.plugin.XivCommon.Functions.Housing.Location;
        if (IsApartment(territory) && playerConfiguration.Apartment.Enabled) {
          ushort apartmentNumber = loc.Apartment ?? 0;
          bool apartmentWing = loc.ApartmentWing != 1;
          if (apartmentNumber == playerConfiguration.Apartment.ApartmentNumber
            && apartmentWing == playerConfiguration.Apartment.Subdistrict
            && loc.Ward == playerConfiguration.Apartment.Ward
            && ConvertToDistrict(territory) == playerConfiguration.Apartment.District && CheckTime(2)) {
            playerConfiguration.Apartment.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            Services.pluginConfig.PlayerConfigs.Find(x => x.OwnerName.Equals(playerConfiguration.OwnerName)).Update(playerConfiguration);
            return;
          }
        } else {
          ushort plot = loc.Plot ?? 0;
          if (playerConfiguration.PrivateEstate.Enabled
            && plot == playerConfiguration.PrivateEstate.Plot
            && loc.Ward == playerConfiguration.PrivateEstate.Ward
            && ConvertToDistrict(territory) == playerConfiguration.PrivateEstate.District && CheckTime(1)) {
            playerConfiguration.PrivateEstate.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            Services.pluginConfig.PlayerConfigs.Find(x => x.OwnerName.Equals(playerConfiguration.OwnerName)).Update(playerConfiguration);
            return;
          } else if (playerConfiguration.FreeCompanyEstate.Enabled
            && plot == playerConfiguration.FreeCompanyEstate.Plot
            && loc.Ward == playerConfiguration.FreeCompanyEstate.Ward
            && ConvertToDistrict(territory) == playerConfiguration.FreeCompanyEstate.District && CheckTime(0)) {
            playerConfiguration.FreeCompanyEstate.LastVisit = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            Services.pluginConfig.PlayerConfigs.Find(x => x.OwnerName.Equals(playerConfiguration.OwnerName)).Update(playerConfiguration);
            return;
          }
        }
      }
      Services.plugin.IsLate = CheckTime();
    }

    public void OnTerritoryChanged(object sender, ushort e) {
      Task.Delay(1000).ContinueWith(t => CheckLocation(e));
    }

    public void ManualCheck() {
      OnTerritoryChanged(this, Services.ClientState.TerritoryType);
    }

    public void Update() {
      Services.pluginConfig.PlayerConfigs.Find(x => x.OwnerName.Equals(playerConfiguration.OwnerName)).Update(playerConfiguration);
      Services.pluginConfig.Save();
    }
  }
}
