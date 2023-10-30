using System;
using System.Collections.Generic;
using System.Linq;

using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;

using FFXIVClientStructs.FFXIV.Common.Math;

using ImGuiNET;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;
public class SettingsUI : Window, IDisposable {
  private const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse |
                                               ImGuiWindowFlags.NoScrollbar |
                                               ImGuiWindowFlags.NoScrollWithMouse;
  public static string Name { get => "Housing Timeout Reminder Settings"; }

  public SettingsUI() : base(Name, WindowFlags) {
    Size = new Vector2(630, 500) * ImGuiHelpers.GlobalScale;
    SizeCondition = ImGuiCond.Always;
  }

  public void Dispose() {
    this.Dispose(true);
    GC.SuppressFinalize(this);
  }

  private bool _isDisposed = false;

  protected virtual void Dispose(bool disposing) {
    if (!_isDisposed && disposing) {
      this._isDisposed = true;
    }
  }

  private readonly Dictionary<string, int> DistrictDict = new() {
    { "Unknown", 0 },
    { "Goblet", 1 },
    { "Mist", 2 },
    { "LavenderBeds", 3 },
    { "Empyreum", 4 },
    { "Shirogane", 5 }
  };

  private const int WardMax = 32;
  private const int PlotMax = 60;
  private const int ApartmentMax = 90;

  private DateTimeOffset ShortenFunction(int type, bool next = false, bool now = false) {
    long visit = 0;
    switch (type) {
      case 0:
        visit = Services.HousingTimer.playerConfiguration.FreeCompanyEstate.LastVisit;
        break;
      case 1:
        visit = Services.HousingTimer.playerConfiguration.PrivateEstate.LastVisit;
        break;
      case 2:
        visit = Services.HousingTimer.playerConfiguration.Apartment.LastVisit;
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

  private (string, string) CheckConsistancy(DateTimeOffset lastStamp, DateTimeOffset nextStamp) {
    if (lastStamp.ToUnixTimeSeconds() <= 946627200 && nextStamp.ToUnixTimeSeconds() <= 946627200) {
      return ("Never", "Now");
    } else if (lastStamp.ToUnixTimeSeconds() <= nextStamp.ToUnixTimeSeconds() &&
      nextStamp.ToUnixTimeSeconds() >= ShortenFunction(0, false, true).ToUnixTimeSeconds()) {
      return ($"{lastStamp:yyyy-MM-dd HH:mm:ss}", $"{nextStamp:yyyy-MM-dd HH:mm:ss}");
      //return ($"{lastStamp.ToUnixTimeSeconds()}",$"{nextStamp.ToUnixTimeSeconds()}");
    } else if (nextStamp.ToUnixTimeSeconds() <= ShortenFunction(0, false, true).ToUnixTimeSeconds()) {
      return ($"{lastStamp:yyyy-MM-dd HH:mm:ss}", "Now");
    } else {
      return ("Never", "Now");
    }
  }

  public override void Draw() {
    if (Services.HousingTimer.playerConfiguration is null) {
      return;
    }
    ImGui.Text($"Housing Configuration for {Services.HousingTimer.playerConfiguration.OwnerName}:");

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
    }

    if (ImGui.CollapsingHeader("Free Company Estate")) {
      if (Services.HousingTimer.playerConfiguration.FreeCompanyEstate.IsValid()) {
        var Visit = CheckConsistancy(ShortenFunction(0), ShortenFunction(0, true));
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
      var enabled = Services.HousingTimer.playerConfiguration.FreeCompanyEstate.Enabled;
      if (ImGui.Checkbox("##FreeCompanyEstateEnabled", ref enabled)) {
        Services.HousingTimer.playerConfiguration.FreeCompanyEstate.Enabled = enabled;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      if (ImGui.Button("Reset##FreeCompanyEstateReset")) {
        Services.HousingTimer.playerConfiguration.FreeCompanyEstate = new HousingPlot();
      }
      ImGui.Text("District");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      if (ImGui.BeginCombo("##FreeCompanyEstateDistrict", DistrictDict.Keys.ToList()[(int)Services.HousingTimer.playerConfiguration.FreeCompanyEstate.District])) {
        foreach (var district in DistrictDict.Where(district => district.Value != 0)) {
          if (ImGui.Selectable(district.Key, district.Value == (int)Services.HousingTimer.playerConfiguration.FreeCompanyEstate.District)) {
            Services.HousingTimer.playerConfiguration.FreeCompanyEstate.District = (District)district.Value;
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
      var ward = (int)Services.HousingTimer.playerConfiguration.FreeCompanyEstate.Ward;
      if (ImGui.InputInt("##FreeCompanyEstateWard", ref ward, 1, 20)) {
        if (ward > WardMax) {
          ward = WardMax;
        }
        if (ward < 1) {
          ward = 1;
        }
        Services.HousingTimer.playerConfiguration.FreeCompanyEstate.Ward = (ushort)ward;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      ImGui.Text("Plot");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      var plot = (int)Services.HousingTimer.playerConfiguration.FreeCompanyEstate.Plot;
      if (ImGui.InputInt("##FreeCompanyEstatePlot", ref plot, 1, 20)) {
        if (plot > PlotMax) {
          plot = PlotMax;
        }
        if (plot < 1) {
          plot = 1;
        }
        Services.HousingTimer.playerConfiguration.FreeCompanyEstate.Plot = (ushort)plot;
      }
    }

    if (ImGui.CollapsingHeader("Private Estate")) {
      if (Services.HousingTimer.playerConfiguration.PrivateEstate.IsValid()) {
        var Visit = CheckConsistancy(ShortenFunction(1), ShortenFunction(1, true));
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
      var enabled = Services.HousingTimer.playerConfiguration.PrivateEstate.Enabled;
      if (ImGui.Checkbox("##PrivateEstateEnabled", ref enabled)) {
        Services.HousingTimer.playerConfiguration.PrivateEstate.Enabled = enabled;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      if (ImGui.Button("Reset##PrivateEstateReset")) {
        Services.HousingTimer.playerConfiguration.PrivateEstate = new HousingPlot();
      }
      ImGui.Text("District");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      if (ImGui.BeginCombo("##PrivateEstateDistrict", DistrictDict.Keys.ToList()[(int)Services.HousingTimer.playerConfiguration.PrivateEstate.District])) {
        foreach (var district in DistrictDict.Where(district => district.Value != 0)) {
          if (ImGui.Selectable(district.Key, district.Value == (int)Services.HousingTimer.playerConfiguration.PrivateEstate.District)) {
            Services.HousingTimer.playerConfiguration.PrivateEstate.District = (District)district.Value;
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
      var ward = (int)Services.HousingTimer.playerConfiguration.PrivateEstate.Ward;
      if (ImGui.InputInt("##PrivateEstateWard", ref ward, 1, 20)) {
        if (ward > WardMax) {
          ward = WardMax;
        }
        if (ward < 1) {
          ward = 1;
        }
        Services.HousingTimer.playerConfiguration.PrivateEstate.Ward = (ushort)ward;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      ImGui.Text("Plot");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      var plot = (int)Services.HousingTimer.playerConfiguration.PrivateEstate.Plot;
      if (ImGui.InputInt("##PrivateEstatePlot", ref plot, 1, 20)) {
        if (plot > PlotMax) {
          plot = PlotMax;
        }
        if (plot < 1) {
          plot = 1;
        }
        Services.HousingTimer.playerConfiguration.PrivateEstate.Plot = (ushort)plot;
      }
    }

    if (ImGui.CollapsingHeader("Apartment")) {
      if (Services.HousingTimer.playerConfiguration.Apartment.IsValid()) {
        var Visit = CheckConsistancy(ShortenFunction(2), ShortenFunction(2, true));
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
      var enabled = Services.HousingTimer.playerConfiguration.Apartment.Enabled;
      if (ImGui.Checkbox("##ApartmentEnabled", ref enabled)) {
        Services.HousingTimer.playerConfiguration.Apartment.Enabled = enabled;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      if (ImGui.Button("Reset##ApartmentReset")) {
        Services.HousingTimer.playerConfiguration.Apartment = new Apartment();
      }
      ImGui.Text("District");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      if (ImGui.BeginCombo("##ApartmentDistrict", DistrictDict.Keys.ToList()[(int)Services.HousingTimer.playerConfiguration.Apartment.District])) {
        foreach (var district in DistrictDict.Where(district => district.Value != 0)) {
          if (ImGui.Selectable(district.Key, district.Value == (int)Services.HousingTimer.playerConfiguration.Apartment.District)) {
            Services.HousingTimer.playerConfiguration.Apartment.District = (District)district.Value;
          }
        }
        ImGui.EndCombo();
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      ImGui.Text("Is Subdistrict");
      ImGui.SameLine();
      var isSubdistrict = Services.HousingTimer.playerConfiguration.Apartment.Subdistrict;
      if (ImGui.Checkbox("##ApartmentIsSubdistrict", ref isSubdistrict)) {
        Services.HousingTimer.playerConfiguration.Apartment.Subdistrict = isSubdistrict;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      ImGui.Text("Ward");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      var ward = (int)Services.HousingTimer.playerConfiguration.Apartment.Ward;
      if (ImGui.InputInt("##ApartmentWard", ref ward, 1, 20)) {
        if (ward > WardMax) {
          ward = WardMax;
        }
        if (ward < 1) {
          ward = 1;
        }
        Services.HousingTimer.playerConfiguration.Apartment.Ward = (ushort)ward;
      }
      ImGui.Text("Apartment Number");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      var apartmentNumber = (int)Services.HousingTimer.playerConfiguration.Apartment.ApartmentNumber;
      if (ImGui.InputInt("##ApartmentNumber", ref apartmentNumber, 1, 20)) {
        if (apartmentNumber > ApartmentMax) {
          apartmentNumber = ApartmentMax;
        }
        if (apartmentNumber < 1) {
          apartmentNumber = 1;
        }
        Services.HousingTimer.playerConfiguration.Apartment.ApartmentNumber = (ushort)apartmentNumber;
      }
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

    ImGui.End();
  }
}
