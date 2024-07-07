using System;

using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;

using FFXIVClientStructs.FFXIV.Common.Math;

using ImGuiNET;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;
/// <summary>
/// TODO: Write summary.
/// </summary>
public class WarningUI : Window, IDisposable {
  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public static string Name { get => "Housing Timeout Reminder Warning"; }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  private const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse |
                                               ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse |
                                               ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove;

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public WarningUI() : base(Name, WindowFlags) {
    Size = new Vector2(500, 85) * ImGuiHelpers.GlobalScale;
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
  private Vector2 oldPoistion = Vector2.Zero;

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public void DrawTesting() {
    Flags = ImGuiWindowFlags.NoResize    | ImGuiWindowFlags.NoCollapse        |
            ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse |
            ImGuiWindowFlags.NoTitleBar;
    this.BgAlpha = 0.5f;
    if (!oldPoistion.Equals(ImGui.GetWindowPos())) {
      Services.Config.WarningPosition = HousingTimeoutReminder.Position.FromVector2(ImGui.GetWindowPos());
      oldPoistion = ImGui.GetWindowPos();
    }
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="playerConfig"></param>
  /// <param name="value"></param>
  private void SetFCDismissed(PerPlayerConfiguration playerConfig, bool value) {
    playerConfig.IsDismissed = (value, playerConfig.IsDismissed.Private, playerConfig.IsDismissed.Apartment);
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="playerConfig"></param>
  /// <param name="value"></param>
  private void SetPDismissed(PerPlayerConfiguration playerConfig, bool value) {
    playerConfig.IsDismissed = (playerConfig.IsDismissed.FreeCompany, value, playerConfig.IsDismissed.Apartment);
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="playerConfig"></param>
  /// <param name="value"></param>
  private void SetADismissed(PerPlayerConfiguration playerConfig, bool value) {
    playerConfig.IsDismissed = (playerConfig.IsDismissed.FreeCompany, playerConfig.IsDismissed.Private, value);
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="playerConfig"></param>
  public void ResetDismissed(PerPlayerConfiguration playerConfig) {
    SetFCDismissed(playerConfig, false);
    SetPDismissed(playerConfig, false);
    SetADismissed(playerConfig, false);
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="playerConfig"></param>
  public void DisplayForPlayer(int type, PerPlayerConfiguration playerConfig) {
    if (type == 0) {
      var dateTimeOffset = (DateTimeOffset)DateTime.Now;
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.FreeCompanyEstate.LastVisit);
      var pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
      ImGui.Text($"Your Free Company Estate has not been visited in, {pastDays} days.");
      ImGui.Text("You can dismiss this at the button below.");
      if (ImGui.Button("Dismiss")) {
        SetFCDismissed(playerConfig, true);
      }
    } else if (type == 1) {
      var dateTimeOffset = (DateTimeOffset)DateTime.Now;
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.PrivateEstate.LastVisit);
      var pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
      ImGui.Text($"Your Private Estate has not been visited in, {pastDays} days.");
      ImGui.Text("You can dismiss this at the button below.");
      if (ImGui.Button("Dismiss")) {
        SetPDismissed(playerConfig, true);
      }
    } else if (type == 2) {
      var dateTimeOffset = (DateTimeOffset)DateTime.Now;
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.Apartment.LastVisit);
      var pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
      ImGui.Text($"Your Apartment has not been visited in, {pastDays} days.");
      ImGui.Text("You can dismiss this at the button below.");
      if (ImGui.Button("Dismiss")) {
        SetADismissed(playerConfig, true);
      }
    }
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="playerConfig"></param>
  /// <returns></returns>
  public bool DrawWarning(int type, PerPlayerConfiguration playerConfig) {
    bool state = false;
    switch (type) {
      case 0:
        state = playerConfig.IsLate.FreeCompany && !playerConfig.IsDismissed.FreeCompany;
        break;
      case 1:
        state = playerConfig.IsLate.Private && !playerConfig.IsDismissed.Private;
        break;
      case 2:
        state = playerConfig.IsLate.Apartment && !playerConfig.IsDismissed.Apartment;
        break;
    }
    if (!state) {
      return false;
    }
    this.BgAlpha = 0.5f;
    Flags = WindowFlags;
    Position = HousingTimeoutReminder.Position.ToVector2(Services.Config.WarningPosition);
    DisplayForPlayer(type, playerConfig);
    return true;
  }

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  private int playerPage = 0;

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  private int playerTypePage = 0;

  /// <summary>
  /// TODO: Write summary.
  /// </summary>
  public override void Draw() {
    if (Services.Instance.Testing) {
      DrawTesting();
    } else {
      if (Services.Config.ShowAllPlayers) {
        var playerConfig = Services.Config.PlayerConfigs[playerPage];
        if ((!playerConfig.IsLate.FreeCompany || playerConfig.IsDismissed.FreeCompany) && (!playerConfig.IsLate.Private || playerConfig.IsDismissed.Private) && (!playerConfig.IsLate.Apartment || playerConfig.IsDismissed.Apartment)) {
          this.IsOpen = false;
        }
        DrawWarning(playerTypePage, playerConfig);

        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.SameLine();
        if (ImGui.Button(FontAwesomeIcon.ArrowLeft.ToIconString(), new Vector2(16, 16))) {
          playerTypePage -= 1;
          if (playerTypePage < 0) {
            playerTypePage = 3;
            playerPage -= 1;
          }
          if (playerPage < 0) {
            playerPage = Services.Config.PlayerConfigs.Count - 1;
          }
        }
        ImGui.SameLine();
        if (ImGui.Button(FontAwesomeIcon.ArrowRight.ToIconString(), new Vector2(16, 16))) {
          playerTypePage += 1;
          if (playerTypePage > 3) {
            playerTypePage = 0;
            playerPage += 1;
          }
          if (playerPage >= Services.Config.PlayerConfigs.Count) {
            playerPage = 0;
          }
        }
        ImGui.PopFont();
      } else {
        var playerConfig = Configuration.GetCurrentPlayerConfig();
        if ((!playerConfig.IsLate.FreeCompany || playerConfig.IsDismissed.FreeCompany) && (!playerConfig.IsLate.Private || playerConfig.IsDismissed.Private) && (!playerConfig.IsLate.Apartment || playerConfig.IsDismissed.Apartment)) {
          this.IsOpen = false;
        }
        DrawWarning(playerTypePage, playerConfig);

        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.SameLine();
        if (ImGui.Button(FontAwesomeIcon.ArrowLeft.ToIconString(), new Vector2(16, 16))) {
          playerTypePage -= 1;
          if (playerTypePage < 0) {
            playerTypePage = 3;
          }
        }
        ImGui.SameLine();
        if (ImGui.Button(FontAwesomeIcon.ArrowRight.ToIconString(), new Vector2(16, 16))) {
          playerTypePage += 1;
          if (playerTypePage > 3) {
            playerTypePage = 0;
          }
        }
        ImGui.PopFont();
      }

      ImGui.End();
      if (Position.HasValue) {
        Position = null;
      }
      ImGui.End();
    }
  }
}
