using System.Linq;
using ImGuiNET;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;

internal static class AddPlayerWindow {
  private static string CurrentlySelectedWorldName => currentlySelectedWorld == 0 ? "NONE" : Systems.WorldNameDictionary.FirstOrDefault(x => x.Key == AddPlayerWindow.currentlySelectedWorld).Value;
  private static uint currentlySelectedWorld = 0;
  private static string currentNewPlayerName = string.Empty;
  private static uint errorCode = 0;

  private static bool PlayerNameValidate(string text) {
    var textSplit = text.Split(' ');
    if (textSplit.Length != 2) {
      return false;
    }
    return textSplit.All(c => char.IsAsciiLetterUpper(c[0]) && c.Skip(1).All(b => char.IsAsciiLetterLower(b) || b == '\''));
  }

  public static bool Draw(string windowName, Vector2? size, ref PlayerConfigEntry? newPlayer) {
    if (ImGui.BeginChild(windowName, size ?? Vector2.Zero, true, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking)) {
      string newPlayerName = currentNewPlayerName;
      if (ImGui.InputText("Player Name: ", ref newPlayerName, 100)) {
        if (newPlayerName != currentNewPlayerName) {
          currentNewPlayerName = newPlayerName;
        }
      }
      ImGui.SameLine();
      if (ImGui.BeginCombo("World: ", CurrentlySelectedWorldName)) {

      }
      if (ImGui.Button("Ok")) {
        if (PlayerNameValidate(newPlayerName) && currentlySelectedWorld != 0) {

        }
      }
      ImGui.SameLine();
      if (ImGui.Button("Cancel")) {

      }
      ImGui.EndChild();
    }
  }
}
