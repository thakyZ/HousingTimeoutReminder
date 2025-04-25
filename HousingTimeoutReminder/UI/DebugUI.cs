using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Colors;

using ImGuiNET;

using ECommons.DalamudServices;

using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;
using ECommons;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;
public class DebugUI : Window, IDisposable {
  private const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;

  public static string Name => "Housing Timeout Reminder Debug Window";

  public DebugUI() : base(Name, WindowFlags) {
    SizeCondition = ImGuiCond.Always;
  }

  public void Dispose() {
    GC.SuppressFinalize(this);
  }

  public override void Draw() {
    ImGui.BeginChild("ScrollArea##HousingTimeoutReminder.Debug", ImGui.GetWindowContentRegionMax(), false, ImGuiWindowFlags.AlwaysVerticalScrollbar | ImGuiWindowFlags.HorizontalScrollbar);
    var dateTimeNow = DateTimeOffset.Now.ToUnixTimeSeconds();
    var dateToWaitSeconds = System.PluginConfig.DateToWaitSeconds;
    ImGui.Text($"ImGui.GetStyle().ItemSpacing ( {ImGui.GetStyle().ItemSpacing.X}, {ImGui.GetStyle().ItemSpacing.Y} )");
    var defaultColor = ImGui.GetColorU32(ImGuiCol.Text);

    if (!System.IsLoggedIn) {
      return;
    }

    ImGui.Text("Warning UI Pagination");
    ImGui.Indent(20f);
    ImGui.Text($"CurrentMainPage = {System.WarningUI.Pagination.CurrentMainPage}");
    ImGui.Text($"TotalMainPages = {Pagination.TotalMainPages}");
    ImGui.Text($"CurrentSubPage = {System.WarningUI.Pagination.CurrentSubPage}");
    ImGui.Text($"TotalSubPages = {System.WarningUI.Pagination.ValidSubPages}");
    ImGui.Text($"CurrentPage = {System.WarningUI.Pagination.CurrentPage}");
    ImGui.Text($"TotalPages = {Pagination.TotalPages}");
    ImGui.Text($"CurrentlyDisplayedPlayer = {System.WarningUI.Pagination.CurrentPlayerConfig?.DisplayName ?? "NULL"}");
    ImGui.Indent(20f);
    foreach (var playerConfig in System.PluginConfig.PlayerConfigs) {
      ImGui.Text($"{playerConfig.DisplayName} -> MaxSubPages = {Pagination.GetValidSubPagesCount(playerConfig)}");
    }

    ImGui.Indent(-20f);
    ImGui.Indent(-20f);

    ImGui.Text($"Player Configs With Warnings: {System.PluginConfig.PlayerConfigsWithWarnings.Count}");
    ImGui.Indent(20f);
    foreach (var playerConfig in System.PluginConfig.PlayerConfigsWithWarnings) {
      ImGui.Text(playerConfig.DisplayName);
      ImGui.Indent(20f);
      ImGui.Text($"HasLateProperties = {playerConfig.HasLateProperties}");
      ImGui.Text($"IsValid = {playerConfig.IsValid}");
      ImGui.Text($"IsBroken = {playerConfig.IsBroken}");
      ImGui.Text($"IsCurrentPlayerConfig = {playerConfig.IsCurrentPlayerConfig}");
      ImGui.Indent(-20f);

      ImGui.Indent(20f);
      ImGui.Text(nameof(PerPlayerConfig.FreeCompanyEstate));
      ImGui.Indent(20f);
      ImGui.Text($"IsLate = {playerConfig.FreeCompanyEstate.IsLate}");
      ImGui.Text($"IsValid = {playerConfig.FreeCompanyEstate.IsValid}");
      ImGui.Text($"Enabled = {playerConfig.FreeCompanyEstate.Enabled}");
      ImGui.Text($"IsDismissed = {playerConfig.FreeCompanyEstate.IsDismissed}");
      ImGui.Text($"Time = {dateTimeNow} - {playerConfig.FreeCompanyEstate.LastVisit} > {dateToWaitSeconds} = {dateTimeNow - playerConfig.FreeCompanyEstate.LastVisit > dateToWaitSeconds}");
      ImGui.Indent(-20f);
      ImGui.Indent(-20f);

      ImGui.Indent(20f);
      ImGui.Text(nameof(PerPlayerConfig.PrivateEstate));
      ImGui.Indent(20f);
      ImGui.Text($"IsLate = {playerConfig.PrivateEstate.IsLate}");
      ImGui.Text($"IsValid = {playerConfig.PrivateEstate.IsValid}");
      ImGui.Text($"Enabled = {playerConfig.PrivateEstate.Enabled}");
      ImGui.Text($"IsDismissed = {playerConfig.PrivateEstate.IsDismissed}");
      ImGui.Text($"Time = {dateTimeNow} - {playerConfig.PrivateEstate.LastVisit} > {dateToWaitSeconds} = {dateTimeNow - playerConfig.PrivateEstate.LastVisit > dateToWaitSeconds}");
      ImGui.Indent(-20f);
      ImGui.Indent(-20f);

      ImGui.Indent(20f);
      ImGui.Text(nameof(PerPlayerConfig.Apartment));
      ImGui.Indent(20f);
      ImGui.Text($"IsLate = {playerConfig.Apartment.IsLate}");
      ImGui.Text($"IsValid = {playerConfig.Apartment.IsValid}");
      ImGui.Text($"Enabled = {playerConfig.Apartment.Enabled}");
      ImGui.Text($"IsDismissed = {playerConfig.Apartment.IsDismissed}");
      ImGui.Text($"Time = {dateTimeNow} - {playerConfig.Apartment.LastVisit} > {dateToWaitSeconds} = {dateTimeNow - playerConfig.Apartment.LastVisit > dateToWaitSeconds}");
      ImGui.Indent(-20f);
      ImGui.Indent(-20f);
    }

    ImGui.Indent(-20f);

    var currentLocation = HousingManager.GetCurrentLocation(Svc.ClientState.TerritoryType);

    // ReSharper disable once InvertIf
    if (ImGui.BeginTable("DebugTable", 2)) {
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Is Apartment: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLocation?.IsApartment == true ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed, currentLocation?.IsApartment == true ? "True" : "False");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Is Inside: ");
      ImGui.TableSetColumnIndex(1);

      Vector4 color;

      if (currentLocation?.IsInside is not null) {
        color = currentLocation.IsInside.Value ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed;
      } else {
        color = ImGuiColors.DalamudOrange;
      }

      string label = currentLocation?.IsInside is not null ? currentLocation.IsInside.Value.ToString() : "Null";

      ImGui.TextColored(color, label);
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Ward #: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLocation?.Ward is not null ? defaultColor.ToVector4() : ImGuiColors.DalamudRed, currentLocation?.Ward is not null ? currentLocation.Ward.ToString() : "Null");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Plot #: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLocation?.Plot is not null ? defaultColor.ToVector4() : ImGuiColors.DalamudRed, currentLocation?.Plot is not null ? currentLocation.Plot.ToString() : "Null");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Room #: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLocation?.Room is not null ? defaultColor.ToVector4() : ImGuiColors.DalamudRed, currentLocation?.Room is not null ? currentLocation.Room.ToString() : "Null");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Division: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLocation?.Division is not null ? defaultColor.ToVector4() : ImGuiColors.DalamudRed, currentLocation?.Division is not null ? currentLocation.Division.ToString() : "Null");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Apartment Wing: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLocation?.ApartmentWing is not null ? defaultColor.ToVector4() : ImGuiColors.DalamudRed, currentLocation?.ApartmentWing is not null ? currentLocation.ApartmentWing.ToString() : "Null");
      ImGui.EndTable();
    }

    ImGui.EndChild();
  }
}
