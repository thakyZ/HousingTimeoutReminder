using ImGuiNET;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions.ImGui;

// Place this after the namespace to avoid conflict.
using ImGui = ImGuiNET.ImGui;

/// <inheritdoc cref="ImGuiRaii" />
public static partial class ImGuiRaii {
  /// <summary>
  /// Pushes a style as an <see cref="IDisposable" />. Should be used with the <see langworld="using" /> statement.
  /// </summary>
  /// <param name="text">The type of ImGui style to modify.</param>
  /// <param name="hovered">The value of the ImGui style to change.</param>
  /// <returns>An <see cref="IDisposable" /> instance.</returns>
  public static bool Tooltip(string text, bool hovered = false) {
    try {
      if(hovered || ImGui.IsItemHovered()) {
        ImGui.BeginTooltip();
        using (var _ = ImGuiRaii.PushTextWrapPos(ImGui.GetFontSize() * 35.0f)) {
          ImGui.TextUnformatted(text);
        }
        ImGui.EndTooltip();
        return true;
      }

      return false;
    } catch (Exception exception) {
      Svc.Log.Error(exception, "Failed to draw tooltip with text, \"{0}\"", text);
    }
    return false;
  }
}
