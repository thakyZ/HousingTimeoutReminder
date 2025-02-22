using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Houses;
using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;

[Serializable]
[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
public class PlayerConfig {
  /// <summary>
  /// Gets or sets a runtime value if this player config is new.
  /// </summary>
  [JsonIgnore]
  public bool IsNew { get; set; }

  private HouseBase? _apartment;

  /// <summary>
  /// Gets or sets the apartment config of this player's config.
  /// </summary>
  [Versions(introduced: 2)]
  public HouseBase Apartment {
    get => _apartment ??= new ApartmentHouseBase();
    set => _apartment = value;
  }

  private HouseBase? _freeCompanyEstate;

  /// <summary>
  /// Gets or sets the free company estate config of this player's config.
  /// </summary>
  [Versions(introduced: 2)]
  public HouseBase FreeCompanyEstate {
    get => _freeCompanyEstate ??= new FreeCompanyHouseBase();
    set => _freeCompanyEstate = value;
  }

  private HouseBase? _privateEstate;

  /// <summary>
  /// Gets or sets the private estate config of this player's config.
  /// </summary>
  [Versions(introduced: 2)]
  public HouseBase PrivateEstate {
    get => _privateEstate ??= new PrivateHouseBase();
    set => _privateEstate = value;
  }

  /// <summary>
  /// Copies settings from another instance of <see cref="PlayerConfig" /> to the current instance.
  /// </summary>
  /// <param name="other">The other instance to copy from.</param>
  public void CopyFrom(PlayerConfig other) {
    Apartment = other.Apartment;
    FreeCompanyEstate = other.FreeCompanyEstate;
    PrivateEstate = other.PrivateEstate;
  }

  /// <summary>
  /// Ensures a default parameterless constructor.
  /// </summary>
  public PlayerConfig() { }

#pragma warning disable CA2208
  [SuppressMessage("ReSharper", "SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault")]
  public HouseBase this[HousingType type] {
    get {
      return type switch {
        HousingType.Apartment => this.Apartment,
        HousingType.FreeCompanyEstate => this.FreeCompanyEstate,
        HousingType.PrivateEstate => this.PrivateEstate,
        _ => throw new ArgumentOutOfRangeException($"Argument {nameof(type)} on indexer for {nameof(PlayerConfig)} returned an invalid value.")
      };
    }
  }
#pragma warning restore CA2208
}
