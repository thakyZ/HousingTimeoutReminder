using Dalamud.Configuration;
using Dalamud.Plugin;

using FFXIVClientStructs.FFXIV.Common.Math;

using System;
using System.Collections.Generic;

namespace NekoBoiNick.HousingTimeoutReminder {
  /// <summary>
  /// 
  /// </summary>
  public enum District {
    Unknown = 0,
    Goblet = 1,
    Mist = 2,
    LavenderBeds = 3,
    Empyreum = 4,
    Shirogane = 5
  }

  /// <summary>
  /// 
  /// </summary>
  [Serializable]
  public class HousingPlot {
    /// <summary>
    /// 
    /// </summary>
    public bool Enabled { get; set; } = false;
    /// <summary>
    /// 
    /// </summary>
    public long LastVisit { get; set; } = 0;
    /// <summary>
    /// 
    /// </summary>
    public District District { get; set; } = District.Unknown;
    /// <summary>
    /// 
    /// </summary>
    public ushort Ward { get; set; } = 0;
    /// <summary>
    /// 
    /// </summary>
    public ushort Plot { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsValid() { return !District.Equals(District.Unknown) && (Ward > 0) && (Plot > 0); }
  }

  /// <summary>
  /// 
  /// </summary>
  [Serializable]
  public class Apartment {
    /// <summary>
    /// 
    /// </summary>
    public bool Enabled { get; set; } = false;
    /// <summary>
    /// 
    /// </summary>
    public long LastVisit { get; set; } = 0;
    /// <summary>
    /// 
    /// </summary>
    public District District { get; set; } = District.Unknown;
    /// <summary>
    /// 
    /// </summary>
    public ushort Ward { get; set; } = 0;
    /// <summary>
    /// 
    /// </summary>
    public bool Subdistrict { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public ushort ApartmentNumber { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsValid() { return !District.Equals(District.Unknown) && (Ward > 0) && (ApartmentNumber > 0); }
  }

  /// <summary>
  /// 
  /// </summary>
  [Serializable]
  public class PerPlayerConfiguration {
    /// <summary>
    /// 
    /// </summary>
    public string OwnerName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public HousingPlot FreeCompanyEstate { get; set; } = new HousingPlot();
    /// <summary>
    /// 
    /// </summary>
    public HousingPlot PrivateEstate { get; set; } = new HousingPlot();
    /// <summary>
    /// 
    /// </summary>
    public Apartment Apartment { get; set; } = new Apartment();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsNew() { return !string.IsNullOrEmpty(OwnerName); }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsValid() { return !string.IsNullOrEmpty(OwnerName) && (FreeCompanyEstate.IsValid() || PrivateEstate.IsValid() || Apartment.IsValid()); }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerConfig"></param>
    public void Update(PerPlayerConfiguration playerConfig) {
      this.FreeCompanyEstate = playerConfig.FreeCompanyEstate;
      this.PrivateEstate = playerConfig.PrivateEstate;
      this.Apartment = playerConfig.Apartment;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  [Serializable]
  public class Position {
    /// <summary>
    /// 
    /// </summary>
    public float X { get; set; } = 0;
    /// <summary>
    /// 
    /// </summary>
    public float Y { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector"></param>
    public void FromVector2(Vector2 vector) {
      this.X = vector.X;
      this.Y = vector.Y;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Vector2 ToVector2() {
      return new(this.X, this.Y);
    }
  }

  /// <summary>
  /// 
  /// </summary>
  [Serializable]
  public class Configuration : IPluginConfiguration {
    /// <summary>
    /// 
    /// </summary>
    public int Version { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public List<PerPlayerConfiguration> PlayerConfigs { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public ushort DaysToWait { get; set; } = 28;

    /// <summary>
    /// 
    /// </summary>
    public Position WarningPosition { get; set; } = new Position();

    /// <summary>
    /// 
    /// </summary>
    [NonSerialized]
    private DalamudPluginInterface pluginInterface;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pluginInterface"></param>
    public void Initialize(DalamudPluginInterface pluginInterface) {
      this.pluginInterface = pluginInterface;

      if (DaysToWait > 30) {
        DaysToWait = 30;
      }
      if (DaysToWait < 1) {
        DaysToWait = 1;
      }

      switch (Version) {
        default: break;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Save() {
      this.pluginInterface.SavePluginConfig(this);
    }
  }
}
