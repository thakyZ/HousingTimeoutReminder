using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;

using ECommons.DalamudServices;

using ImGuiNET;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;

/// <summary>
/// The class for the settings of the plugin.
/// </summary>
public class SettingsUI : Window, IDisposable {
  /// <summary>
  /// The default window flags of the settings ui.
  /// We disable the scroll with mouse because we have it layed out to have
  /// fixed elements.
  /// </summary>
  private const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar |
    ImGuiWindowFlags.NoScrollWithMouse;

  /// <summary>
  /// The name of the window.
  /// </summary>
  public static string Name => "Housing Timeout Reminder Settings";

  /// <summary>
  /// The default constructor.
  /// </summary>
  public SettingsUI() : base(Name, WindowFlags) {
    Size = ImGuiHelpers.ScaledVector2(630.0f, 550.0f);
    SizeCondition = ImGuiCond.Always;
  }

  /// <summary>
  /// Disposal of this class on unload.
  /// </summary>
  public void Dispose() {
    GC.SuppressFinalize(this);
  }

  /// <summary>
  /// The max number of wards.
  /// TODO: Update ward counts when necessary.
  /// </summary>
  private const int WardMax = 32;

  /// <summary>
  /// The max number of plots.
  /// </summary>
  private const int PlotMax = 60;

  /// <summary>
  /// The max number of apartments.
  /// TODO: Update apartment counts when necessary.
  /// </summary>
  private const int ApartmentMax = 90;

  // ReSharper disable once MemberCanBeMadeStatic.Local
  /// <summary>
  /// A function to shorten the repetitiveness of <see cref="CheckConsistency"/>
  /// Gets the new date time offset required for when the housing plot is outside of the check date.
  /// </summary>
  /// <param name="playerConfig">The player config to check against.</param>
  /// <param name="type">The type of housing to check.</param>
  /// <param name="next">If to check for the next limit date.</param>
  /// <param name="now">If to check for the date from now.</param>
  /// <returns>The offset date from the last visit of <see cref="type"/>.</returns>
  [SuppressMessage("Performance", "CA1822:Mark members as static")]
  private DateTimeOffset ShortenFunction(PerPlayerConfig playerConfig, HousingType type, bool next = false, bool now = false) {
    long visit = type switch {
      HousingType.FreeCompanyEstate => playerConfig.FreeCompanyEstate.LastVisit,
      HousingType.PrivateEstate => playerConfig.PrivateEstate.LastVisit,
      HousingType.Apartment => playerConfig.Apartment.LastVisit,
      _ => 0
    };

    var date = DateTimeOffset.FromUnixTimeSeconds(visit);

    if (next) {
      return date.AddDays(System.PluginConfig.DaysToWait);
    }

    if (now) {
      return DateTimeOffset.Now;
    }

    return date;
  }

  /// <summary>
  /// Gets the new date time offset required for when the housing plot is outside of the check date.
  /// </summary>
  /// <param name="playerConfig">The player config to check against.</param>
  /// <param name="type">The type of housing to check.</param>
  /// <returns>A <see cref="Tuple{String, String}"/> with the last check formatted timestamp,
  /// and the next timelimit formatted timestamp.</returns>
  private (string Last, string Next) CheckConsistency(PerPlayerConfig playerConfig, HousingType type) {
    var lastStamp = ShortenFunction(playerConfig, type);
    var nextStamp = ShortenFunction(playerConfig, type, next: true);

    if (lastStamp.ToUnixTimeSeconds() <= 946627200 && nextStamp.ToUnixTimeSeconds() <= 946627200) {
      return (Last: "Never", Next: "Now");
    }

    if (lastStamp <= nextStamp && nextStamp >= ShortenFunction(playerConfig, type, now: true)) {
      return (Last: $"{lastStamp:yyyy-MM-dd HH:mm:ss}", Next: $"{nextStamp:yyyy-MM-dd HH:mm:ss}");
    }

    if (nextStamp <= ShortenFunction(playerConfig, type, now: true)) {
      return (Last: $"{lastStamp:yyyy-MM-dd HH:mm:ss}", Next: "Now");
    }

    return (Last: "Never", Next: "Now");
  }

  public float GetMaxWidth() {
    var windowSize = 128f;
    if (this.Size.HasValue) {
      windowSize = this.Size.Value.X - ImGui.GetStyle().WindowPadding.X;
    }

    return windowSize;// ImGuiHelpers.ScaledVector2(windowSize, 0).X;
  }

  /// <summary>
  /// Draw the housing config dropdown based on a <see cref="HousingType"/>
  /// </summary>
  /// <param name="playerConfig">The player config to check against.</param>
  /// <param name="type">The type of housing to check.</param>
  /// <param name="indent">Indenting check to make the child object better formatted.</param>
  public bool DrawHousingDropDown(PerPlayerConfig playerConfig, HousingType type, float indent = 0f) {
    try {
      var playerId = playerConfig.DisplayName;
      using (var header = ImRaii.TreeNode($"{HousingTypeEnumHelper.GetDescriptor(type)}##{type}-{playerId}", ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.NoTreePushOnOpen)) {
        if (header.Success) {
          using (var child = ImRaii.Child($"##{type}-Child-{playerId}", new Vector2(this.GetMaxWidth() - 10f - indent, 106f * ImGuiHelpers.GlobalScale), true)) {
            if (child.Success) {
              if (playerConfig.GetOfType(type).IsValid()) {
                (string last, string next) = CheckConsistency(playerConfig, type);

                ImGuiRaii.ColoredLabel("Your last visit was on: ", last, (string value) => {
                  if (value.Equals("never", StringComparison.OrdinalIgnoreCase)) {
                    return ImGuiColors.DalamudRed;
                  }

                  return ImGuiRaii.GetImGuiColor(ImGuiCol.Text);
                });
                ImGuiRaii.VerticalSeparator();
                ImGuiRaii.ColoredLabel("Your next visit is on: ", next, (string value) => {
                  if (value.Equals("now", StringComparison.OrdinalIgnoreCase)) {
                    return ImGuiColors.DalamudRed;
                  }

                  return ImGuiRaii.GetImGuiColor(ImGuiCol.Text);
                });
              } else {
                ImGui.Text($"No {HousingTypeEnumHelper.GetDescriptor(type).ToLower()} set.");
              }

              var playerHousing = playerConfig.GetOfType(type);

              ImGui.Text("Enabled");
              ImGui.SameLine();

              var enabled = playerHousing.Enabled;

              if (ImGui.Checkbox($"##{type}Enabled-{playerId}", ref enabled)) {
                playerHousing.Enabled = enabled;
              }

              ImGuiRaii.VerticalSeparator();

              if (ImGui.Button($"Reset##{type}Reset-{playerId}")) {
                playerHousing.Reset();
              }

              ImGui.Text("District");
              ImGui.SameLine();

              using (var i = ImRaii.ItemWidth(100)) {
                if (i.Success) {
                  using (var c = ImRaii.Combo($"##{type}District-{playerId}", DistrictEnumHelper.GetDescriptor(playerHousing.District))) {
                    if (c.Success) {
                      foreach ((string name, District district) in DistrictEnumHelper.GetDescriptors()) {
                        if (ImGui.Selectable(name, district == playerHousing.District)) {
                          playerHousing.District = district;
                        }
                      }
                    }
                  }
                }
              }

              if (type == HousingType.Apartment) {
                ImGuiRaii.VerticalSeparator();
                ImGui.Text("Is Subdistrict");
                ImGui.SameLine();

                bool isSubdistrict = playerHousing.IsSubdistrict;

                if (ImGui.Checkbox($"##{type}IsSubdistrict-{playerId}", ref isSubdistrict)) {
                  playerHousing.Plot = (sbyte)(isSubdistrict ? -127 : -126);
                }
              }

              ImGui.Text("Ward");
              ImGui.SameLine();
              using (var i = ImRaii.ItemWidth(100)) {
                if (i.Success) {
                  int ward = playerHousing.Ward;

                  // ReSharper disable once InvertIf
                  if (ImGuiRaii.InputIntMinMax($"##{type}Ward-{playerId}", ref ward, 1, 20, 1, WardMax)) {
                    playerHousing.Ward = (sbyte)ward;
                  }
                }
              }

              ImGuiRaii.VerticalSeparator();

              if (type == HousingType.Apartment) {
                ImGui.Text("Room Number");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);

                int apartmentNumber = playerHousing.Room;

                // ReSharper disable once InvertIf
                if (ImGuiRaii.InputIntMinMax($"##{type}RoomNumber-{playerId}", ref apartmentNumber, 1, 20, 1, ApartmentMax)) {
                  playerHousing.Room = (sbyte)apartmentNumber;
                }
              } else {
                ImGui.Text("Plot");
                ImGui.SameLine();

                using (var i = ImRaii.ItemWidth(100)) {
                  if (i.Success) {
                    int plot = playerHousing.Plot;
                    // ReSharper disable once InvertIf
                    if (ImGuiRaii.InputIntMinMax($"##{type}Plot-{playerId}", ref plot, 1, 20, 1, PlotMax)) {
                      playerHousing.Plot = (sbyte)plot;
                    }
                  }
                }
              }
            }
          }
        }
      }
    } catch (Exception ex) {
      Svc.Log.Error(ex, nameof(DrawHousingDropDown));
      return false;
    }
    return true;
  }

  /// <summary>
  /// Draws player timeout settings for a single user.
  /// </summary>
  /// <param name="playerConfig">The player config involved with the single player.</param>
  /// <param name="indent">Indenting check to make the child object better formatted.</param>
  public void DrawUserTimeoutSettings(PerPlayerConfig playerConfig, float indent = 0f) {
    var playerId = playerConfig.DisplayName;
    if (!DrawHousingDropDown(playerConfig, HousingType.FreeCompanyEstate, indent + ScaledIndent5)) {
      ImGui.Text($"Failed to draw {HousingTypeEnumHelper.GetDescriptor(HousingType.FreeCompanyEstate).ToLower()} tab.");
    }

    if (!DrawHousingDropDown(playerConfig, HousingType.PrivateEstate, indent + ScaledIndent5)) {
      ImGui.Text($"Failed to draw {HousingTypeEnumHelper.GetDescriptor(HousingType.PrivateEstate).ToLower()} tab.");
    }

    if (!DrawHousingDropDown(playerConfig, HousingType.Apartment, indent + ScaledIndent5)) {
      ImGui.Text($"Failed to draw {HousingTypeEnumHelper.GetDescriptor(HousingType.Apartment).ToLower()} tab.");
    }

    // ReSharper disable once InvertIf
    if (ImGui.Button($"Reset Notifs##Reset-{playerId}")) {
      System.PluginInstance.CheckTimers();
      playerConfig.IsDismissed.Reset();
    }
  }

  public bool DrawGlobalTab() {
    try {
      using (var scrolling = ImRaii.Child($"scrolling##HousingTimeoutReminder-{nameof(DrawGlobalTab)}", ImGuiHelpers.ScaledVector2(0, -(25 + ImGui.GetStyle().ItemSpacing.Y)), false)) {
        if (!scrolling.Success) {
          return true;
        }
        ImGui.Text("Days To Wait");
        ImGui.SameLine();
        using (var i = ImRaii.ItemWidth(100)) {
          if (i.Success) {
            var daysToWait = (int)System.PluginConfig.DaysToWait;

            if (ImGuiRaii.InputIntMinMax("##GlobalDaysToWait", ref daysToWait, 1, 5, 1, 30)) {
              System.PluginConfig.DaysToWait = (ushort)daysToWait;
            }
          }
        }

        var displayAllPlayers = System.PluginConfig.ShowAllPlayers;

        if (ImGui.Checkbox("Show All Player Timeouts##GlobalShowAllPlayers", ref displayAllPlayers)) {
          System.PluginConfig.ShowAllPlayers = displayAllPlayers;
        }
      }
    } catch (Exception ex) {
      Svc.Log.Error(ex, nameof(DrawGlobalTab));
      return false;
    }
    return true;
  }

  public bool DrawCurrentPlayerTab() {
    try {
      using (var scrolling = ImRaii.Child($"scrolling##HousingTimeoutReminder-{nameof(DrawCurrentPlayerTab)}", ImGuiHelpers.ScaledVector2(0, -(25 + ImGui.GetStyle().ItemSpacing.Y)), false)) {
        if (!scrolling.Success) {
          return true;
        }
        if (System.IsLoggedIn && Config.GetCurrentPlayerConfig() is PerPlayerConfig playerConfig) {
          ImGui.Text($"Housing Configuration for {playerConfig.DisplayName}:");
          DrawUserTimeoutSettings(playerConfig);
        }
      }
    } catch (Exception ex) {
      Svc.Log.Error(ex, nameof(DrawCurrentPlayerTab));
      return false;
    }
    return true;
  }

  public static float ScaledIndent5 => ImGuiHelpers.ScaledVector2(5.0f).X;

  public bool DrawOtherPlayersTab() {
    try {
      using (var scrolling = ImRaii.Child($"scrolling##HousingTimeoutReminder-{nameof(DrawOtherPlayersTab)}", ImGuiHelpers.ScaledVector2(0, -(25 + ImGui.GetStyle().ItemSpacing.Y)), false)) {
        if (!scrolling.Success) {
          return true;
        }

#if DEBUG
        ImGui.Text($"System.PluginConfig.PlayerConfigs.Count: {System.PluginConfig.PlayerConfigs.Count}");
#endif

        foreach (var config in System.PluginConfig.PlayerConfigs.Where(config => !config.IsCurrentPlayerConfig)) {
          using (var header = ImRaii.TreeNode($"Housing Configuration for {config.DisplayName}", ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.NoTreePushOnOpen)) {
            if (header.Success) {
              ImGui.Indent(ScaledIndent5);
              DrawUserTimeoutSettings(config, ScaledIndent5);
              ImGui.Indent(-ScaledIndent5);
            }
          }
        }
      }
    } catch (Exception ex) {
      Svc.Log.Error(ex, nameof(DrawOtherPlayersTab));
      return false;
    }
    return true;
  }

  /// <inheritdoc />
  public override void Draw() {
    using (var tabBar = ImRaii.TabBar("ConfigTabs")) {
      if (tabBar.Success) {
        using (var tabItem = ImRaii.TabItem("Global")) {
          if (tabItem.Success && !DrawGlobalTab()) {
            ImGui.Text("Failed to draw global settings tab.");
          }
        }
        if (System.IsLoggedIn) {
          using (var tabItem = ImRaii.TabItem("Current Player")) {
            if (tabItem.Success && !DrawCurrentPlayerTab()) {
              ImGui.Text("Failed to draw current player settings tab.");
            }
          }
        }
        if (System.PluginConfig.ShowAllPlayers) {
          using (var tabItem = ImRaii.TabItem("Other Players")) {
            if (tabItem.Success && !DrawOtherPlayersTab()) {
              ImGui.Text("Failed to draw other players settings tab.");
            }
          }
        }
      }
    }

    if (ImGui.Button("Save")) {
      HousingTimer.Update();
    }

    ImGui.SameLine();

    if (ImGui.Button("Save and close")) {
      HousingTimer.Update();
      this.IsOpen = false;
    }

    ImGuiRaii.VerticalSeparator();
    ImGui.Text("Reposition");
    ImGui.SameLine();

    var isTesting = System.PluginInstance.Testing;

    if (ImGui.Checkbox("##isTesting", ref isTesting)) {
      System.PluginInstance.Testing = isTesting;
    }

    ImGuiRaii.VerticalSeparator();

    if (ImGui.Button("Reset Notifs")) {
      System.PluginInstance.CheckTimers();
      System.PluginConfig.ResetAll();
    }

#if DEBUG
    ImGuiRaii.VerticalSeparator();

    if (ImGui.Button("Debug")) {
      System.DebugUI.IsOpen = true;
    }
#endif
  }
}
