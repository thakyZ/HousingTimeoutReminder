using System;

using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Colors;
using FFXIVClientStructs.FFXIV.Common.Math;

using ImGuiNET;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;
public class DebugUI : Window, IDisposable {
  public static string Name { get => "Housing Timeout Reminder Debug Window"; }

  private const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;

  public DebugUI() : base(Name, WindowFlags) {
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

  public override void Draw() {
    var currentLoc = HousingManager.GetCurrentLoc();
    if (ImGui.BeginTable("DebugTable", 2)) {
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Is Apartment: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLoc.IsApartment ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed, currentLoc.IsApartment ? "True" : "False");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Is Inside: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLoc.IsInside ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed, currentLoc.IsInside ? "True" : "False");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Ward #: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.Text($"{currentLoc.Ward}");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Plot #: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.Text($"{currentLoc.Plot}");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Room #: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.Text($"{currentLoc.Room}");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Division: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.Text($"{currentLoc.Division}");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Apartment Wing: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.Text($"{currentLoc.ApartmentWing}");
      ImGui.EndTable();
    }
  }
}
