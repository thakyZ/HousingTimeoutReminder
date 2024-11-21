using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommons.DalamudServices;
using PluginConfig = NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Config.Data.Config;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
internal class Systems : IDisposable {
  internal PluginConfig Config { get; }
  internal Systems() {
    Config = Svc.PluginInterface.GetPluginConfig() as PluginConfig ?? new PluginConfig();
  }

  public void Dispose() {
    PluginConfig.Save();
  }
}
