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

  /// <summary>
  /// The instanced name of the plugin (required by the inherited class.
  /// </summary>
  /// <inheritdoc />
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
  /// The Dalamud Plugin constructor.
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
  public Plugin(IDalamudPluginInterface pluginInterface) {
    Services.Init(pluginInterface, this);

    Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
      HelpMessage = "The config menu for the housing timer reminder plugin."
    });

    Services.PluginInterface.UiBuilder.Draw += DrawUI;
    Services.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    Services.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
    Services.ClientState.Login += ClientState_Login;
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
#if DEBUG
      Services.DebugUI.Dispose();
#endif
      Services.PluginInterface.UiBuilder.Draw -= DrawUI;
      Services.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
      Services.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
      Services.ClientState.Login -= ClientState_Login;
      Services.CommandManager.RemoveHandler(CommandName);
      this._isDisposed = true;
    }
  }

  /// <summary>
  /// Checks to see if the player configuration house types are named.
  /// </summary>
  /// <return>Tuple of FreeCompany, Private, Apartment.</return>
  internal (bool FreeCompany, bool Private, bool Apartment) CheckEnabled(PerPlayerConfiguration playerConfig) {
    return (playerConfig.FreeCompanyEstate.Enabled, playerConfig.PrivateEstate.Enabled, playerConfig.Apartment.Enabled);
  }

  /// <summary>
  /// Checks the timers when necessary.
  /// </summary>
  internal void CheckTimers() {
    foreach (var playerConfig in Services.Config.PlayerConfigs) {
      Services.HousingTimer.ManualCheckAsync(playerConfig).ContinueWith((task) => {
        var check = CheckEnabled(playerConfig);
        if ((playerConfig.IsLate.FreeCompany && !playerConfig.IsDismissed.FreeCompany && check.FreeCompany) || (playerConfig.IsLate.Private && !playerConfig.IsDismissed.Private && check.Private) || (playerConfig.IsLate.Apartment && !playerConfig.IsDismissed.Apartment && check.Apartment)) {
          Services.WarningUI.ResetDismissed(playerConfig);
          Services.WarningUI.IsOpen = true;
        }
      });
    }
  }

  /// <summary>
  /// The function to call when changing instance. Checks timers after.
  /// </summary>
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
  private void ClientState_Login() {
    // Services.Config.TryUpdateBrokenNames();
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
  /// <param name="_command">The Command Name</param>
  /// <param name="args">The Arguments</param>
  private void OnCommand(string _command, string args) {
    var argsParsed = !string.IsNullOrEmpty(args) ? args.Split(" ", StringSplitOptions.RemoveEmptyEntries).AsEnumerable().Last().ToLower() : string.Empty;
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
