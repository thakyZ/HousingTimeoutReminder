#if DEBUG || xPersonalRelease
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using ECommons;
using ImGuiNET;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;

public class DebugWindow : Window, IDisposable {
  private static string DebugWindowName => $"###{Plugin.InternalName}-{nameof(DebugWindow)}";

  /// <inheritdoc />
  public DebugWindow() : base(DebugWindowName, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse) {
    this.SizeCondition = ImGuiCond.Always;
  }

  /// <inheritdoc />
  public override void Draw() {
    ImGui.Text($"ImGui.GetStyle().ItemSpacing ( {ImGui.GetStyle().ItemSpacing.X}, {ImGui.GetStyle().ItemSpacing.Y} )");
    ImGui.SameLine();
    var defaultColor = ImGui.GetColorU32(ImGuiCol.Text).ToVector4();

    if (!Systems.IsLoggedIn) {
      return;
    }

    var currentLocation = HousingLocation.GetCurrentLocation(Svc.ClientState.TerritoryType);

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
      ImGui.TextColored(currentLocation.IsInside ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed, currentLocation.IsInside.ToString());
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Ward #: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLocation.Ward is not -1 ? defaultColor : ImGuiColors.DalamudRed, currentLocation.Ward is not -1 ? currentLocation.Ward.ToString() : "Null");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Plot #: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLocation.Plot is not -1 ? defaultColor : ImGuiColors.DalamudRed, currentLocation.Plot is not -1 ? currentLocation.Plot.ToString() : "Null");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Room #: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLocation.Room is not -1 ? defaultColor : ImGuiColors.DalamudRed, currentLocation.Room is not -1 ? currentLocation.Room.ToString() : "Null");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Division: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLocation.Division is not byte.MaxValue ? defaultColor : ImGuiColors.DalamudRed, currentLocation.Division is not byte.MaxValue ? currentLocation.Division.ToString() : "Null");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.Text("Apartment Wing: ");
      ImGui.TableSetColumnIndex(1);
      ImGui.TextColored(currentLocation.SubDistrictID is not byte.MaxValue ? defaultColor : ImGuiColors.DalamudRed, currentLocation.SubDistrictID is not byte.MaxValue ? currentLocation.SubDistrictID.ToString() : "Null");
      ImGui.EndTable();
    }
  }

  /// <inheritdoc cref="IDisposable.Dispose" />
  public void Dispose() {
    GC.SuppressFinalize(this);
  }
}
#endif
