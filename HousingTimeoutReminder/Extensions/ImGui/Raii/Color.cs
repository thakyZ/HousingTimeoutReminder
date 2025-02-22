using System.Numerics;
using ImGuiNET;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions.ImGui;

// Place this after the namespace to avoid conflict.
using ImGui = ImGuiNET.ImGui;

/// <inheritdoc cref="ImGuiRaii" />
public static partial class ImGuiRaii {
  /// <summary>
  /// Pushes a color as an <see cref="IDisposable" />. Should be used with the <see langworld="using" /> statement.
  /// </summary>
  /// <param name="idx">The type of ImGui color to modify.</param>
  /// <param name="color">The value of the ImGui color to change.</param>
  /// <param name="condition">Whether to set the value or not.</param>
  /// <returns>An <see cref="IDisposable" /> instance.</returns>
  public static Color PushColor(ImGuiCol idx, uint color, bool condition = true)
    => new Color().Push(idx, color, condition);

  /// <inheritdoc cref="ImGuiRaii.PushColor(ImGuiCol, uint, bool)" />
  public static Color PushColor(ImGuiCol idx, Vector4 color, bool condition = true)
    => new Color().Push(idx, color, condition);

  /// <summary>
  /// An instanced <see cref="IDisposable" /> class to be used when pushing and popping colors safely.
  /// <seealso cref="ImGuiRaii.PushColor(ImGuiCol, uint, bool)"/>
  /// <seealso cref="ImGuiRaii.PushColor(ImGuiCol, Vector4, bool)"/>
  /// </summary>
  public class Color : IDisposable {
    /// <summary>
    /// Gets or sets a value related to how many colors were pushed.
    /// </summary>
    private int _count;

    /// <summary>
    /// Pushes a color as an <see cref="IDisposable" />. Should be used with the <see langworld="using" /> statement.
    /// </summary>
    /// <param name="idx">The type of ImGui color to modify.</param>
    /// <param name="color">The value of the ImGui color to change.</param>
    /// <param name="condition">Whether to set the value or not.</param>
    /// <returns>An <see cref="IDisposable" /> instance.</returns>
    public Color Push(ImGuiCol idx, uint color, bool condition = true) {
      // ReSharper disable once InvertIf
      if (condition) {
        ImGui.PushStyleColor(idx, color);
        ++_count;
      }

      return this;
    }

    /// <inheritdoc cref="ImGuiRaii.Color.Push(ImGuiCol, uint, bool)" />
    public Color Push(ImGuiCol idx, Vector4 color, bool condition = true) {
      // ReSharper disable once InvertIf
      if (condition) {
        ImGui.PushStyleColor(idx, color);
        ++_count;
      }

      return this;
    }

    /// <summary>
    /// Pops the last <paramref name="num" /> pushes to the <see cref="Color"/>.
    /// </summary>
    /// <param name="num">The number of colors to pop.</param>
    public void Pop(int num = 1) {
      // ReSharper disable once InvertIf
      num = Math.Min(num, _count);
      _count -= num;
      ImGui.PopStyleColor(num);
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
      => Pop(_count);
  }
}