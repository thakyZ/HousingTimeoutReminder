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
  /// <param name="idx">The type of ImGui style to modify.</param>
  /// <param name="value">The value of the ImGui style to change.</param>
  /// <param name="condition">Whether to set the value or not.</param>
  /// <returns>An <see cref="IDisposable" /> instance.</returns>
  public static TextWrapPos PushTextWrapPos(float wrap_local_pos_x)
    => new TextWrapPos().Push(wrap_local_pos_x);


  /// <summary>
  /// An instanced <see cref="IDisposable" /> class to be used when pushing and popping styles safely.
  /// <seealso cref="ImGuiRaii.PushTextWrapPos(ImGuiStyleVar, float, bool)"/>
  /// </summary>
  public class TextWrapPos : IDisposable {
    /// <summary>
    /// Gets or sets a value related to how many styles were pushed.
    /// </summary>
    private int _count;

    /// <summary>
    /// Pushes a style as an <see cref="IDisposable" />. Should be used with the <see langworld="using" /> statement.
    /// </summary>
    /// <param name="idx">The type of ImGui Style to modify.</param>
    /// <param name="value">The value of the ImGui Style to change.</param>
    /// <param name="condition">Whether to set the value or not.</param>
    /// <returns>An <see cref="IDisposable" /> instance.</returns>
    public TextWrapPos Push(float wrap_local_pos_x) {
      // ReSharper disable once InvertIf
      ImGui.PushTextWrapPos(wrap_local_pos_x);
      ++_count;

      return this;
    }

    /// <summary>
    /// Pops the last <paramref name="num" /> pushes to the <see cref="Style"/>.
    /// </summary>
    /// <param name="num">The number of styles to pop.</param>
    public void Pop(int num = 1) {
      num = Math.Min(num, _count);
      _count -= num;
      for (int i = 0; i < num; i++) {
        ImGui.PopTextWrapPos();
      }
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
      => Pop(_count);
  }
}
