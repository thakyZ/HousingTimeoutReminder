using System;

using Dalamud.Plugin;

using ECommons;
using ECommons.DalamudServices;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
public class Plugin : IDalamudPlugin {
  private bool _isDisposed;
  private static Plugin _instance = null!;
  private readonly Systems _systems;
  internal static Systems Systems => _instance._systems;

  public Plugin(IDalamudPluginInterface @interface) {
    _instance = this;
    ECommonsMain.Init(@interface, this, Module.ObjectFunctions);
    _systems = new Systems();
  }

  protected virtual void Dispose(bool disposing) {
    if (!_isDisposed) {
      if (disposing) {
        _systems.Dispose();
        // NOTE: dispose managed state (managed objects)
      }

      // NOTE: free unmanaged resources (unmanaged objects) and override finalizer
      // NOTE: set large fields to null
      _isDisposed = true;
    }
  }

  // // NOTE: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
  // ~Plugin()
  // {
  //     Dispose(disposing: false);
  // }

  public void Dispose() {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}
