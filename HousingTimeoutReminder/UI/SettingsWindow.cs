using System.Linq;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data.Houses;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions.ImGui;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;

public class SettingsWindow : Window, IDisposable {
  /// <summary>
  /// Defines the date format to display.
  /// TODO: make this customizable.
  /// </summary>
  private const string _DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

  /// <summary>
  /// Gets a <see langword="string" /> that represents the name of with <see cref="Window" />.
  /// </summary>
  private static string SettingsWindowName => $"{Plugin.Name} Settings###{Plugin.InternalName}-{nameof(SettingsWindow)}";

  /// <inheritdoc />
  public SettingsWindow() : base(SettingsWindowName, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse) {
    this.Size = ImGuiHelpers.ScaledVector2(630.0f, 550.0f);
  }

  /// <summary>
  /// Gets a <see langword="float" /> that represents the units of <code>5.0f</code> scaled by the global <see cref="ImGui" /> scale.
  /// </summary>
  private static float ScaledIndent5 => ImGuiHelpers.ScaledVector2(5.0f).X;

  /// <summary>
  /// Gets the current max width available in the area the cursor is at.
  /// </summary>
  /// <returns>A <see langword="float" /> that represents the max available width.</returns>
  private float GetMaxWidth() {
    var windowSize = 128f;
    if (this.Size.HasValue) {
      windowSize = this.Size.Value.X - ImGui.GetStyle().WindowPadding.X;
    }

    // ImGuiHelpers.ScaledVector2(windowSize, 0).X;
    return windowSize;
  }

  /// <summary>
  /// Draw the housing config dropdown based on a <see cref="HousingType"/>
  /// </summary>
  /// <param name="playerConfig">The player config to check against.</param>
  /// <param name="entry">The player config entry to check against.</param>
  /// <param name="type">The type of housing to check.</param>
  /// <param name="indent">Indenting check to make the child object better formatted.</param>
  public bool DrawHousingDropDown(PlayerConfig playerConfig, PlayerConfigEntry entry, HousingType type, float indent = 0f) {
    try {
      string playerId = entry.DisplayName;
      // ReSharper disable once SuggestVarOrType_SimpleTypes
      using var header = ImRaii.TreeNode($"{type.GetName()}##{type}-{playerId}", ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.NoTreePushOnOpen);

      if (header.Success) {
        // ReSharper disable once SuggestVarOrType_SimpleTypes
        using var child = ImRaii.Child($"##{type}-Child-{playerId}", new Vector2(this.GetMaxWidth() - 10f - indent, 106f * ImGuiHelpers.GlobalScale), true);

        if (child.Success) {
          if (playerConfig[type].IsValid) {
            ImGuiRaii.ColoredLabel("Your last visit was on: ", playerConfig[type].GetLastVisit().ToString(_DATE_FORMAT), (string value) => {
              if (value.Equals("never", StringComparison.OrdinalIgnoreCase)) {
                return ImGuiColors.DalamudRed;
              }

              return ImGuiRaii.GetImGuiColor(ImGuiCol.Text);
            });
            ImGuiRaii.VerticalSeparator();
            ImGuiRaii.ColoredLabel("Your next visit is on: ", playerConfig[type].GetNextVisit().ToString(_DATE_FORMAT), (string value) => {
              if (value.Equals("now", StringComparison.OrdinalIgnoreCase)) {
                return ImGuiColors.DalamudRed;
              }

              return ImGuiRaii.GetImGuiColor(ImGuiCol.Text);
            });
          } else {
            ImGui.Text($"No {type.GetName().ToLower()} set.");
          }

          HouseBase playerHousing = playerConfig[type];

          ImGui.Text("Enabled");
          ImGui.SameLine();

          bool enabled = playerHousing.Enabled;

          if (ImGui.Checkbox($"##{type}Enabled-{playerId}", ref enabled)) {
            playerHousing.Enabled = enabled;
          }

          ImGuiRaii.VerticalSeparator();

          if (ImGui.Button($"Restore Notifications##{type}Reset-{playerId}")) {

          }

          ImGui.Text("District");
          ImGui.SameLine();

          // ReSharper disable once SuggestVarOrType_SimpleTypes
          using (var i = ImRaii.ItemWidth(100)) {
            if (i.Success) {
              // ReSharper disable once SuggestVarOrType_SimpleTypes
              using var c = ImRaii.Combo($"##{type}District-{playerId}", playerHousing.District.GetName());

              if (c.Success) {
                foreach ((string name, District district) in DistrictEnumHelper.GetNames()) {
                  if (ImGui.Selectable(name, district == playerHousing.District)) {
                    playerHousing.District = district;
                  }
                }
              }
            }
          }

          if (type == HousingType.Apartment) {
            ImGuiRaii.VerticalSeparator();
            ImGui.Text("Is SubDistrict");
            ImGui.SameLine();

            bool isSubDistrict = playerHousing.IsSubDistrict;

            if (ImGui.Checkbox($"##{type}IsSubDistrict-{playerId}", ref isSubDistrict)) {
              playerHousing.Plot = (sbyte)(isSubDistrict ? -127 : -126);
            }
          }

          ImGui.Text("Ward");
          ImGui.SameLine();
          // ReSharper disable once SuggestVarOrType_SimpleTypes
          using (var i = ImRaii.ItemWidth(100)) {
            if (i.Success) {
              int ward = playerHousing.Ward;

              // ReSharper disable once InvertIf
              if (ImGuiRaii.InputIntMinMax($"##{type}Ward-{playerId}", ref ward, 1, 20, 1, HouseBase.WARD_MAX)) {
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
            if (ImGuiRaii.InputIntMinMax($"##{type}RoomNumber-{playerId}", ref apartmentNumber, 1, 20, 1, HouseBase.APARTMENT_MAX)) {
              playerHousing.Room = (sbyte)apartmentNumber;
            }
          } else {
            ImGui.Text("Plot");
            ImGui.SameLine();

            // ReSharper disable once SuggestVarOrType_SimpleTypes
            using var i = ImRaii.ItemWidth(100);

            if (i.Success) {
              int plot = playerHousing.Plot;
              // ReSharper disable once InvertIf
              if (ImGuiRaii.InputIntMinMax($"##{type}Plot-{playerId}", ref plot, 1, 20, 1, HouseBase.PLOT_MAX)) {
                playerHousing.Plot = (sbyte)plot;
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
  /// <param name="entry">The player config entry involved with the single player.</param>
  /// <param name="indent">Indenting check to make the child object better formatted.</param>
  private void DrawUserTimeoutSettings(PlayerConfig playerConfig, PlayerConfigEntry entry, float indent = 0f) {
    string playerId = entry.DisplayName;
    if (!DrawHousingDropDown(playerConfig, entry, HousingType.FreeCompanyEstate, indent + ScaledIndent5)) {
      ImGui.Text($"Failed to draw the {HousingType.FreeCompanyEstate.GetName().ToLower()} tab.");
    }

    if (!DrawHousingDropDown(playerConfig, entry, HousingType.PrivateEstate, indent + ScaledIndent5)) {
      ImGui.Text($"Failed to draw the {HousingType.PrivateEstate.GetName().ToLower()} tab.");
    }

    if (!DrawHousingDropDown(playerConfig, entry, HousingType.Apartment, indent + ScaledIndent5)) {
      ImGui.Text($"Failed to draw the {HousingType.Apartment.GetName().ToLower()} tab.");
    }

    // ReSharper disable once InvertIf
    if (ImGui.Button($"Restore Notifications##Reset-{playerId}")) {
      Plugin.Systems.CheckForRemindersForPlayer(entry, playerConfig);
    }
  }

  private bool DrawGlobalTab() {
    try {
      // ReSharper disable once SuggestVarOrType_SimpleTypes
      using var scrolling = ImRaii.Child($"scrolling##HousingTimeoutReminder-{nameof(DrawGlobalTab)}", ImGuiHelpers.ScaledVector2(0, -(25 + ImGui.GetStyle().ItemSpacing.Y)), false);

      if (!scrolling.Success) {
        return true;
      }
      ImGui.Text("Days To Wait");
      ImGui.SameLine();
      // ReSharper disable once SuggestVarOrType_SimpleTypes
      using (var i = ImRaii.ItemWidth(100)) {
        if (i.Success) {
          int daysToWait = Plugin.Systems.Config.Global.DaysToWait;

          if (ImGuiRaii.InputIntMinMax("##GlobalDaysToWait", ref daysToWait, 1, 5, 1, 30)) {
            Plugin.Systems.Config.Global.DaysToWait = (ushort)daysToWait;
          }
        }
      }

      bool displayAllPlayers = Plugin.Systems.Config.Global.ShowForAllPlayers;

      if (ImGui.Checkbox("Show Timeouts For All Players##ShowForAllPlayers", ref displayAllPlayers)) {
        Plugin.Systems.Config.Global.ShowForAllPlayers = displayAllPlayers;
      }
    } catch (Exception ex) {
      Svc.Log.Error(ex, nameof(DrawGlobalTab));
      return false;
    }
    return true;
  }

  private bool DrawCurrentPlayerTab() {
    try {
      // ReSharper disable once SuggestVarOrType_SimpleTypes
      using var scrolling = ImRaii.Child($"scrolling##HousingTimeoutReminder-{nameof(DrawCurrentPlayerTab)}", ImGuiHelpers.ScaledVector2(0, -(25 + ImGui.GetStyle().ItemSpacing.Y)), false);

      if (!scrolling.Success) {
        return true;
      }

      if (Systems.IsLoggedIn && Plugin.Systems.PlayerManager.TryGetCurrentPlayerConfigAndEntry(out PlayerConfig? playerConfig, out PlayerConfigEntry? entry)) {
        ImGui.Text($"Housing Configuration for {entry.DisplayName}:");
        DrawUserTimeoutSettings(playerConfig, entry);
      }
    } catch (Exception ex) {
      Svc.Log.Error(ex, nameof(DrawCurrentPlayerTab));
      return false;
    }
    return true;
  }

  private bool DrawOtherPlayersTab() {
    try {
      // ReSharper disable once SuggestVarOrType_SimpleTypes
      using var scrolling = ImRaii.Child($"scrolling##HousingTimeoutReminder-{nameof(DrawOtherPlayersTab)}", ImGuiHelpers.ScaledVector2(0, -(25 + ImGui.GetStyle().ItemSpacing.Y)), false);
      if (!scrolling.Success) {
        return true;
      }

#if DEBUG
      ImGui.Text($"Player Configs Count: {Plugin.Systems.Config.PlayerEntries.Count}");
#endif

      // ReSharper disable once SuggestVarOrType_SimpleTypes
      foreach (var entry in Plugin.Systems.Config.PlayerEntries.Where(config => !config.IsCurrentPlayer).Select(x => x.ConfigEntry)) {
        using ImRaii.IEndObject header = ImRaii.TreeNode($"Housing Configuration for {entry.DisplayName}", ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.NoTreePushOnOpen);

        if (!header.Success || Plugin.Systems.PlayerManager.LoadPlayerConfig(entry) is not PlayerConfig playerConfig) {
          continue;
        }

        ImGui.Indent(ScaledIndent5);
        DrawUserTimeoutSettings(playerConfig, entry, ScaledIndent5);
        ImGui.Indent(-ScaledIndent5);
      }
    } catch (Exception ex) {
      Svc.Log.Error(ex, nameof(DrawOtherPlayersTab));
      return false;
    }
    return true;
  }

  /// <inheritdoc />
  public override void Draw() {
    Svc.Log.Verbose("Drawing {0}.", SettingsWindowName);
    try {
      try {
        // ReSharper disable once SuggestVarOrType_SimpleTypes
        using var tabBar = ImRaii.TabBar("ConfigTabs");

        if (tabBar.Success) {
          try {
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            using var tabItem = ImRaii.TabItem("Global");

            if (tabItem.Success && !DrawGlobalTab()) {
              ImGui.Text("Failed to draw global settings tab.");
            }
          } catch {
            ImGui.Text("Failed to draw tab item \"Global\".");
          }

          try {
            if (Systems.IsLoggedIn) {
              // ReSharper disable once SuggestVarOrType_SimpleTypes
              using var tabItem = ImRaii.TabItem("Current Player");

              if (tabItem.Success && !DrawCurrentPlayerTab()) {
                ImGui.Text("Failed to draw current player settings tab.");
              }
            }
          } catch {
            ImGui.Text("Failed to draw tab item \"Global\".");
          }

          try {
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            using var tabItem = ImRaii.TabItem("Other Players");

            if (tabItem.Success && !DrawOtherPlayersTab()) {
              ImGui.Text("Failed to draw other players settings tab.");
            }
          } catch {
            ImGui.Text("Failed to draw tab item \"Global\".");
          }
        }
      } catch {
        ImGui.Text("Failed to draw tab bar.");
      }

      if (ImGui.Button("Save")) {
        PluginConfig.Save();
      }

      ImGui.SameLine();

      if (ImGui.Button("Save and close")) {
        PluginConfig.Save();
        this.IsOpen = false;
      }

      ImGuiRaii.VerticalSeparator();

      if (ImGui.Button("Reposition")) {
        Plugin.Systems.ToggleWarningDialogRepositioning();
      }

      ImGuiRaii.VerticalSeparator();

      if (ImGui.Button("Restore Dismissals")) {
        Plugin.Systems.RestoreAllDismissals();
      }

      ImGuiRaii.VerticalSeparator();

      if (ImGui.Button("Check For Reminders")) {
        Plugin.Systems.CheckForReminders();
      }
    } catch {
      ImGui.Text($"Failed to {nameof(SettingsWindow)}.");
    }
  }

  /// <inheritdoc cref="IDisposable.Dispose" />
  public void Dispose() {
    GC.SuppressFinalize(this);
  }
}
