using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions.ImGui;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;

public class WarningDialog : Window, IDisposable {
  /// <summary>
  /// Gets a <see langword="string" /> that represents the name of with <see cref="Window" />.
  /// </summary>
  private static string WarningDialogName => $"###{Plugin.InternalName}-{nameof(WarningDialog)}";

  /// <summary>
  /// A field that indicates the whole dialog is dismissed.
  /// </summary>
  private bool _isDismissed;

  private bool ShouldOpen
    => (Systems.WarningEntries.Any(x => !x.WarningEntry.IsDismissed)
        && !this._isDismissed && !this.UIHidden) || this._isRepositioning;

  /// <summary>
  /// Gets or sets a value that indicates the dalamud UI is hidden.
  /// </summary>
  private bool UIHidden { get; set; }

  /// <summary>
  /// A field that indicates that is dialog is being repositioned.
  /// </summary>
  private bool _isRepositioning;

  /// <summary>
  /// Gets or sets a value that indicates the current page the dialog is on.
  /// </summary>
  private int CurrentPage { get; set; }

  /// <summary>
  /// Caching the old position when repositioning the locked window.
  /// </summary>
  private Vector2 _oldPosition = Vector2.Zero;

  private const ImGuiWindowFlags _popupTestFlags = ImGuiWindowFlags.Popup;
  private const ImGuiWindowFlags _regularFlags = ImGuiWindowFlags.NoResize    | ImGuiWindowFlags.NoCollapse        |
                                                 ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse |
                                                 ImGuiWindowFlags.NoTitleBar  | ImGuiWindowFlags.NoMove;

  /// <inheritdoc />
  public WarningDialog() : base(WarningDialogName, _regularFlags) {
    this.AllowClickthrough = true;
    this.AllowPinning = true;
    this.Size = ImGuiHelpers.ScaledVector2(500, 85);
    this.SizeCondition = ImGuiCond.Always;
  }

  /// <summary>
  /// Draw the window a specific way when it needs to be repositioned.
  /// </summary>
  private void DrawRepositioning() {
    // ReSharper disable SuggestVarOrType_SimpleTypes
    using var _1_ = ImGuiRaii.PushColor(ImGuiCol.Border, ImGuiColors.DalamudRed);
    using var _2_ = ImGuiRaii.PushStyle(ImGuiStyleVar.WindowBorderSize, 1.0f);
    using var _3_ = ImGuiRaii.PushColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, 0.1f));
    // ReSharper restore SuggestVarOrType_SimpleTypes
    Flags = ImGuiWindowFlags.NoResize    | ImGuiWindowFlags.NoCollapse        |
            ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse |
            ImGuiWindowFlags.NoTitleBar;

    this.BgAlpha = 0.5f;

    if (this._oldPosition.Equals(ImGui.GetWindowPos())) {
      return;
    }

    Vector2 windowPosition = ImGui.GetWindowPos();
    Plugin.Systems.Config.Global.WarningPosition = windowPosition;
    this._oldPosition = windowPosition;
  }

  /// <inheritdoc />
  public override void Draw() {
    // ReSharper disable once PatternAlwaysMatches
    if (this.ShouldOpen && this.IsOpen != this.ShouldOpen) {
      this.IsOpen = this.ShouldOpen;
    }

    if (this._isRepositioning) {
      DrawRepositioning();
    }

    if (Systems.WarningEntries.Count != 0) {
      WarningEntry? warningEntry = Systems.WarningEntries[this.CurrentPage];

      if (warningEntry is not null) {
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (warningEntry.ConfigEntry.Equals(PlayerManager.GetCurrentPlayerConfigEntry())) {
          ImGui.Text($"You haven't visited your {warningEntry.HousingType.GetName()} in {warningEntry.DaysMissed} days.");
        } else {
          ImGui.Text($"{warningEntry.ConfigEntry.ToString("@")} hasn't visited their {warningEntry.HousingType.GetName()} in {warningEntry.DaysMissed} days.");
        }
      } else {
        ImGui.Text($"Warning entry at page {this.CurrentPage} was not found.");
      }

      if (Systems.NumberFailed > 0) {
        ImGui.Text($"Failed to process {Systems.NumberFailed} warning entries.");

        ImGui.SameLine();
      }

      if (ImGui.Button("Dismiss All")) {
        this.DismissAll();
      }

      ImGui.SameLine();

      if (ImGui.Button("Dismiss")) {
        this.DismissCurrent();
      }

      ImGui.SameLine();

      if (ImGuiComponents.IconButton(FontAwesomeIcon.ChevronLeft)) {
        this.PreviousPage();
      }

      ImGui.SameLine();

      if (ImGuiComponents.IconButton(FontAwesomeIcon.ChevronRight)) {
        this.NextPage();
      }
    }


    if (Position.HasValue) {
      Position = null;
    }
  }

  /// <summary>
  /// Increments <see cref="CurrentPage" /> up by one.
  /// </summary>
  private void NextPage() {
    if (this.CurrentPage > Systems.WarningEntries.Count) {
      this.CurrentPage = 0;
    } else if (this.CurrentPage < 0) {
      this.CurrentPage = Systems.WarningEntries.Count - 1;
    } else {
      this.CurrentPage++;
    }
  }

  /// <summary>
  /// Increments <see cref="CurrentPage" /> down by one.
  /// </summary>
  private void PreviousPage() {
    if (this.CurrentPage > Systems.WarningEntries.Count) {
      this.CurrentPage = 0;
    } else if (this.CurrentPage < 0) {
      this.CurrentPage = Systems.WarningEntries.Count - 1;
    } else {
      this.CurrentPage--;
    }
  }

  /// <summary>
  /// Handles showing or hiding the <see cref="WarningDialog" /> based on configuration.
  /// </summary>
  public void CheckForRemindersOrHide() {
    CheckForReminders();
    this._isDismissed.Toggle();
  }

  /// <summary>
  /// Handles hiding the <see cref="WarningDialog" /> based on configuration.
  /// </summary>
  public void OnUIHide() {
    this.UIHidden = false;
  }

  /// <summary>
  /// Handles showing the <see cref="WarningDialog" /> based on configuration.
  /// </summary>
  public void OnUIShow() {
    this.UIHidden = true;
  }

  /// <summary>
  /// Handles hiding the <see cref="WarningDialog" />.
  /// </summary>
  public void DismissAll() {
    this._isDismissed.Toggle();
  }

  /// <summary>
  /// Handles hiding the <see cref="WarningDialog" />.
  /// </summary>
  public void DismissCurrent() {
    Systems.WarningEntries[this.CurrentPage]?.Dismiss();
  }

  /// <summary>
  /// Handles showing the <see cref="WarningDialog" />.
  /// </summary>
  public void Restore() {
    this._isDismissed = false;
  }

  /// <summary>
  /// Handles restoring all warning messages.
  /// </summary>
  public void RestoreAll() {
    // TODO: Handle restoring all wrning messages.
  }

  /// <summary>
  /// Handles toggling the repositioning of the <see cref="WarningDialog" />.
  /// </summary>
  public void ToggleRepositioning() {
    this._isRepositioning.Toggle();
  }

  /// <summary>
  /// Handles checking for reminders the <see cref="WarningDialog" />.
  /// </summary>
  /// <returns><see langword="true" /> if reminders found; otherwise <see langword="false" />.</returns>
  public void CheckForReminders() {
    if (Systems.IsLoggedIn && A(out PlayerConfigEntry? b) && B(out PlayerConfig? c)) {
      CheckForReminders(b, c);
    }

    if (Plugin.Systems.Config.Global.ShowForAllPlayers) {
      Plugin.Systems.PlayerManager.Each(CheckForReminders);
    }

    return;

    static bool A([NotNullWhen(true)] out PlayerConfigEntry? b) {
      b = null;
      return Systems.IsLoggedIn && Plugin.Systems.Config.PlayerEntries.TryGetValue(Systems.CurrentPlayerID.Value, out b);
    }

    static bool B([NotNullWhen(true)] out PlayerConfig? c) {
      if (!Systems.IsLoggedIn) {
        c = null;
        return false;
      }

      c = Plugin.Systems.PlayerManager.GetPlayerConfig(Systems.CurrentPlayerID.Value, Systems.CurrentPlayer);
      // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
      return c is not null;
    }
  }

  /// <summary>
  /// Handles checking for reminders the <see cref="WarningDialog" />.
  /// </summary>
  /// <returns><see langword="true" /> if reminders found; otherwise <see langword="false" />.</returns>
  internal void CheckForReminders(PlayerConfigEntry configEntry, PlayerConfig playerConfig) {
    if (playerConfig.FreeCompanyEstate.GetDaysMissed() is var daysMissedFC && daysMissedFC >= Plugin.Systems.Config.Global.DaysToWait) {
      Systems.WarningEntries.Add(new WarningEntry(configEntry, HousingType.FreeCompanyEstate, daysMissedFC));
    }

    if (playerConfig.PrivateEstate.GetDaysMissed() is var daysMissedP && daysMissedP >= Plugin.Systems.Config.Global.DaysToWait) {
      Systems.WarningEntries.Add(new WarningEntry(configEntry, HousingType.PrivateEstate, daysMissedP));
    }

    // ReSharper disable once InvertIf
    if (playerConfig.Apartment.GetDaysMissed() is var daysMissedA && daysMissedA >= Plugin.Systems.Config.Global.DaysToWait) {
      Systems.WarningEntries.Add(new WarningEntry(configEntry, HousingType.Apartment, daysMissedA));
    }
  }

  /// <inheritdoc cref="IDisposable.Dispose" />
  public void Dispose() {
    GC.SuppressFinalize(this);
  }
}
