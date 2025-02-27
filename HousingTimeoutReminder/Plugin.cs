using System.IO;
using Dalamud.Plugin;
using ECommons;
using Module = ECommons.Module;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class Plugin : IDalamudPlugin {
  public static string Name => Svc.PluginInterface.Manifest.Name;
  public static string InternalName => Svc.PluginInterface.Manifest.InternalName.TrimEnd('2');
  public static Version Version => Svc.PluginInterface.Manifest.AssemblyVersion;
  public static string PluginConfigDirectory {
    get {
      string output = Svc.PluginInterface.GetPluginConfigDirectory();

      // ReSharper disable once InvertIf
      if (Name.EndsWith('2')) {
        try {
          var info = new DirectoryInfo(output);

          if (info.Exists) {
            Directory.Delete(output);
          }

          if (info.Parent is DirectoryInfo parent) {
            output = Path.Combine(parent.FullName, info.Name.TrimEnd('2'));
          } else {
            Directory.Delete(output);
            output = output.Remove(output.EndsWith('\\') || output.EndsWith('/') ? output.Length - 2 : output.Length - 1, 1);
          }
          info = new DirectoryInfo(output);
          if (!info.Exists) {
            info.Create();
          }
        } catch (Exception exception) {
          Svc.Log.Error(exception, "Failed to get the non-development directory of the plugin config directory.");
        }
      }

      return output;
    }
  }

  /// <summary>
  /// A <see langword="bool" /> indicating if the instance of this plugin is disposed already.
  /// </summary>
  private bool _isDisposed;

  /// <summary>
  /// A static instance of this plugin.
  /// <remarks>A non-nullable null value is passes because this should always be applied by the constructor.</remarks>
  /// </summary>
  private static Plugin _instance = null!;

  private readonly Systems _systems;
  /// <summary>
  /// Gets the instance of the <see cref="Systems" /> class.
  /// </summary>
  internal static Systems Systems => _instance._systems;

  public Plugin(IDalamudPluginInterface @interface) {
    _instance = this;
    ECommonsMain.Init(@interface, this, Module.ObjectFunctions);
    Svc.Log?.Verbose("Loading {0}", Name);
    _systems = new Systems();
    this._systems.Config.LoadPlayerConfigs();
  }

  protected virtual void Dispose(bool disposing) {
    // ReSharper disable once InvertIf
    if (!_isDisposed) {
      if (disposing) {
        // NOTE: dispose managed state (managed objects)
      }

      this._systems.Dispose();
      if (!ECommonsMain.Disposed) {
        ECommonsMain.Dispose();
      }

      // NOTE: free unmanaged resources (unmanaged objects) and override finalizer
      // NOTE: set large fields to null
      _isDisposed = true;
    }
  }

  // NOTE: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
  ~Plugin() {
    Svc.Log?.Verbose("Destructing {0}", Name);
    this.Dispose(disposing: false);
  }

  /// <inheritdoc cref="IDisposable.Dispose"/>
  public void Dispose() {
    Svc.Log?.Verbose("Unloading {0}", Name);
    this.Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}
