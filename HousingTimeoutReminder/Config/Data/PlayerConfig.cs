using ECommons.ExcelServices.TerritoryEnumeration;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Config.Data.Houses;
using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Config.Data;

public class PlayerConfig {
  [JsonIgnore]
  public bool IsNew { get; set; }
  private IHouse? _apartment;
  private IHouse? _freeCompanyEstate;
  private IHouse? _privateEstate;
  public IHouse Apartment {
    get => _apartment ??= new ApartmentHouse();
    set => _apartment = value;
  }
  public IHouse FreeCompanyEstate {
    get => _freeCompanyEstate ??= new FreeCompanyHouse();
    set => _freeCompanyEstate = value;
  }
  public IHouse PrivateEstate {
    get => _privateEstate ??= new PrivateHouse();
    set => _privateEstate = value;
  }
}
