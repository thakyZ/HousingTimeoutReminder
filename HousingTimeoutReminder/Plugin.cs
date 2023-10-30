using System;
using System.Linq;

using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
/// <summary>
/// The Dalamud Plugin Library
/// </summary>
public class Plugin : IDalamudPlugin {
  /// <summary>
  /// The name of the plugin.
  /// </summary>
  internal static string StaticName = "Housing Timeout Reminder";

  public string Name => StaticName;

  /// <summary>
  /// The plugin's main command name.
  /// </summary>
  private const string CommandName = "/htimeout";

  /// <summary>
  /// Bool to test if repositioning the warning dialog.
  /// </summary>
  public bool Testing { get; set; }

  /// <summary>
  /// The return booleans if the user hasn't visited their property in the days set.
  /// <see cref="Item1"/>: The bool if player is late for their FC House.
  /// <see cref="Item2"/>: The bool if player is late for their Private House.
  /// <see cref="Item3"/>: The bool if player is late for their Apartment.
  /// </summary>
  public (bool, bool, bool) IsLate { get; set; } = (false, false, false);

  /// <summary>
  /// The return booleans if the user has dismissed the warning for the property.
  /// <see cref="Item1"/>: The bool if player has dismissed warning for their FC House.
  /// <see cref="Item2"/>: The bool if player has dismissed warning for their Private House.
  /// <see cref="Item3"/>: The bool if player has dismissed warning for their Apartment.
  /// </summary>
  public (bool, bool, bool) IsDismissed { get; set; } = (false, false, false);

  /// <summary>
  /// The Dalamud Plugin constructor
  /// </summary>
  /// <param name="pluginInterface">Argument passed by Dalamud</param>
  public Plugin([RequiredVersion("1.0")] DalamudPluginInterface pluginInterface) {
    Services.Init(pluginInterface, this);

    Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
      HelpMessage = "The config menu for the housing timer reminder plugin."
    });

    Services.PluginInterface.UiBuilder.Draw += DrawUI;
    Services.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    Services.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
    Services.ClientState.Login += ClientState_Login;
    Services.ClientState.Logout += ClientState_Logout;
    if (Services.ClientState.IsLoggedIn) {
      ClientState_Login();
    }
    ClientState_TerritoryChanged(Services.ClientState.TerritoryType);
  }

  /// <summary>
  /// Dispose of the Dalamud Plugin safely.
  /// </summary>
  public void Dispose() {
    this.Dispose(true);
    GC.SuppressFinalize(this);
  }

  /// <summary>
  /// Prevent disposing twice.
  /// </summary>
  private bool _isDisposed;

  /// <summary>
  /// Actual dispose method which is safe.
  /// </summary>
  /// <param name="disposing">Affirms your intention to dispose.</param>
  protected virtual void Dispose(bool disposing) {
    if (!_isDisposed && disposing) {
      Services.WindowSystem.RemoveAllWindows();
      Services.WarningUI.Dispose();
      Services.SettingsUI.Dispose();
      Services.XivCommon.Dispose();
      Services.PluginInterface.UiBuilder.Draw -= DrawUI;
      Services.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
      Services.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
      Services.ClientState.Login -= ClientState_Login;
      Services.ClientState.Logout -= ClientState_Logout;
      Services.CommandManager.RemoveHandler(CommandName);
      this._isDisposed = true;
    }
  }

  /// <summary>
  /// Checks to see if the player configuration is disabled.
  /// </summary>
  /// <return>Tuple of FC, Private, Apartment.</return>
  internal (bool, bool, bool) CheckDisabled() {
    if (Configuration.GetPlayerConfiguration() is null) {
      return (false, false, false);
    }
    var _fc = Configuration.GetPlayerConfiguration()!.FreeCompanyEstate.Enabled;
    var _pe = Configuration.GetPlayerConfiguration()!.PrivateEstate.Enabled;
    var _ap = Configuration.GetPlayerConfiguration()!.Apartment.Enabled;
    return (_fc, _pe, _ap);
  }

  /// <summary>
  /// Checks the timers when necessary.
  /// </summary>
  internal void CheckTimers() {
    Services.HousingTimer.ManualCheckAsync().ContinueWith((task) => {
      var check = CheckDisabled();
      if ((IsLate.Item1 && !IsDismissed.Item1 && check.Item1) || (IsLate.Item3 && !IsDismissed.Item3 && check.Item2) || (IsLate.Item3 && !IsDismissed.Item3 && check.Item3)) {
        Services.WarningUI.ResetDismissed();
        Services.WarningUI.IsOpen = true;
      }
    });
  }

  /// <summary>
  /// The function to call when changing instance. Checks timers after.
  /// </summary>
  /// <param name="sender">The object instance of the sender.</param>
  /// <param name="e">The territory ID as a ushort.</param>
  private void ClientState_TerritoryChanged(ushort e) {
    Services.HousingTimer.OnTerritoryChanged(e);
    CheckTimers();
  }

  /// <summary>
  /// The function to call when logging in.
  /// Creates plugin config for player if it doesn't exist.
  /// Then loads the housing timer.
  /// </summary>
  /// <param name="sender">The object instance of the sender.</param>
  /// <param name="e">Random unneeded event args.</param>
  private void ClientState_Login() {
    var playerConfig = Services.Config.PlayerConfigs.Find(x => x.OwnerName == Services.ClientState.LocalPlayer?.Name.TextValue) ?? new PerPlayerConfiguration() { OwnerName = "Unknown" };
    if (playerConfig.OwnerName == "Unknown" && Services.ClientState.LocalPlayer?.Name.TextValue is not null) {
      Services.Config.PlayerConfigs.Add(new PerPlayerConfiguration() {
        OwnerName = Services.ClientState.LocalPlayer?.Name.TextValue!
      });
    }
    if (Services.ClientState.LocalPlayer?.Name.TextValue is not null) {
      Services.HousingTimer.Load();
    }
  }

  /// <summary>
  /// The function to call when logging out.
  /// Unloads the housing timer.
  /// </summary>
  /// <param name="sender">The object instance of the sender.</param>
  /// <param name="e">Random unneeded event args.</param>
  private void ClientState_Logout() {
    Services.HousingTimer.Unload();
  }

  /// <summary>
  /// In response to the slash command, just display our main UI
  /// </summary>
  /// <param name="command">The Command Name</param>
  /// <param name="args">The Arguments</param>
  private void OnCommand(string command, string args) {
    var argsParsed = !string.IsNullOrEmpty(args) ? args.Split(" ", StringSplitOptions.RemoveEmptyEntries).Last().ToLower() : string.Empty;
    if (argsParsed.Equals("check")) {
      CheckTimers();
    } else {
      Services.SettingsUI.IsOpen ^= true;
    }
  }

  /// <summary>
  /// Draws the UI of the plugin.
  /// </summary>
  private void DrawUI() {
    Services.WindowSystem.Draw();
    if (Testing) {
      Services.WarningUI.IsOpen = true;
    }
  }

  /// <summary>
  /// Draws the UI of the settings menu of the plugin.
  /// </summary>
  public void DrawConfigUI() {
    Services.SettingsUI.IsOpen = true;
  }
}
