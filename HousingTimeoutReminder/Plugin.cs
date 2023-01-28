using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.IoC;
using System.IO;
using System.Reflection;
using System;

namespace NekoBoiNick.HousingTimeoutReminder {
  public class Plugin : IDalamudPlugin {
    public string Name => "Housing Timeout Reminder";

    private const string CommandName = "/htimeout";

    private PluginUI PluginUI { get; init; }

    public Plugin([RequiredVersion("1.0")] DalamudPluginInterface pluginInterface) {
      pluginInterface.Create<Services>();

      Services.plugin = this;

      Services.pluginConfig = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
      Services.pluginConfig.Initialize(Services.PluginInterface);

      this.PluginUI = new PluginUI(Services.pluginConfig);

      Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
        HelpMessage = "The config menu for the housing timer reminder plugin."
      });

      Services.PluginInterface.UiBuilder.Draw += DrawUI;
      Services.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private bool _isDisposed = false;

    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed && disposing) {
        this.PluginUI.Dispose();
        Services.CommandManager.RemoveHandler(CommandName);
        this._isDisposed = true;
      }
    }

    private void OnCommand(string command, string args) {
      // in response to the slash command, just display our main ui
      this.PluginUI.Visible = true;
    }

    private void DrawUI() {
      this.PluginUI.Draw();
    }

    private void DrawConfigUI() {
      this.PluginUI.SettingsVisible = true;
    }
  }
}
