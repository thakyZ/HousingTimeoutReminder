using System;

using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;

using FFXIVClientStructs.FFXIV.Common.Math;

using ImGuiNET;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;
public class WarningUI : Window, IDisposable {
  public static string Name { get => "Housing Timeout Reminder Warning"; }

  private const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse |
                                                 ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse |
                                                 ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove;

  public WarningUI() : base(Name, WindowFlags) {
    Size = new Vector2(500, 85) * ImGuiHelpers.GlobalScale;
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

  private Vector2 oldPoistion = Vector2.Zero;

  public void DrawTesting() {
    Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse |
            ImGuiWindowFlags.NoTitleBar;
    this.BgAlpha = 0.5f;
    if (!oldPoistion.Equals(ImGui.GetWindowPos())) {
      Services.Config.WarningPosition = HousingTimeoutReminder.Position.FromVector2(ImGui.GetWindowPos());
      oldPoistion = ImGui.GetWindowPos();
    }
  }

  private bool FCDismissed { get => Services.Instance.IsDismissed.Item1; set => Services.Instance.IsDismissed = (value, Services.Instance.IsDismissed.Item2, Services.Instance.IsDismissed.Item3); }
  private bool PDismissed { get => Services.Instance.IsDismissed.Item2; set => Services.Instance.IsDismissed = (Services.Instance.IsDismissed.Item1, value, Services.Instance.IsDismissed.Item3); }
  private bool ADismissed { get => Services.Instance.IsDismissed.Item3; set => Services.Instance.IsDismissed = (Services.Instance.IsDismissed.Item1, Services.Instance.IsDismissed.Item2, value); }

  public void ResetDismissed() {
    FCDismissed = false;
    PDismissed = false;
    ADismissed = false;
  }

  public bool DrawWarning(uint type, bool state) {
    if (!state) {
      return false;
    }
    this.BgAlpha = 0.5f;
    Flags = WindowFlags;
    Position = HousingTimeoutReminder.Position.ToVector2(Services.Config.WarningPosition);
    if (type == 0) {
      var dateTimeOffset = ((DateTimeOffset)DateTime.Now);
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(Services.HousingTimer.playerConfiguration.FreeCompanyEstate.LastVisit);
      var pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
      ImGui.Text($"Your Free Company Estate has not been visited in, {pastDays} days.");
      ImGui.Text("You can dismiss this at the button below.");
      if (ImGui.Button("Dismiss")) {
        FCDismissed = true;
      }
    } else if (type == 1) {
      var dateTimeOffset = ((DateTimeOffset)DateTime.Now);
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(Services.HousingTimer.playerConfiguration.PrivateEstate.LastVisit);
      var pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
      ImGui.Text($"Your Private Estate has not been visited in, {pastDays} days.");
      ImGui.Text("You can dismiss this at the button below.");
      if (ImGui.Button("Dismiss")) {
        PDismissed = true;
      }
    } else if (type == 2) {
      var dateTimeOffset = ((DateTimeOffset)DateTime.Now);
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(Services.HousingTimer.playerConfiguration.Apartment.LastVisit);
      var pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
      ImGui.Text($"Your Apartment has not been visited in, {pastDays} days.");
      ImGui.Text("You can dismiss this at the button below.");
      if (ImGui.Button("Dismiss")) {
        ADismissed = true;
      }
    }
    return true;
  }

  public override void Draw() {
    if ((!Services.Instance.IsLate.Item1 || FCDismissed) && (!Services.Instance.IsLate.Item2 || PDismissed) && (!Services.Instance.IsLate.Item3 || ADismissed)) {
      this.IsOpen = false;
    }
    if (Services.Instance.Testing) {
      DrawTesting();
    } else {
      if (DrawWarning(0, Services.Instance.IsLate.Item1 && !FCDismissed)) {
        ImGui.End();
      } else if (DrawWarning(1, Services.Instance.IsLate.Item2 && !PDismissed)) {
        ImGui.End();
      } else if (DrawWarning(2, Services.Instance.IsLate.Item3 && !ADismissed)) {
        ImGui.End();
      }
    }
    if (Position.HasValue) {
      Position = null;
    }
    ImGui.End();
  }
}
