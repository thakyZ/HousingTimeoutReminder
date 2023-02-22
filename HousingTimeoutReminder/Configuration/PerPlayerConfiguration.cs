using System;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder {
  /// <summary>
  /// The configuration that applies per character and world.
  /// </summary>
  [Serializable]
  public class PerPlayerConfiguration {
    /// <summary>
    /// The name of the player and owner of the configuration.
    /// </summary>
    public string OwnerName { get; set; } = "Unknown";
    /// <summary>
    /// The player's Free Company estate location.
    /// </summary>
    public HousingPlot FreeCompanyEstate { get; set; } = new HousingPlot();
    /// <summary>
    /// The player's Private estate location.
    /// </summary>
    public HousingPlot PrivateEstate { get; set; } = new HousingPlot();
    /// <summary>
    /// The player's appartment location.
    /// </summary>
    public Apartment Apartment { get; set; } = new Apartment();

    /// <summary>
    /// Checks if the player config is new.
    /// </summary>
    /// <returns>Returns <see langword="true"/> if the player config is now.</returns>
    public bool IsNew() => OwnerName == "Unknown";
    /// <summary>
    /// Checks if the plyaer config is valid.
    /// </summary>
    /// <returns>Returns <see langword="true"/> if the player config is valid.</returns>
    public bool IsValid() { return !string.IsNullOrEmpty(OwnerName) && OwnerName != "Unknown" && (FreeCompanyEstate.IsValid() || PrivateEstate.IsValid() || Apartment.IsValid()); }

    /// <summary>
    /// Updates the player config with an instanced version.
    /// </summary>
    /// <param name="playerConfig">The player config that is instanced.</param>
    public void Update(PerPlayerConfiguration playerConfig) {
      this.FreeCompanyEstate = playerConfig.FreeCompanyEstate;
      this.PrivateEstate = playerConfig.PrivateEstate;
      this.Apartment = playerConfig.Apartment;
    }
  }
}
