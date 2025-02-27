using System.IO;
using Newtonsoft.Json;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;

public partial class Config {
  public static PluginConfig LoadConfig() {
    PluginConfig? config = null;
    // ReSharper disable once InvertIf
    if (Svc.PluginInterface.Manifest.InternalName.EndsWith('2') && Path.GetDirectoryName(Plugin.PluginConfigDirectory) is string path) {
      try {
        string configPath = Path.Combine(path, Svc.PluginInterface.Manifest.InternalName.TrimEnd('2').Append(".json"));
        Svc.Log.Verbose("Attempting to load config from path, {0}", configPath);
        // ReSharper disable once InvertIf
        if (File.Exists(configPath)) {
          using FileStream fr = File.OpenRead(configPath);
          using var sr = new StreamReader(fr);
          string json = sr.ReadToEnd();
          json = json.Replace(Plugin.InternalName.Append("\",").Prepend(", "), Svc.PluginInterface.Manifest.InternalName.Append("\",").Prepend(", "));
          config = JsonConvert.DeserializeObject<PluginConfig>(json, new JsonSerializerSettings {
            Error = (object? _, ErrorEventArgs args) => {
              Svc.Log.Error(args.ErrorContext.Error, "Failed to parse config at path {0}", configPath);
            },
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            TypeNameHandling = TypeNameHandling.None,
          });
        }
      } catch (Exception exception) {
        Svc.Log.Error(exception, "Failed to parse config.");
      }
    }

    Svc.Log.Verbose("Config is null {0}.", config is null);
    config ??= Svc.PluginInterface.GetPluginConfig() as PluginConfig;
    Svc.Log.Verbose("Config is null {0}.", config is null);
    config ??= new PluginConfig();
    Svc.Log.Verbose("Config version Prior v{0}.", config.Version);
    Svc.Log.Verbose("Config PlayerConfigs Count Prior {0}.", config.PlayerConfigs?.Count.ToString() ?? "null");
    ConfigVersionManager.DoMigration(config);
    Svc.Log.Verbose("Config PlayerConfigs Count After {0}.", config.PlayerConfigs?.Count.ToString() ?? "null");
    Svc.Log.Verbose("Config version After v{0}.", config.Version);
    SaveConfig(config);
    return config;
  }

  public static void SaveConfig(PluginConfig config) {
    try {
      // ReSharper disable once InvertIf
      if (Svc.PluginInterface.Manifest.InternalName.EndsWith('2') && Path.GetDirectoryName(Plugin.PluginConfigDirectory) is string path) {
        string configPath = Path.Combine(path, Plugin.InternalName.Append(".json"));

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (File.Exists(configPath)) {
          Svc.Log.Verbose("Writing config to already existing path, {0}", configPath);
          WriteFile(configPath, FileMode.Truncate);
        } else {
          Svc.Log.Verbose("Writing config to new path, {0}", configPath);
          WriteFile(configPath, FileMode.Create);
        }
      } else {
        Svc.PluginInterface.SavePluginConfig(config);
      }
    } catch (Exception exception) {
      Svc.Log.Error(exception, "Failed to serialize config to path.");
    }

    Svc.Log.Verbose("Saved config.");

    return;

    void WriteFile(string configPath, FileMode mode) {
      using FileStream fr = File.Open(configPath, mode);
      using var sw = new StreamWriter(fr);

      string json = JsonConvert.SerializeObject(config, Formatting.Indented, new JsonSerializerSettings {
        Error = (object? _, ErrorEventArgs args) => {
          Svc.Log.Error(args.ErrorContext.Error, "Failed to serialize config to path, {0}", configPath);
        },
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        TypeNameHandling = TypeNameHandling.Objects,
      });

      json = json.Replace(Svc.PluginInterface.Manifest.InternalName, Plugin.InternalName);
      sw.Write(json);
    }
  }
}
