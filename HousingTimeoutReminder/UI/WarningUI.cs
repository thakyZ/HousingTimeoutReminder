using System;

using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;

using ECommons.DalamudServices;

using FFXIVClientStructs.FFXIV.Common.Math;

using ImGuiNET;

using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;

/// <summary>
/// The class for the warning ui of the plugin.
/// </summary>
public class WarningUI : Window, IDisposable {
  /// <summary>
  /// The default window flags of the settings ui.
  /// </summary>
  private const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse |
    ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoTitleBar |
    ImGuiWindowFlags.NoMove;

  /// <summary>
  /// An instance of the <see cref="Pagination" /> to implement pages in the warning dialog.
  /// </summary>
  internal readonly Pagination Pagination = new();

  private const float INITIAL_WINDOW_HEIGHT = 85f;

  private float WindowHeight = INITIAL_WINDOW_HEIGHT;

  /// <summary>
  /// The name of the window.
  /// </summary>
  public static string Name { get => "Housing Timeout Reminder Warning"; }

  /// <summary>
  /// The default constructor.
  /// </summary>
  public WarningUI() : base(Name, WindowFlags) {
    this.AllowPinning = true;
    this.AllowClickthrough = true;
    this.Size = new Vector2(500f, INITIAL_WINDOW_HEIGHT) * ImGuiHelpers.GlobalScale;
    this.SizeCondition = ImGuiCond.Always;
  }

  /// <summary>
  /// Disposal of this class on unload.
  /// </summary>
  public void Dispose() {
    GC.SuppressFinalize(this);
  }

  /// <summary>
  /// Caching the old position when repositioning the locked window.
  /// </summary>
  private Vector2 _oldPostion = Vector2.Zero;

  // cSpell:ignore thicc
  /// <summary>
  /// Draw the window a specific way when it needs to be repositioned.
  /// TODO: Make it a clear window with thicc red border or something like that.
  /// </summary>
  public void DrawRepositioning() {
    Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse |
            ImGuiWindowFlags.NoTitleBar;

    this.BgAlpha = 0.5f;

    if (_oldPostion.Equals(ImGui.GetWindowPos())) {
      return;
    }

    System.PluginConfig.WarningPosition = Configuration.Position.FromVector2(ImGui.GetWindowPos());
    _oldPostion = ImGui.GetWindowPos();
  }

  private bool DisplayText(string text) {
    var height = ImGui.CalcTextSize("A").Y;
    var padding = ImGui.GetStyle().WindowPadding.X;
    var wrappedHeight = ImGui.CalcTextSize(text, 0, (this.Size?.X ?? 500f) - (padding * 2)).Y;

    if (WindowHeight != INITIAL_WINDOW_HEIGHT + (wrappedHeight - height)) {
      WindowHeight += wrappedHeight - height;
      this.Size = new Vector2(500f, WindowHeight) * ImGuiHelpers.GlobalScale;
    }

    ImGui.TextWrapped(text);

    if (height == wrappedHeight) {
      ImGui.SetCursorPosY(ImGui.GetCursorPosY() + height);
    }

    ImGui.Text("You can dismiss this at the button below.");
    return ImGui.Button("Dismiss");
  }

  /// <summary>
  /// Displays a warning for the player of a <see cref="PerPlayerConfig" />.
  /// </summary>
  /// <param name="type">The type of housing to display.</param>
  /// <param name="playerConfig">An instance of a <see cref="PerPlayerConfig" />.</param>
  public void DisplayForPlayer(HousingType type, PerPlayerConfig playerConfig) {
    DateTimeOffset dateTimeOffset = DateTime.Now;
    DateTimeOffset dateTimeOffsetLast;
    int pastDays;

    switch (type) {
      case HousingType.FreeCompanyEstate:
        dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.FreeCompanyEstate.LastVisit);
        pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
        if (this.DisplayText($"The Free Company Estate of {playerConfig.DisplayName} has not been visited in, {pastDays} days.")) {
          playerConfig.FreeCompanyEstate.IsDismissed = true;
          this.Pagination.WrapPages();
        }

        break;
      case HousingType.PrivateEstate:
        dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.PrivateEstate.LastVisit);
        pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
        if (this.DisplayText($"The Private Estate of {playerConfig.DisplayName} has not been visited in, {pastDays} days.")) {
          playerConfig.PrivateEstate.IsDismissed = true;
          this.Pagination.WrapPages();
        }

        break;
      case HousingType.Apartment:
        dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.Apartment.LastVisit);
        pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
        if (this.DisplayText($"The Apartment of {playerConfig.DisplayName} has not been visited in, {pastDays} days.")) {
          playerConfig.Apartment.IsDismissed = true;
          this.Pagination.WrapPages();
        }

        break;
    }
  }

  /// <summary>
  /// Displays a warning for the player of a <see cref="PerPlayerConfig" />.
  /// </summary>
  /// <param name="type">The type of housing to display.</param>
  /// <param name="playerConfig">An instance of a <see cref="PerPlayerConfig" />.</param>
  /// <returns><see langword="true"/> if successfully drawn otherwise <see langword="false"/>.</returns>
  public bool DrawWarning(HousingType type, PerPlayerConfig playerConfig) {
    this.BgAlpha = 0.5f;
    Flags = WindowFlags;
    Position = Configuration.Position.ToVector2(System.PluginConfig.WarningPosition);
    DisplayForPlayer(type, playerConfig);
    return false;
  }

  private Vector2 MoveCursorToBottomRight(string text) {
    var pos = ImGui.GetCursorPos();
    if (this.Size.HasValue) {
      var textSize = ImGui.CalcTextSize(text).X;
      var buttonSize = ImGuiHelpers.GetButtonSize("A");
      var padding = ImGui.GetStyle().WindowPadding;
      ImGui.SetCursorPos(new Vector2(this.Size.Value.X - (padding.X * 3) - (buttonSize.X * 2) - textSize, this.Size.Value.Y - (padding.Y * 2) - buttonSize.Y));
    }

    return pos;
  }

  /// <summary>
  /// Code to be executed every time the window renders.
  /// </summary>
  public override void Draw() {
    if (System.PluginInstance.Repositioning) {
      this.DrawRepositioning();
    } else if (System.IsLoggedIn) {
      if (this.Pagination.CurrentPlayerConfig is { } config) {
        if (this.Pagination.CurrentSubPage is HousingType.FreeCompanyEstate) {
          _ = this.DrawWarning(HousingType.FreeCompanyEstate, config);
        } else if (this.Pagination.CurrentSubPage is HousingType.PrivateEstate) {
          _ = this.DrawWarning(HousingType.PrivateEstate, config);
        } else if (this.Pagination.CurrentSubPage is HousingType.Apartment) {
          _ = this.DrawWarning(HousingType.Apartment, config);
        }
      }

      var text = $"{this.Pagination.CurrentPage + 1}/{Pagination.TotalPages}";
      var pos = this.MoveCursorToBottomRight(text);
      ImGui.AlignTextToFramePadding();
      ImGui.Text(text);
      ImGui.SameLine();

      if (ImGui.ArrowButton("Previous##HousingTimeoutReminder.Warning", ImGuiDir.Left)) {
        this.Pagination.PreviousPage();
      }

      ImGui.SameLine();

      if (ImGui.ArrowButton("Next##HousingTimeoutReminder.Warning", ImGuiDir.Right)) {
        this.Pagination.NextPage();
      }

      ImGui.SetCursorPos(pos);
    }

    if (Position.HasValue) {
      Position = null;
    }
  }
}
