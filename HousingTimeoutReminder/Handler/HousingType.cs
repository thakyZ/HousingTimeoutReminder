namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

public enum HousingType {
  None = 0,
  Apartment = 1,
  PrivateEstate = 2,
  FreeCompanyEstate = 3,
}

public static class HousingTypeEnumHelper {
  public static string GetName(this HousingType district) {
    return district switch {
      HousingType.Apartment          => "Apartment",
      HousingType.PrivateEstate      => "Private Estate",
      HousingType.FreeCompanyEstate  => "Free Company Estate",
      _                              => "INVALID TYPE",
    };
  }
}