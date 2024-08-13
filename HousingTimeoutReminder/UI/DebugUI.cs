using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Colors;

using ImGuiNET;

using ECommons.DalamudServices;

using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

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
      ImGui.TextColored(currentLocation.IsApartment ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed, currentLocation.IsApartment ? "True" : "False");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Is Inside: ");
      ImGui.TableSetColumnIndex(1);

      Vector4 color;

      if (currentLocation.IsInside.HasValue) {
        color = currentLocation.IsInside.Value ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed;
      } else {
        color = ImGuiColors.DalamudOrange;
      }

      string label = currentLocation.IsInside.HasValue ? currentLocation.IsInside.Value.ToString() : "Null";

      ImGui.TextColored(color, label);
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Ward #: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.Text($"{currentLocation.Ward}");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Plot #: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.Text($"{currentLocation.Plot}");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Room #: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.Text($"{currentLocation.Room}");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Division: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.Text($"{currentLocation.Division}");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Apartment Wing: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.Text($"{currentLocation.ApartmentWing}");
      ImGui.EndTable();
    }
  }
}
