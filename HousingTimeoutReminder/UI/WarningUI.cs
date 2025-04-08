using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
  private Vector2 _oldPostion = Vector2.Zero;

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

    if (_oldPostion.Equals(ImGui.GetWindowPos())) {
      return;
    }

    System.PluginConfig.WarningPosition = Configuration.Position.FromVector2(ImGui.GetWindowPos());
    _oldPostion = ImGui.GetWindowPos();
  }

  /// <summary>
  /// Displays a warning for the player of a <see cref="PerPlayerConfig" />.
  /// </summary>
  /// <param name="type">The type of housing to display.</param>
  /// <param name="playerConfig">An instance of a <see cref="PerPlayerConfig" />.</param>
  [SuppressMessage("Performance", "CA1822:Mark members as static")]
  public void DisplayForPlayer(HousingType type, PerPlayerConfig playerConfig) {
    DateTimeOffset dateTimeOffset = DateTime.Now;
    DateTimeOffset dateTimeOffsetLast;
    int pastDays;
    switch (type)
    {
      case HousingType.FreeCompanyEstate:
        dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.FreeCompanyEstate.LastVisit);
        pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
        ImGui.Text($"Your Free Company Estate has not been visited in, {pastDays} days.");
        ImGui.Text("You can dismiss this at the button below.");
        if (ImGui.Button("Dismiss")) {
          playerConfig.IsDismissed.FreeCompanyEstate = true;
        }

        break;
      case HousingType.PrivateEstate:
        dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.PrivateEstate.LastVisit);
        pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
        ImGui.Text($"Your Private Estate has not been visited in, {pastDays} days.");
        ImGui.Text("You can dismiss this at the button below.");
        if (ImGui.Button("Dismiss")) {
          playerConfig.IsDismissed.PrivateEstate = true;
        }

        break;
      case HousingType.Apartment:
        dateTimeOffsetLast = DateTimeOffset.FromUnixTimeSeconds(playerConfig.Apartment.LastVisit);
        pastDays = (int)dateTimeOffset.Subtract(dateTimeOffsetLast).TotalDays;
        ImGui.Text($"Your Apartment has not been visited in, {pastDays} days.");
        ImGui.Text("You can dismiss this at the button below.");
        if (ImGui.Button("Dismiss")) {
          playerConfig.IsDismissed.Apartment = true;
        }

        break;
      case HousingType.Unknown:
      default:
        this.IsOpen = false;
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
    bool isDismissed = type switch {
      HousingType.FreeCompanyEstate => playerConfig.IsDismissed.FreeCompanyEstate,
      HousingType.PrivateEstate => playerConfig.IsDismissed.PrivateEstate,
      HousingType.Apartment => playerConfig.IsDismissed.Apartment,
      _ => false,
    };

    if (isDismissed) {
      return false;
    }

    this.BgAlpha = 0.5f;
    Flags = WindowFlags;
    Position = Configuration.Position.ToVector2(System.PluginConfig.WarningPosition);
    DisplayForPlayer(type, playerConfig);
    return false;
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
    if (System.PluginInstance.Repositioning) {
      DrawRepositioning();
    } else if (System.IsLoggedIn && Config.PlayerConfiguration is not null) {
      if (playerTypePage == 0) {
        _ = DrawWarning(HousingType.FreeCompanyEstate, Config.PlayerConfiguration);
      } else if (playerTypePage == 1) {
        _ = DrawWarning(HousingType.PrivateEstate, Config.PlayerConfiguration);
      } else if (playerTypePage == 2) {
        _ = DrawWarning(HousingType.Apartment, Config.PlayerConfiguration);
      }
    } else if (System.PluginConfig.ShowAllPlayers) {
      foreach (var config in System.PluginConfig.PlayerConfigs.Where(config => !config.IsCurrentPlayerConfig)) {
        if (playerTypePage == 0) {
          _ = DrawWarning(HousingType.FreeCompanyEstate, config);
        } else if (playerTypePage == 1) {
          _ = DrawWarning(HousingType.PrivateEstate, config);
        } else if (playerTypePage == 2) {
          _ = DrawWarning(HousingType.Apartment, config);
        }
      }
    }

    if (Position.HasValue) {
      Position = null;
    }
  }
}
