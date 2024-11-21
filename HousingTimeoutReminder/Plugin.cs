using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Command;
using Dalamud.Plugin;

using ECommons;
using ECommons.DalamudServices;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;

// Ignore Spelling: htimeout

/// <summary>
/// The Dalamud Plugin Library
/// </summary>
public class Plugin : IDalamudPlugin {
  /// <summary>
  /// The name of the plugin.
  /// </summary>
  internal static string StaticName = "Housing Timeout Reminder";

  /// <summary>
  /// The name of the plugin.
  /// </summary>
  public string Name => StaticName;

  /// <summary>
  /// The plugin's main command name.
  /// </summary>
  private const string CommandName = "/htimeout";

  /// <summary>
  /// Bool to test if repositioning the warning dialog.
  /// </summary>
  internal bool Repositioning { get; set; }

  /// <summary>
  /// The Dalamud Plugin constructor
  /// </summary>
  /// <param name="pluginInterface">Argument passed by Dalamud</param>
  public Plugin(IDalamudPluginInterface pluginInterface) {
    System.PluginInstance = this;

    ECommonsMain.Init(pluginInterface, System.PluginInstance, Module.All);

    System.Init();

    Svc.Commands.AddHandler(CommandName, new CommandInfo(OnCommand) {
      HelpMessage = "The config menu for the housing timer reminder plugin."
    });

    Svc.PluginInterface.UiBuilder.Draw += DrawUI;
    Svc.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    Svc.ClientState.TerritoryChanged += OnTerritoryChanged;
    Svc.ClientState.Login += OnLogin;
    // ReSharper disable once InvertIf
    if (Svc.ClientState.IsLoggedIn) {
      OnLogin();
      OnTerritoryChanged(Svc.ClientState.TerritoryType);
    }
  }

  /// <summary>
  /// Dispose of the Dalamud Plugin safely.
  /// </summary>
  public void Dispose() {
    System.PluginConfig.Save();
    System.WindowSystem.RemoveAllWindows();
    System.WarningUI.Dispose();
    System.SettingsUI.Dispose();
#if DEBUG
    System.DebugUI.Dispose();
#endif
    Svc.PluginInterface.UiBuilder.Draw -= DrawUI;
    Svc.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
    Svc.ClientState.TerritoryChanged -= OnTerritoryChanged;
    Svc.ClientState.Login -= OnLogin;
    Svc.ClientState.Logout -= OnLogout;
    Svc.Commands.RemoveHandler(CommandName);
    ECommonsMain.Dispose();
    GC.SuppressFinalize(this);
  }

  /// <summary>
  /// Checks the timers when necessary.
  /// </summary>
  [SuppressMessage("Performance", "CA1822:Mark members as static")]
  internal void CheckTimers(ushort? territory = null) {
    if (territory is null && System.IsLoggedIn) {
      territory = Svc.ClientState.TerritoryType;
    }

    if (territory is null) {
      return;
    }

    HousingTimer.OnTerritoryChanged(territory.Value);
  }

  internal bool IsWarningToBeDisplayed(ushort? territory = null) {
    if (territory is null && System.IsLoggedIn) {
      territory = Svc.ClientState.TerritoryType;
    }

    if (territory is null) {
      return false;
    }

    return System.PluginConfig.PlayerConfigs.Any(playerConfig => playerConfig.PlayerID is not null && HousingTimer.ManualCheck(playerConfig.PlayerID, territory.Value) && (playerConfig.IsLate(HousingType.FreeCompanyEstate) || playerConfig.IsLate(HousingType.PrivateEstate) || playerConfig.IsLate(HousingType.Apartment)));
  }

  /// <summary>
  /// The function to call when changing instance. Checks timers after.
  /// </summary>
  /// <param name="e">The territory ID as a ushort.</param>
  private void OnTerritoryChanged(ushort e) {
    CheckTimers(e);
  }

  /// <summary>
  /// The function to call when logging in.
  /// Creates plugin config for player if it doesn't exist.
  /// Then loads the housing timer.
  /// </summary>
  private static void OnLogin() {
    // Services.Config.TryUpdateBrokenNames();
    if (Svc.ClientState.LocalPlayer is not null && Config.GetCurrentPlayerConfig() is null) {
      System.PluginConfig.PlayerConfigs.Add(new PerPlayerConfig {
        PlayerID = new PlayerID(Svc.ClientState.LocalPlayer)
      });
    }
  }

  /// <summary>
  /// The function to call when logging out.
  /// Unloads the housing timer.
  /// </summary>
  private static void OnLogout(int type, int code) {
    // Do nothing for now.
  }

  /// <summary>
  /// In response to the slash command, just display our main UI
  /// </summary>
  /// <param name="command">The Command Name</param>
  /// <param name="args">The Arguments</param>
  private void OnCommand(string command, string args) {
    var argsParsed = !string.IsNullOrEmpty(args) ? args.Split(" ", StringSplitOptions.RemoveEmptyEntries).AsEnumerable().Last().ToLower() : string.Empty;
    if (argsParsed.Equals("check")) {
      CheckTimers();
    } else {
      System.SettingsUI.IsOpen ^= true;
    }
  }

  /// <summary>
  /// Draws the UI of the plugin.
  /// </summary>
  private void DrawUI() {
    System.WindowSystem.Draw();
    if (Repositioning) {
      System.WarningUI.IsOpen = true;
    } else if (IsWarningToBeDisplayed()) {
      System.WarningUI.IsOpen = true;
    } else {
      System.WarningUI.IsOpen = false;
    }
  }

  /// <summary>
  /// Draws the UI of the settings menu of the plugin.
  /// </summary>
  [SuppressMessage("Performance", "CA1822:Mark members as static")]
  public void DrawConfigUI() {
    System.SettingsUI.IsOpen = true;
  }
}
