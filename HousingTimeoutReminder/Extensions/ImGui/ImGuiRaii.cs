using System.Numerics;
using Dalamud.Interface.Utility;
using ImGuiNET;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions.ImGui;

// Place this after the namespace to avoid conflict.
using ImGui = ImGuiNET.ImGui;

/// <summary>
/// Extension methods to help with <see cref="ImGuiNET"/> methods.
/// </summary>
public static partial class ImGuiRaii {
  /// <summary>
  /// Creates a vertical separator on the same line.
  /// Borrowed from: <see href="https://github.com/NotNite/DistantSeas/blob/f91788220c6a153a2691331c7d0347cd6930cf3f/DistantSeas/Utils.cs#L66"/>
  /// </summary>
  public static void VerticalSeparator() {
    ImGui.SameLine();

    Vector2 cursor = ImGui.GetCursorPos();
    Vector2 windowPos = ImGui.GetWindowPos();

    ImGui.GetWindowDrawList().AddRectFilled(
      ImGuiHelpers.ScaledVector2(cursor.X, cursor.Y) + windowPos,
      ImGuiHelpers.ScaledVector2(cursor.X + 1, cursor.Y + ImGuiHelpers.GetButtonSize("a").Y) + windowPos,
      ImGui.GetColorU32(ImGuiCol.Separator)
    );

    ImGui.SetCursorPos(ImGuiHelpers.ScaledVector2(cursor.X + 1 + ImGui.GetStyle().ItemSpacing.X, cursor.Y));
  }

  /// <summary>
  /// Makes a label with default font color and a value at the end with a custom color depending on the value of the
  /// parameter <param name="value"/>
  /// </summary>
  /// <param name="label">The label using the default font color.</param>
  /// <param name="value">The colored value part.</param>
  /// <param name="colorFn">Function to determine the color of the value</param>
  public static void ColoredLabel(string label, string value, Func<string, Vector4> colorFn) {
    ImGui.Text(label);
    ImGui.SameLine();
    ImGuiHelpers.SafeTextColoredWrapped(colorFn.Invoke(value), value);
  }

  /// <summary>
  /// Gets a ImGui <see cref="Vector4" /> color from the enum <see cref="ImGuiCol" />.
  /// </summary>
  /// <param name="col">The enum value to convert into a <see cref="Vector4" />.</param>
  /// <returns>Returns a <see cref="Vector4" /> that is equivalent to a <see cref="ImGuiCol" />.</returns>
  public static Vector4 GetImGuiColor(ImGuiCol col) {
    try {
      unsafe {
        var ptr = ImGui.GetStyleColorVec4(col);
        return new Vector4(ptr->X, ptr->Y, ptr->Z, ptr->W);
      }
    } catch {
      // Do nothing.
    }
    return Vector4.One;
  }

  /// <summary>
  /// Creates an input <see cref="ImGui.InputInt(string, ref int, int, int)" /> element with a min and max boundary.
  /// </summary>
  /// <param name="label">The label to apply to the element.</param>
  /// <param name="input">A reference to the value to alter.</param>
  /// <param name="step">A number representing a value to increment by.</param>
  /// <param name="stepFast">A number representing a value to increment by while holding Shift.</param>
  /// <param name="min">A number representing the minimum value.</param>
  /// <param name="max">A number representing the max value.</param>
  /// <returns><see langworld="true" /> if there was a change to the referenced <nameref name="input" /> value, otherwise <see langword="false"/>.</returns>
  public static bool InputIntMinMax(string label, ref int input, int step, int stepFast, int min, int max) {
    // ReSharper disable once InvertIf
    if (ImGui.InputInt(label, ref input, step, stepFast)) {
      if (input > max) {
        input = min;
      }

      if (input < min) {
        input = 1;
      }

      return true;
    }

    return false;
  }
}