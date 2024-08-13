namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

/// <summary>
/// The return booleans if the user has dismissed the warning for the property.
/// </summary>
public class Dismissed {
  /// <summary>
  /// The bool if player has dismissed warning for their FC House.
  /// </summary>
  public bool FreeCompanyEstate { get; set; }
  /// <summary>
  /// The bool if player has dismissed warning for their Private House.
  /// </summary>
  public bool PrivateEstate { get; set; }
  /// <summary>
  /// The bool if player has dismissed warning for their Apartment.
  /// </summary>
  public bool Apartment { get; set; }

  public void Reset() {
    this.FreeCompanyEstate = false;
    this.PrivateEstate = false;
    this.Apartment = false;
  }
}
