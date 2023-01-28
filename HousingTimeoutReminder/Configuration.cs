using Dalamud.Configuration;
using Dalamud.Plugin;

using System;
using System.Collections.Generic;

namespace NekoBoiNick.HousingTimeoutReminder {
  public enum District {
    Unknown = 0,
    Goblet = 1,
    Mist = 2,
    LavenderBeds = 3,
    Empyreum = 4,
    Shirogane = 5
  }

  [Serializable]
  public class HousingPlot {
    public District District { get; set; } = District.Unknown;
    public ushort Ward { get; set; } = 0;
    public ushort Plot { get; set; } = 0;
  }


  [Serializable]
  public class Appartment {
    public District District { get; set; } = District.Unknown;
    public ushort Ward { get; set; } = 0;
    public ushort Subdistrict { get; set; } = 0;
    public ushort AppartmentNumber { get; set; } = 0;
  }

  [Serializable]
  public class PerPlayerConfiguration {
    public HousingPlot FreeCompanyHouse { get; set; } = new HousingPlot();
    public HousingPlot PrivateEstate { get; set; } = new HousingPlot();
    public Appartment Appartment { get; set; } = new Appartment();
  }

  [Serializable]
  public class Configuration : IPluginConfiguration {
    public int Version { get; set; } = 0;

    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

    public Dictionary<string, PerPlayerConfiguration> PlayerConfigs { get; set; } = new Dictionary<string, PerPlayerConfiguration>();

    // the below exist just to make saving less cumbersome

    [NonSerialized]
    private DalamudPluginInterface pluginInterface;

    public void Initialize(DalamudPluginInterface pluginInterface) {
      this.pluginInterface = pluginInterface;

      switch (Version) {
        default: break;
      }
    }

    public void Save() {
      this.pluginInterface.SavePluginConfig(this);
    }
  }
}
