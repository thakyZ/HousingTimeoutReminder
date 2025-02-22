namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

/// <summary>
///
/// </summary>
public enum NewPlayerHandlingModes {
  Ask = 0,
  Import = 1,
  Blank = 2,
}

public static class NewPlayerHandlingModesEnumHelper {
  public static string GetName(this NewPlayerHandlingModes district) {
    return district switch {
      NewPlayerHandlingModes.Ask    => "Ask",
      NewPlayerHandlingModes.Import => "Import",
      NewPlayerHandlingModes.Blank  => "Blank",
      _                             => "INVALID CONFIG",
    };
  }
}
