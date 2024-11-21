using System.Xml.Linq;
using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Config.Data;
public class PlayerConfigEntry {
  private string? _name;
  private string? _world;
  public string Name {
    get => _name ?? "INVALID CONFIG";
    set => _name = value;
  }
  public string World {
    get => _world ?? "INVALID CONFIG";
    set => _world = value;
  }
  public string FileName { get; set; } = "";
}
