using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.IoC;
using System.IO;
using System.Reflection;
using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using Dalamud.Interface.Windowing;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;
using XivCommon;
using Dalamud.Logging;
using System.Threading.Tasks;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder {
  /// <summary>
  /// The Dalamud Plugin Library
  /// </summary>
  public class Plugin : IDalamudPlugin {
    /// <summary>
    /// The name of the pluginb
    /// </summary>
    private static string name = "Housing Timeout Reminder";

    public string Name => name;

    /// <summary>
    /// The plugin's main command name.
    /// </summary>
    private const string CommandName = "/htimeout";

    /// <summary>
    /// The window system of the plugin.
    /// </summary>
    public WindowSystem WindowSystem = new(name.Replace(" ",String.Empty));
    /// <summary>
    /// The ui for warning the player that their house hasn't been visited in a while.
    /// </summary>
    public WarningUI WarningUI = new();
    /// <summary>
    /// The ui for settings of the plugin.
    /// </summary>
    public SettingsUI SettingsUI = new();

    /// <summary>
    /// XIVCommon base instance that allows yhou to get the housing district location.
    /// </summary>
    internal XivCommonBase XivCommon;

    /// <summary>
    /// Bool to test if repositioning the warning dialog.
    /// </summary>
    public bool Testing { get; set; }

    /// <summary>
    /// The return bools if the user hasn't visited their property in the days set.
    /// <see cref="Item1"/>: The bool if player is late for their FC House.
    /// <see cref="Item2"/>: The bool if player is late for their Private House.
    /// <see cref="Item3"/>: The bool if player is late for their Appartment.
    /// </summary>
    public (bool, bool, bool) IsLate { get; set; } = (false, false, false);

    /// <summary>
    /// The return bools if the user has dismissed the warning for the property.
    /// <see cref="Item1"/>: The bool if player has dismissed warning for their FC House.
    /// <see cref="Item2"/>: The bool if player has dismissed warning for their Private House.
    /// <see cref="Item3"/>: The bool if player has dismissed warning for their Appartment.
    /// </summary>
    public (bool, bool, bool) IsDismissed { get; set; } = (false, false, false);

    /// <summary>
    /// The services instance of the plugin.
    /// </summary>
    private Services _services { get; }

    /// <summary>
    /// The Dalamud Plugin constructor
    /// </summary>
    /// <param name="pluginInterface">Argument passed by Dalamud</param>
    public Plugin([RequiredVersion("1.0")] DalamudPluginInterface pluginInterface) {
      _services = pluginInterface.Create<Services>()!;

      Services.plugin = this;

      Services.pluginConfig = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
      Services.pluginConfig.Initialize(Services.PluginInterface);
      XivCommon = new XivCommonBase(Hooks.None);

      Services.housingTimer = new();
      WindowSystem.AddWindow(SettingsUI);
      WindowSystem.AddWindow(WarningUI);

      Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
        HelpMessage = "The config menu for the housing timer reminder plugin."
      });

      Services.PluginInterface.UiBuilder.Draw += DrawUI;
      Services.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
      Services.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
      Services.ClientState.Login += ClientState_Login;
      Services.ClientState.Logout += ClientState_Logout;
      if (Services.ClientState.IsLoggedIn) {
        ClientState_Login(this, EventArgs.Empty);
      }
      ClientState_TerritoryChanged(null, Services.ClientState.TerritoryType);
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
        WindowSystem.RemoveAllWindows();
        WarningUI.Dispose();
        SettingsUI.Dispose();
        XivCommon.Dispose();
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
    /// <return>Trupple of FC, Private, Apartment.</return>
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
      Services.housingTimer.ManualCheck().ContinueWith((task) => {
        var check = CheckDisabled();
        if ((IsLate.Item1 && !IsDismissed.Item1 && check.Item1) || (IsLate.Item3 && !IsDismissed.Item3 && check.Item2) || (IsLate.Item3 && !IsDismissed.Item3 && check.Item3)) {
          WarningUI.ResetDismissed();
          WarningUI.IsOpen = true;
        }
      });
    }

    /// <summary>
    /// The function to call when changing instance. Checks timers after.
    /// </summary>
    /// <param name="sender">The object instance of the sender.</param>
    /// <param name="e">The territory ID as a ushort.</param>
    private void ClientState_TerritoryChanged(object? sender, ushort e) {
      Services.housingTimer.OnTerritoryChanged(e);
      CheckTimers();
    }

    /// <summary>
    /// The function to call when logging in.
    /// Creates plugin config for player if it doesn't exist.
    /// Then loads the housing timer.
    /// </summary>
    /// <param name="sender">The object instance of the sender.</param>
    /// <param name="e">Random unneeded event args.</param>
    private void ClientState_Login(object? sender, EventArgs e) {
      var playerConfig = Services.pluginConfig.PlayerConfigs.Find(x => x.OwnerName == GetCurrentPlayerName()) ?? new PerPlayerConfiguration() { OwnerName = "Unknown" };
      if (playerConfig.OwnerName == "Unknown" && GetCurrentPlayerName() is not null) {
        Services.pluginConfig.PlayerConfigs.Add(new PerPlayerConfiguration() {
          OwnerName = GetCurrentPlayerName()!
        });
      }
      if (GetCurrentPlayerName() is not null) {
        Services.housingTimer.Load();
      }
    }

    /// <summary>
    /// The function to call when logging out.
    /// Unloads the housing timer.
    /// </summary>
    /// <param name="sender">The object instance of the sender.</param>
    /// <param name="e">Random unneeded event args.</param>
    private void ClientState_Logout(object? sender, EventArgs e) {
      Services.housingTimer.Unload();
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
        SettingsUI.IsOpen ^= true;
      }
    }

    /// <summary>
    /// Draws the UI of the plugin.
    /// </summary>
    private void DrawUI() {
      WindowSystem.Draw();
      if (Testing) {
        WarningUI.IsOpen = true;
      }
    }

    /// <summary>
    /// Draws the UI of the settings menu of the plugin.
    /// </summary>
    public void DrawConfigUI() {
      SettingsUI.IsOpen = true;
    }

    /// <summary>
    /// Gets the current player name of the client.
    /// </summary>
    /// <returns>The current player name.</returns>
    public static string? GetCurrentPlayerName() {
      if (Services.ClientState == null || Services.ClientState.LocalPlayer == null || Services.ClientState.LocalPlayer.Name == null) {
        return null;
      }

      return Services.ClientState.LocalPlayer.Name.TextValue;
    }
  }
}
