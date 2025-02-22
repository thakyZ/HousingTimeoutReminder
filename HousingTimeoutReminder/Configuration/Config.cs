// ReSharper disable once CheckNamespace
namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;

public partial class Config {
  public static Config LoadConfig() {
    Config? config = null;
    // ReSharper disable once InvertIf
    if (Svc.PluginInterface.Manifest.InternalName.EndsWith('2') && System.IO.Path.GetDirectoryName(Plugin.PluginConfigDirectory) is string path) {
      string configPath = System.IO.Path.Combine(path, Svc.PluginInterface.Manifest.InternalName.TrimEnd('2'));
      // ReSharper disable once InvertIf
      if (System.IO.File.Exists(configPath)) {
        using System.IO.FileStream fr = System.IO.File.OpenRead(configPath);
        using var sr = new System.IO.StreamReader(fr);

        config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(sr.ReadToEnd(), new Newtonsoft.Json.JsonSerializerSettings {
          Error = (object? _, Newtonsoft.Json.Serialization.ErrorEventArgs args) => {
            Svc.Log.Error(args.ErrorContext.Error, "Failed to parse config at path, \"{0}\", on line {1}:{2}", configPath, args.ErrorContext.Path);
          },
          TypeNameAssemblyFormatHandling = Newtonsoft.Json.TypeNameAssemblyFormatHandling.Simple,
          TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None,
        });
      }
    }

    config ??= Svc.PluginInterface.GetPluginConfig() as Config;
    config ??= new Config();
    ConfigVersionManager.DoMigration(config);
    SaveConfig(config);
    return config;
  }
}
