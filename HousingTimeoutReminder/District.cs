using System.ComponentModel;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;

public enum District {
  Unknown = 0,
  Goblet = 1,
  Mist = 2,
  LavenderBeds = 3,
  Empyreum = 4,
  Shirogane = 5,
}
public static class DistrictEnumHelper {
  public static string GetName(this District district) {
    return district switch {
      District.Goblet       => "The Goblet",
      District.Mist         => "The Mist",
      District.LavenderBeds => "The Lavender Beds",
      District.Empyreum     => "The Empyreum",
      District.Shirogane    => "Shirogane",
      _                     => "INVALID CONFIG",
    };
  }
}
