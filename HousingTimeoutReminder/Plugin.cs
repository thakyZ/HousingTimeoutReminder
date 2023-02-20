using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.IoC;
using System.IO;
using System.Reflection;
using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using Dalamud.Interface.Windowing;
using HousingTimeoutReminder.UI;
using XivCommon;
using Dalamud.Logging;
using System.Threading.Tasks;

namespace NekoBoiNick.HousingTimeoutReminder {
  /// <summary>
  /// 
  /// </summary>
  public class Plugin : IDalamudPlugin {
    /// <summary>
    /// 
    /// </summary>
    private static string name = "Housing Timeout Reminder";

    public string Name => name;

    /// <summary>
    /// 
    /// </summary>
    private const string CommandName = "/htimeout";

    /// <summary>
    /// 
    /// </summary>
    public WindowSystem WindowSystem = new(name.Replace(" ",String.Empty));
    public WarningUI WarningUI = new();
    public SettingsUI SettingsUI = new();
    
    /// <summary>
    /// 
    /// </summary>
    internal XivCommonBase XivCommon;

    /// <summary>
    /// 
    /// </summary>
    public bool Testing { get; set; } =  false;

    /// <summary>
    /// 
    /// </summary>
    public (bool, bool, bool) IsLate { get; set; } = (false, false, false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pluginInterface"></param>
    public Plugin([RequiredVersion("1.0")] DalamudPluginInterface pluginInterface) {
      pluginInterface.Create<Services>();

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
        ClientState_Login(this, new EventArgs());
      }
      ClientState_TerritoryChanged(null, Services.ClientState.TerritoryType);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private bool _isDisposed = false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed && disposing) {
        WindowSystem.RemoveAllWindows();
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

    internal void CheckTimers() {
      Services.housingTimer.ManualCheck();
      Task.Delay(2000).ContinueWith(t => {
        if (IsLate.Item1 || IsLate.Item2 || IsLate.Item3) {
          WarningUI.ResetDismissed();
          WarningUI.IsOpen = true;
        }
      });
    }

    private void ClientState_TerritoryChanged(object sender, ushort e) {
      Services.housingTimer.OnTerritoryChanged(sender, e);
      CheckTimers();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClientState_Login(object sender, EventArgs e) {
      PerPlayerConfiguration playerConfig = Services.pluginConfig.PlayerConfigs.Find(x => x.OwnerName == GetCurrentPlayerName()) ?? new PerPlayerConfiguration() { OwnerName = "Unknown" };
      if (playerConfig.OwnerName == "Unknown") {
        Services.pluginConfig.PlayerConfigs.Add(new PerPlayerConfiguration() {
          OwnerName = GetCurrentPlayerName()
        });
      }
      Services.housingTimer.Load(GetCurrentPlayerName());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClientState_Logout(object sender, EventArgs e) {
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
    /// 
    /// </summary>
    private void DrawUI() {
      WindowSystem.Draw();
      if (Testing) {
        WarningUI.IsOpen = true;
      }
    }

    public void DrawConfigUI() {
      SettingsUI.IsOpen = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetCurrentPlayerName() {
      if (Services.ClientState == null || Services.ClientState.LocalPlayer == null || Services.ClientState.LocalPlayer.Name == null) {
        return null;
      }

      return Services.ClientState.LocalPlayer.Name.TextValue;
    }
  }
}
