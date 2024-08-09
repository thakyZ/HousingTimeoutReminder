using System;
using System.Collections.Generic;
using System.Linq;

using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;

using FFXIVClientStructs.FFXIV.Common.Math;

using ImGuiNET;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;
/// <summary>
/// TODO: Write summary.
/// </summary>
public class SettingsUI : Window, IDisposable {
  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  private const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse |
                                               ImGuiWindowFlags.NoScrollbar |
                                               ImGuiWindowFlags.NoScrollWithMouse;
  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public static string Name { get => "Housing Timeout Reminder Settings"; }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public SettingsUI() : base(Name, WindowFlags) {
    Size = new Vector2(630, y: 500) * ImGuiHelpers.GlobalScale;
    SizeCondition = ImGuiCond.Always;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public void Dispose() {
    this.Dispose(true);
    GC.SuppressFinalize(this);
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  private bool _isDisposed = false;

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="disposing"></param>
  protected virtual void Dispose(bool disposing) {
    if (!_isDisposed && disposing) {
      this._isDisposed = true;
    }
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  private readonly Dictionary<string, int> DistrictDict = new() {
    { "Unknown", 0 },
    { "Goblet", 1 },
    { "Mist", 2 },
    { "LavenderBeds", 3 },
    { "Empyreum", 4 },
    { "Shirogane", 5 }
  };

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  private const int WardMax = 32;

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  private const int PlotMax = 60;

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  private const int ApartmentMax = 90;

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="playerConfig"></param>
  /// <param name="next"></param>
  /// <param name="now"></param>
  /// <returns></returns>
  private DateTimeOffset ShortenFunction(int type, PerPlayerConfiguration playerConfig, bool next = false, bool now = false) {
    long visit = 0;
    switch (type) {
      case 0:
        visit = playerConfig.FreeCompanyEstate.LastVisit;
        break;
      case 1:
        visit = playerConfig.PrivateEstate.LastVisit;
        break;
      case 2:
        visit = playerConfig.Apartment.LastVisit;
        break;
    }
    var date = DateTimeOffset.FromUnixTimeSeconds(visit);
    if (next) {
      return date.AddDays(Services.Config.DaysToWait);
    }
    if (now) {
      return DateTimeOffset.Now;
    }
    return date;
  }

  private (string, string) CheckConsistency(DateTimeOffset lastStamp, DateTimeOffset nextStamp) {
    if (lastStamp.ToUnixTimeSeconds() <= 946627200 && nextStamp.ToUnixTimeSeconds() <= 946627200) {
      return ("Never", "Now");
    } else if (lastStamp.ToUnixTimeSeconds() <= nextStamp.ToUnixTimeSeconds() &&
      nextStamp.ToUnixTimeSeconds() >= ShortenFunction(0, playerConfig, false, true).ToUnixTimeSeconds()) {
      return ($"{lastStamp:yyyy-MM-dd HH:mm:ss}", $"{nextStamp:yyyy-MM-dd HH:mm:ss}");
      //return ($"{lastStamp.ToUnixTimeSeconds()}",$"{nextStamp.ToUnixTimeSeconds()}");
    } else if (nextStamp.ToUnixTimeSeconds() <= ShortenFunction(0, playerConfig, false, true).ToUnixTimeSeconds()) {
      return ($"{lastStamp:yyyy-MM-dd HH:mm:ss}", "Now");
    } else {
      return ("Never", "Now");
    }
  }

  /// <summary>
  /// Draws player timeout settings for a single user.
  /// </summary>
  /// <param name="playerId">The player ID involved with the single player.</param>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "<Pending>")]
  public void DrawUserTimeoutSettings(PerPlayerConfiguration playerConfig) {
    if (ImGui.CollapsingHeader("Free Company Estate")) {
      if (Services.HousingTimer.playerConfiguration.FreeCompanyEstate.IsValid()) {
        var Visit = CheckConsistency(ShortenFunction(0), ShortenFunction(0, true));
        ImGui.Text($"Your last visit was on: {Visit.Item1}");
        ImGui.SameLine();
        ImGui.Separator();
        ImGui.SameLine();
        ImGui.Text($"Your next visit is on: {Visit.Item2}");
      } else {
        ImGui.Text("No free company estate set.");
      }
      ImGui.Text("Enabled");
      ImGui.SameLine();
      var enabled = playerConfig.FreeCompanyEstate.Enabled;
      if (ImGui.Checkbox("##FreeCompanyEstateEnabled", ref enabled)) {
        playerConfig.FreeCompanyEstate.Enabled = enabled;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      if (ImGui.Button("Reset##FreeCompanyEstateReset")) {
        playerConfig.FreeCompanyEstate = new HousingPlot();
      }
      ImGui.Text("District");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      if (ImGui.BeginCombo("##FreeCompanyEstateDistrict", DistrictDict.Keys.ToList()[(int)playerConfig.FreeCompanyEstate.District])) {
        foreach (var district in DistrictDict.Where(district => district.Value != 0)) {
          if (ImGui.Selectable(district.Key, district.Value == (int)playerConfig.FreeCompanyEstate.District)) {
            playerConfig.FreeCompanyEstate.District = (District)district.Value;
          }
        }
        ImGui.EndCombo();
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      ImGui.Text("Ward");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      var ward = (int)playerConfig.FreeCompanyEstate.Ward;
      if (ImGui.InputInt("##FreeCompanyEstateWard", ref ward, 1, 20)) {
        if (ward > WardMax) {
          ward = WardMax;
        }
        if (ward < 1) {
          ward = 1;
        }
        playerConfig.FreeCompanyEstate.Ward = (ushort)ward;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      ImGui.Text("Plot");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      var plot = (int)playerConfig.FreeCompanyEstate.Plot;
      if (ImGui.InputInt("##FreeCompanyEstatePlot", ref plot, 1, 20)) {
        if (plot > PlotMax) {
          plot = PlotMax;
        }
        if (plot < 1) {
          plot = 1;
        }
        playerConfig.FreeCompanyEstate.Plot = (ushort)plot;
      }
    }

    if (ImGui.CollapsingHeader("Private Estate")) {
      if (Services.HousingTimer.playerConfiguration.PrivateEstate.IsValid()) {
        var Visit = CheckConsistency(ShortenFunction(1), ShortenFunction(1, true));
        ImGui.Text($"Your last visit was on: {Visit.Item1}");
        ImGui.SameLine();
        ImGui.Separator();
        ImGui.SameLine();
        ImGui.Text($"Your next visit is on: {Visit.Item2}");
      } else {
        ImGui.Text("No private estate set.");
      }
      ImGui.Text("Enabled");
      ImGui.SameLine();
      var enabled = playerConfig.PrivateEstate.Enabled;
      if (ImGui.Checkbox("##PrivateEstateEnabled", ref enabled)) {
        playerConfig.PrivateEstate.Enabled = enabled;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      if (ImGui.Button("Reset##PrivateEstateReset")) {
        playerConfig.PrivateEstate = new HousingPlot();
      }
      ImGui.Text("District");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      if (ImGui.BeginCombo("##PrivateEstateDistrict", DistrictDict.Keys.ToList()[(int)playerConfig.PrivateEstate.District])) {
        foreach (var district in DistrictDict.Where(district => district.Value != 0)) {
          if (ImGui.Selectable(district.Key, district.Value == (int)playerConfig.PrivateEstate.District)) {
            playerConfig.PrivateEstate.District = (District)district.Value;
          }
        }
        ImGui.EndCombo();
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      ImGui.Text("Ward");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      var ward = (int)playerConfig.PrivateEstate.Ward;
      if (ImGui.InputInt("##PrivateEstateWard", ref ward, 1, 20)) {
        if (ward > WardMax) {
          ward = WardMax;
        }
        if (ward < 1) {
          ward = 1;
        }
        playerConfig.PrivateEstate.Ward = (ushort)ward;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      ImGui.Text("Plot");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      var plot = (int)playerConfig.PrivateEstate.Plot;
      if (ImGui.InputInt("##PrivateEstatePlot", ref plot, 1, 20)) {
        if (plot > PlotMax) {
          plot = PlotMax;
        }
        if (plot < 1) {
          plot = 1;
        }
        playerConfig.PrivateEstate.Plot = (ushort)plot;
      }
    }

    if (ImGui.CollapsingHeader("Apartment")) {
      if (Services.HousingTimer.playerConfiguration.Apartment.IsValid()) {
        var Visit = CheckConsistency(ShortenFunction(2), ShortenFunction(2, true));
        ImGui.Text($"Your last visit was on: {Visit.Item1}");
        ImGui.SameLine();
        ImGui.Separator();
        ImGui.SameLine();
        ImGui.Text($"Your next visit is on: {Visit.Item2}");
      } else {
        ImGui.Text("No apartment Set.");
      }
      ImGui.Text("Enabled");
      ImGui.SameLine();
      var enabled = playerConfig.Apartment.Enabled;
      if (ImGui.Checkbox("##ApartmentEnabled", ref enabled)) {
        playerConfig.Apartment.Enabled = enabled;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      if (ImGui.Button("Reset##ApartmentReset")) {
        playerConfig.Apartment = new Apartment();
      }
      ImGui.Text("District");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      if (ImGui.BeginCombo("##ApartmentDistrict", DistrictDict.Keys.ToList()[(int)playerConfig.Apartment.District])) {
        foreach (var district in DistrictDict.Where(district => district.Value != 0)) {
          if (ImGui.Selectable(district.Key, district.Value == (int)playerConfig.Apartment.District)) {
            playerConfig.Apartment.District = (District)district.Value;
          }
        }
        ImGui.EndCombo();
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      ImGui.Text("Is Subdistrict");
      ImGui.SameLine();
      var isSubdistrict = playerConfig.Apartment.Subdistrict;
      if (ImGui.Checkbox("##ApartmentIsSubdistrict", ref isSubdistrict)) {
        playerConfig.Apartment.Subdistrict = isSubdistrict;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      ImGui.Text("Ward");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      var ward = (int)playerConfig.Apartment.Ward;
      if (ImGui.InputInt("##ApartmentWard", ref ward, 1, 20)) {
        if (ward > WardMax) {
          ward = WardMax;
        }
        if (ward < 1) {
          ward = 1;
        }
        playerConfig.Apartment.Ward = (ushort)ward;
      }
      ImGui.Text("Apartment Number");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      var apartmentNumber = (int)playerConfig.Apartment.ApartmentNumber;
      if (ImGui.InputInt("##ApartmentNumber", ref apartmentNumber, 1, 20)) {
        if (apartmentNumber > ApartmentMax) {
          apartmentNumber = ApartmentMax;
        }
        if (apartmentNumber < 1) {
          apartmentNumber = 1;
        }
        playerConfig.Apartment.ApartmentNumber = (ushort)apartmentNumber;
      }
    }

    if (ImGui.Button("Reset")) {
      Services.Instance.CheckTimers();
      playerConfig.IsDismissed = (false, false, false);
    }
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public override void Draw() {
    ImGui.BeginChild("scrolling", new Vector2(0, -(25 + ImGui.GetStyle().ItemSpacing.Y) * ImGuiHelpers.GlobalScale), false);
    ImGui.PushID("Sorted Stacks");
    if (ImGui.CollapsingHeader("Global Settings:")) {
      ImGui.Text("Days To Wait");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      var daysToWait = (int)Services.Config.DaysToWait;
      if (ImGui.InputInt("##GlobalDaysToWait", ref daysToWait, 1, 5)) {
        if (daysToWait > 30) {
          daysToWait = 30;
        }
        if (daysToWait < 1) {
          daysToWait = 1;
        }
        Services.Config.DaysToWait = (ushort)daysToWait;
      }
      var displayAllPlayers = Services.Config.ShowAllPlayers;
      if (ImGui.Checkbox("Show All Player Timeouts##GlobalShowAllPlayers", ref displayAllPlayers)) {
        Services.Config.ShowAllPlayers = displayAllPlayers;
      }
    }

    if (Services.Config.ShowAllPlayers) {
      Services.PluginLog.Debug($"Services.Config.PlayerConfigs.Count: {Services.Config.PlayerConfigs.Count}");
      foreach (var playerConfig in Services.Config.PlayerConfigs) {
        if (ImGui.CollapsingHeader($"Housing Configuration for {playerConfig.DisplayName}")) {
          DrawUserTimeoutSettings(playerConfig);
        }
      }
    } else if (Services.IsLoggedIn) {
      var playerConfig = Configuration.GetCurrentPlayerConfig();
      ImGui.Text($"Housing Configuration for {playerConfig.DisplayName}:");
      DrawUserTimeoutSettings(playerConfig);
    }

    ImGui.PopID();
    ImGui.EndChild();
    if (ImGui.Button("Save")) Services.HousingTimer.Update();
    ImGui.SameLine();
    if (ImGui.Button("Save and close")) {
      Services.HousingTimer.Update();
      this.IsOpen = false;
    }
    ImGui.SameLine();
    ImGui.Separator();
    ImGui.SameLine();
    ImGui.Text("Test");
    ImGui.SameLine();
    var isTesting = Services.Instance.Testing;
    if (ImGui.Checkbox("##isTesting", ref isTesting)) {
      Services.Instance.Testing = isTesting;
    }
    ImGui.SameLine();
    ImGui.Separator();
    ImGui.SameLine();
    if (ImGui.Button("Reset")) {
      Services.Instance.CheckTimers();
      Services.Instance.IsDismissed = (false, false, false);
    }
#if DEBUG
    ImGui.SameLine();
    ImGui.Separator();
    ImGui.SameLine();
    if (ImGui.Button("Debug")) {
      Services.DebugUI.IsOpen = true;
    }
#endif
  }
}
