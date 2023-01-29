using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;

using Dalamud.Interface.Windowing;

using NekoBoiNick.HousingTimeoutReminder;
using FFXIVClientStructs.FFXIV.Common.Math;
using HousingTimeoutReminder.Handler;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud;
using System.Reflection.Metadata.Ecma335;
using XivCommon.Functions.Housing;

namespace HousingTimeoutReminder.UI {
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

    private Dictionary<string, int> DistrictDict = new() { { "Unknown", 0 }, { "Goblet", 1 }, { "Mist", 2 }, { "LavenderBeds", 3 }, { "Empyreum", 4 }, { "Shirogane", 5 } };

    private const int WardMax = 32;
    private const int PlotMax = 60;
    private const int ApartmentMax = 90;

    private string CheckConsistancy(DateTimeOffset nextStamp) {
      if (nextStamp.ToUnixTimeSeconds() == 0) {
        return "Never";
      }
      if (nextStamp.ToUnixTimeSeconds() < ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()) {
        return "Now";
      }
      return $"{nextStamp:yyyy-MM-dd HH:mm:ss}";
    }

    public override void Draw() {
      if (Services.housingTimer.playerConfiguration is null) {
        return;
      }
      ImGui.Text($"Housing Configuration for {Services.housingTimer.playerConfiguration.OwnerName}:");

      ImGui.BeginChild("scrolling", new Vector2(0, -(25 + ImGui.GetStyle().ItemSpacing.Y) * ImGuiHelpers.GlobalScale), false);
      ImGui.PushID("Sorted Stacks");
      if (ImGui.CollapsingHeader("Global Settings:")) {
        ImGui.Text("Days To Wait");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        var daysToWait = (int)Services.pluginConfig.DaysToWait;
        if (ImGui.InputInt("##GlobalDaysToWait", ref daysToWait, 1, 5)) {
          if (daysToWait > 30) {
            daysToWait = 30;
          }
          if (daysToWait < 1) {
            daysToWait = 1;
          }
          Services.pluginConfig.DaysToWait = (ushort)daysToWait;
        }
      }

      if (ImGui.CollapsingHeader("Free Company Estate")) {
        if (Services.housingTimer.playerConfiguration.FreeCompanyEstate.IsValid()) {
          var LastVisit = CheckConsistancy(DateTimeOffset.FromUnixTimeSeconds(Services.housingTimer.playerConfiguration.FreeCompanyEstate.LastVisit));
          var NextVisit = CheckConsistancy(DateTimeOffset.FromUnixTimeSeconds(Services.housingTimer.playerConfiguration.FreeCompanyEstate.LastVisit).AddDays(Services.pluginConfig.DaysToWait));
          ImGui.Text($"Your last visit was on: {LastVisit:yyyy-MM-dd HH:mm:ss}");
          ImGui.SameLine();
          ImGui.Separator();
          ImGui.SameLine();
          ImGui.Text($"Your next visit is on: {NextVisit}");
        } else {
          ImGui.Text($"No free company estate set.");
        }
        ImGui.Text("Enabled");
        ImGui.SameLine();
        var enabled = Services.housingTimer.playerConfiguration.FreeCompanyEstate.Enabled;
        if (ImGui.Checkbox("##FreeCompanyEstateEnabled", ref enabled)) {
          Services.housingTimer.playerConfiguration.FreeCompanyEstate.Enabled = enabled;
        }
        ImGui.SameLine();
        ImGui.Separator();
        ImGui.SameLine();
        if (ImGui.Button("Reset##FreeCompanyEstateReset")) {
          Services.housingTimer.playerConfiguration.FreeCompanyEstate = new HousingPlot();
        }
        ImGui.Text("District");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        if (ImGui.BeginCombo("##FreeCompanyEstateDistrict", DistrictDict.Keys.ToList()[(int)Services.housingTimer.playerConfiguration.FreeCompanyEstate.District])) {
          foreach (var district in DistrictDict.Where(district => district.Value != 0)) {
            if (ImGui.Selectable(district.Key, district.Value == (int)Services.housingTimer.playerConfiguration.FreeCompanyEstate.District)) {
              Services.housingTimer.playerConfiguration.FreeCompanyEstate.District = (District)district.Value;
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
        var ward = (int)Services.housingTimer.playerConfiguration.FreeCompanyEstate.Ward;
        if (ImGui.InputInt("##FreeCompanyEstateWard", ref ward, 1, 20)) {
          if (ward > WardMax) {
            ward = WardMax;
          }
          if (ward < 1) {
            ward = 1;
          }
          Services.housingTimer.playerConfiguration.FreeCompanyEstate.Ward = (ushort)ward;
        }
        ImGui.SameLine();
        ImGui.Separator();
        ImGui.SameLine();
        ImGui.Text("Plot");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        var plot = (int)Services.housingTimer.playerConfiguration.FreeCompanyEstate.Plot;
        if (ImGui.InputInt("##FreeCompanyEstatePlot", ref plot, 1, 20)) {
          if (plot > PlotMax) {
            plot = PlotMax;
          }
          if (plot < 1) {
            plot = 1;
          }
          Services.housingTimer.playerConfiguration.FreeCompanyEstate.Plot = (ushort)plot;
        }
      }

      if (ImGui.CollapsingHeader("Private Estate")) {
        if (Services.housingTimer.playerConfiguration.PrivateEstate.IsValid()) {
          var LastVisit = CheckConsistancy(DateTimeOffset.FromUnixTimeSeconds(Services.housingTimer.playerConfiguration.PrivateEstate.LastVisit));
          var NextVisit = CheckConsistancy(DateTimeOffset.FromUnixTimeSeconds(Services.housingTimer.playerConfiguration.FreeCompanyEstate.LastVisit).AddDays(Services.pluginConfig.DaysToWait));
          ImGui.Text($"Your last visit was on: {LastVisit:yyyy-MM-dd HH:mm:ss}");
          ImGui.SameLine();
          ImGui.Separator();
          ImGui.SameLine();
          ImGui.Text($"Your next visit is on: {NextVisit}");
        } else {
          ImGui.Text($"No private estate set.");
        }
        ImGui.Text("Enabled");
        ImGui.SameLine();
        var enabled = Services.housingTimer.playerConfiguration.PrivateEstate.Enabled;
        if (ImGui.Checkbox("##PrivateEstateEnabled", ref enabled)) {
          Services.housingTimer.playerConfiguration.PrivateEstate.Enabled = enabled;
        }
        ImGui.SameLine();
        ImGui.Separator();
        ImGui.SameLine();
        if (ImGui.Button("Reset##PrivateEstateReset")) {
          Services.housingTimer.playerConfiguration.PrivateEstate = new HousingPlot();
        }
        ImGui.Text("District");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        if (ImGui.BeginCombo("##PrivateEstateDistrict", DistrictDict.Keys.ToList()[(int)Services.housingTimer.playerConfiguration.PrivateEstate.District])) {
          foreach (var district in DistrictDict.Where(district => district.Value != 0)) {
            if (ImGui.Selectable(district.Key, district.Value == (int)Services.housingTimer.playerConfiguration.PrivateEstate.District)) {
              Services.housingTimer.playerConfiguration.PrivateEstate.District = (District)district.Value;
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
        var ward = (int)Services.housingTimer.playerConfiguration.PrivateEstate.Ward;
        if (ImGui.InputInt("##PrivateEstateWard", ref ward, 1, 20)) {
          if (ward > WardMax) {
            ward = WardMax;
          }
          if (ward < 1) {
            ward = 1;
          }
          Services.housingTimer.playerConfiguration.PrivateEstate.Ward = (ushort)ward;
        }
        ImGui.SameLine();
        ImGui.Separator();
        ImGui.SameLine();
        ImGui.Text("Plot");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        var plot = (int)Services.housingTimer.playerConfiguration.PrivateEstate.Plot;
        if (ImGui.InputInt("##PrivateEstatePlot", ref plot, 1, 20)) {
          if (plot > PlotMax) {
            plot = PlotMax;
          }
          if (plot < 1) {
            plot = 1;
          }
          Services.housingTimer.playerConfiguration.PrivateEstate.Plot = (ushort)plot;
        }
      }

      if (ImGui.CollapsingHeader("Apartment")) {
        if (Services.housingTimer.playerConfiguration.Apartment.IsValid()) {
          var LastVisit = CheckConsistancy(DateTimeOffset.FromUnixTimeSeconds(Services.housingTimer.playerConfiguration.Apartment.LastVisit));
          var NextVisit = CheckConsistancy(DateTimeOffset.FromUnixTimeSeconds(Services.housingTimer.playerConfiguration.FreeCompanyEstate.LastVisit).AddDays(Services.pluginConfig.DaysToWait));
          ImGui.Text($"Your last visit was on: {LastVisit:yyyy-MM-dd HH:mm:ss}");
          ImGui.SameLine();
          ImGui.Separator();
          ImGui.SameLine();
          ImGui.Text($"Your next visit is on: {NextVisit}");
        } else {
          ImGui.Text($"No apartment Set.");
        }
        ImGui.Text("Enabled");
        ImGui.SameLine();
        var enabled = Services.housingTimer.playerConfiguration.Apartment.Enabled;
        if (ImGui.Checkbox("##ApartmentEnabled", ref enabled)) {
          Services.housingTimer.playerConfiguration.Apartment.Enabled = enabled;
        }
        ImGui.SameLine();
        ImGui.Separator();
        ImGui.SameLine();
        if (ImGui.Button("Reset##ApartmentReset")) {
          Services.housingTimer.playerConfiguration.Apartment = new Apartment();
        }
        ImGui.Text("District");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        if (ImGui.BeginCombo("##ApartmentDistrict", DistrictDict.Keys.ToList()[(int)Services.housingTimer.playerConfiguration.Apartment.District])) {
          foreach (var district in DistrictDict.Where(district => district.Value != 0)) {
            if (ImGui.Selectable(district.Key, district.Value == (int)Services.housingTimer.playerConfiguration.Apartment.District)) {
              Services.housingTimer.playerConfiguration.Apartment.District = (District)district.Value;
            }
          }
          ImGui.EndCombo();
        }
        ImGui.SameLine();
        ImGui.Separator();
        ImGui.SameLine();
        ImGui.Text("Is Subdistrict");
        ImGui.SameLine();
        var isSubdistrict = Services.housingTimer.playerConfiguration.Apartment.Subdistrict;
        if (ImGui.Checkbox("##ApartmentIsSubdistrict", ref isSubdistrict)) {
          Services.housingTimer.playerConfiguration.Apartment.Subdistrict = isSubdistrict;
        }
        ImGui.SameLine();
        ImGui.Separator();
        ImGui.SameLine();
        ImGui.Text("Ward");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        var ward = (int)Services.housingTimer.playerConfiguration.Apartment.Ward;
        if (ImGui.InputInt("##ApartmentWard", ref ward, 1, 20)) {
          if (ward > WardMax) {
            ward = WardMax;
          }
          if (ward < 1) {
            ward = 1;
          }
          Services.housingTimer.playerConfiguration.Apartment.Ward = (ushort)ward;
        }
        ImGui.Text("Apartment Number");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        var apartmentNumber = (int)Services.housingTimer.playerConfiguration.Apartment.ApartmentNumber;
        if (ImGui.InputInt("##ApartmentNumber", ref apartmentNumber, 1, 20)) {
          if (apartmentNumber > ApartmentMax) {
            apartmentNumber = ApartmentMax;
          }
          if (apartmentNumber < 1) {
            apartmentNumber = 1;
          }
          Services.housingTimer.playerConfiguration.Apartment.ApartmentNumber = (ushort)apartmentNumber;
        }
      }
      ImGui.PopID();
      ImGui.EndChild();
      if (ImGui.Button("Save")) Services.housingTimer.Update();
      ImGui.SameLine();
      if (ImGui.Button("Save and close")) {
        Services.housingTimer.Update();
        this.IsOpen = false;
      }
      ImGui.SameLine();
      ImGui.Separator();
      ImGui.SameLine();
      ImGui.Text("Test");
      ImGui.SameLine();
      var isTesting = Services.plugin.Testing;
      if (ImGui.Checkbox("##isTesting", ref isTesting)) {
        Services.plugin.Testing = isTesting;
      }

      ImGui.End();
    }
  }
}
