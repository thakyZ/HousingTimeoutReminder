using System;
using System.Diagnostics.CodeAnalysis;

using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;

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
  /// The name of the window.
  /// </summary>
  public static string Name { get => "Housing Timeout Reminder Warning"; }

  /// <summary>
  /// The default constructor.
  /// </summary>
  public WarningUI() : base(Name, WindowFlags) {
    this.AllowPinning = true;
    this.AllowClickthrough = true;
    this.Size = new Vector2(500, 85) * ImGuiHelpers.GlobalScale;
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
  private Vector2 oldPostion = Vector2.Zero;

  // cSpell:ignore thicc
  /// <summary>
  /// Draw the window a specific way when it needs to be repositioned.
  /// TODO: Make it a clear window with thicc red border or something like that.
  /// </summary>
  public void DrawRepositioning() {
    Flags = ImGuiWindowFlags.NoResize    | ImGuiWindowFlags.NoCollapse        |
            ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse |
            ImGuiWindowFlags.NoTitleBar;

    this.BgAlpha = 0.5f;

    if (oldPostion.Equals(ImGui.GetWindowPos())) {
      return;
    }

    System.PluginConfig.WarningPosition = HousingTimeoutReminder.Position.FromVector2(ImGui.GetWindowPos());
    oldPostion = ImGui.GetWindowPos();
  }

  /// <summary>
  /// Reset all dismissed all dismissed instances in a <see cref="PerPlayerConfig">.
  /// </summary>
  /// <param name="playerConfig">An instance of a <see cref="PerPlayerConfig">.</param>
  [SuppressMessage("Performance", "CA1822:Mark members as static")]
  public void ResetDismissed(PerPlayerConfig playerConfig) {
    playerConfig.IsDismissed.Reset();
  }

  /// <summary>
  /// Displays a warning for the player of a <see cref="PerPlayerConfig">.
  /// </summary>
  /// <param name="type">The type of housing to display.</param>
  /// <param name="playerConfig">An instance of a <see cref="PerPlayerConfig">.</param>
  [SuppressMessage("Performance", "CA1822:Mark members as static")]
  public void DisplayForPlayer(HousingType type, PerPlayerConfig playerConfig) {
    if (type == HousingType.FreeCompanyEstate) {
      var dateTimeOffset = (DateTimeOffset)DateTime.Now;
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.FreeCompanyEstate.LastVisit);
      var pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
      ImGui.Text($"Your Free Company Estate has not been visited in, {pastDays} days.");
      ImGui.Text("You can dismiss this at the button below.");
      if (ImGui.Button("Dismiss")) {
        playerConfig.IsDismissed.FreeCompanyEstate = true;
      }
    } else if (type == HousingType.PrivateEstate) {
      var dateTimeOffset = (DateTimeOffset)DateTime.Now;
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.PrivateEstate.LastVisit);
      var pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
      ImGui.Text($"Your Private Estate has not been visited in, {pastDays} days.");
      ImGui.Text("You can dismiss this at the button below.");
      if (ImGui.Button("Dismiss")) {
        playerConfig.IsDismissed.PrivateEstate = true;
      }
    } else if (type == HousingType.Apartment) {
      var dateTimeOffset = (DateTimeOffset)DateTime.Now;
      var dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.Apartment.LastVisit);
      var pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
      ImGui.Text($"Your Apartment has not been visited in, {pastDays} days.");
      ImGui.Text("You can dismiss this at the button below.");
      if (ImGui.Button("Dismiss")) {
        playerConfig.IsDismissed.Apartment = true;
      }
    }
  }

  /// <summary>
  /// Displays a warning for the player of a <see cref="PerPlayerConfig">.
  /// </summary>
  /// <param name="type">The type of housing to display.</param>
  /// <param name="playerConfig">An instance of a <see cref="PerPlayerConfig">.</param>
  /// <returns><see langword="true"/> if successfully drawn otherwise <see langword="false"/>.</returns>
  public bool DrawWarning(HousingType type, PerPlayerConfig playerConfig) {
    bool state = type switch {
      HousingType.FreeCompanyEstate => playerConfig.IsLate.FreeCompanyEstate && !playerConfig.IsDismissed.FreeCompanyEstate,
      HousingType.PrivateEstate => playerConfig.IsLate.PrivateEstate && !playerConfig.IsDismissed.PrivateEstate,
      HousingType.Apartment => playerConfig.IsLate.Apartment && !playerConfig.IsDismissed.Apartment,
      _ => false
    };

    if (!state) {
      return false;
    }

    this.BgAlpha = 0.5f;
    Flags = WindowFlags;
    Position = HousingTimeoutReminder.Position.ToVector2(System.PluginConfig.WarningPosition);
    DisplayForPlayer(type, playerConfig);
    return true;
  }

  /// <summary>
  /// TODO: Make Pagination Functionality
  /// </summary>
  private int playerPage = 0;

  /// <summary>
  /// TODO: Make Pagination Functionality
  /// </summary>
  private int playerTypePage = 0;

  /// <inheritdoc />
  public override void Draw() {
    if (System.PluginInstance.Testing) {
      DrawRepositioning();
    }

    if (System.IsLoggedIn && Config.GetCurrentPlayerConfig() is PerPlayerConfig playerConfig) {
      if (DrawWarning(HousingType.FreeCompanyEstate, playerConfig)) {
        // Do nothing for now.
      } else if (DrawWarning(HousingType.PrivateEstate, playerConfig)) {
        // Do nothing for now.
      } else if (DrawWarning(HousingType.Apartment, playerConfig)) {
        // Do nothing for now.
      }
    }
    if (Position.HasValue) {
      Position = null;
    }
  }
}
