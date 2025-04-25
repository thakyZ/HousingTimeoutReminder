using System;
using System.Numerics;

using Dalamud.Interface.Utility;

using ImGuiNET;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions;

public static partial class ImGuiRaii {
  /// <summary>
  /// Borrowed from: <see href="https://github.com/NotNite/DistantSeas/blob/f91788220c6a153a2691331c7d0347cd6930cf3f/DistantSeas/Utils.cs#L66"/>
  /// </summary>
  public static void VerticalSeparator() {
    ImGui.SameLine();

    var cursor = ImGui.GetCursorPos();
    var windowPos = ImGui.GetWindowPos();

    ImGui.GetWindowDrawList().AddRectFilled(
      ImGuiHelpers.ScaledVector2(cursor.X, cursor.Y) + windowPos,
      ImGuiHelpers.ScaledVector2(cursor.X + 1, cursor.Y + ImGuiHelpers.GetButtonSize("a").Y) + windowPos,
      ImGui.GetColorU32(ImGuiCol.Separator)
    );

    ImGui.SetCursorPos(ImGuiHelpers.ScaledVector2(cursor.X + 1 + ImGui.GetStyle().ItemSpacing.X, cursor.Y));
  }

  /// <summary>
  /// Makes a label with default font color and a value at the end with a custom color
  /// depending on the value of the parameter <param name="value"/>
  /// </summary>
  /// <param name="label">The label using the default font color.</param>
  /// <param name="value">The colored value part.</param>
  /// <param name="colorFn">Function to determine the color of the value</param>
  public static void ColoredLabel(string label, string value, Func<string, Vector4> colorFn) {
    ImGui.Text(label);
    ImGui.SameLine();
    ImGuiHelpers.SafeTextColoredWrapped(colorFn.Invoke(value), value);
  }

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
