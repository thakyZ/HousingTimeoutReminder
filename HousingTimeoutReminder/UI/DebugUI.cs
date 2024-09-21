using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Colors;

using ImGuiNET;

using ECommons.DalamudServices;

using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;
using ECommons;

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
    ImGui.Text($"ImGui.GetStyle().ItemSpacing ( {ImGui.GetStyle().ItemSpacing.X}, {ImGui.GetStyle().ItemSpacing.Y} )");
    ImGui.SameLine();
    var defaultColor = ImGui.GetColorU32(ImGuiCol.Text);

    if (!System.IsLoggedIn) {
      return;
    }

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
  }
}
