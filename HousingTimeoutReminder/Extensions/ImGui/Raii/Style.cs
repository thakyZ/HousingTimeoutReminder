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
  public static Style PushStyle(ImGuiStyleVar idx, float value, bool condition = true)
    => new Style().Push(idx, value, condition);

  /// <inheritdoc cref="ImGuiRaii.PushStyle(ImGuiStyleVar, float, bool)" />
  public static Style PushStyle(ImGuiStyleVar idx, Vector2 value, bool condition = true)
    => new Style().Push(idx, value, condition);

  /// <inheritdoc cref="ImGuiRaii.PushStyle(ImGuiStyleVar, float, bool)" />
  public static Style PushStyle(ImGuiStyleVar idx, bool value, bool condition = true)
    => new Style().Push(idx, value, condition);

  /// <inheritdoc cref="ImGuiRaii.PushStyle(ImGuiStyleVar, float, bool)" />
  public static Style PushStyle(ImGuiStyleVar idx, int value, bool condition = true)
    => new Style().Push(idx, value, condition);


  /// <summary>
  /// An instanced <see cref="IDisposable" /> class to be used when pushing and popping styles safely.
  /// <seealso cref="ImGuiRaii.PushStyle(ImGuiStyleVar, float, bool)"/>
  /// <seealso cref="ImGuiRaii.PushStyle(ImGuiStyleVar, Vector2, bool)"/>
  /// <seealso cref="ImGuiRaii.PushStyle(ImGuiStyleVar, bool, bool)"/>
  /// <seealso cref="ImGuiRaii.PushStyle(ImGuiStyleVar, int, bool)"/>
  /// </summary>
  public class Style : IDisposable {
    /// <summary>
    /// Gets or sets a value related to how many styles were pushed.
    /// </summary>
    private int _count;

    /// <summary>
    /// Tests if the type of the input value is correct for the value of <see cref="ImGuiStyleVar"/>.
    /// </summary>
    /// <param name="idx">The ImGui style variable to test against.</param>
    /// <param name="type">The type of value trying to be applied to the style variable.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If a <see cref="ImGuiStyleVar"/> was not accounted for or for whatever reason the provided style is out of
    /// range (it should never be), this exception will be thrown.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// If the type provided by <paramref name="type" /> is not equal to what should be on the provided
    /// <see cref="ImGuiStyleVar"/>, this exception will be thrown.
    /// </exception>
    [Conditional("DEBUG")]
    private static void CheckStyleIdx(ImGuiStyleVar idx, Type type) {
      bool shouldThrow = idx switch {
        ImGuiStyleVar.Alpha => type != typeof(float),
        ImGuiStyleVar.WindowPadding => type != typeof(Vector2),
        ImGuiStyleVar.WindowRounding => type != typeof(float),
        ImGuiStyleVar.WindowBorderSize => type != typeof(float),
        ImGuiStyleVar.WindowMinSize => type != typeof(Vector2),
        ImGuiStyleVar.WindowTitleAlign => type != typeof(Vector2),
        ImGuiStyleVar.ChildRounding => type != typeof(float),
        ImGuiStyleVar.ChildBorderSize => type != typeof(float),
        ImGuiStyleVar.PopupRounding => type != typeof(float),
        ImGuiStyleVar.PopupBorderSize => type != typeof(float),
        ImGuiStyleVar.FramePadding => type != typeof(Vector2),
        ImGuiStyleVar.FrameRounding => type != typeof(float),
        ImGuiStyleVar.FrameBorderSize => type != typeof(float),
        ImGuiStyleVar.ItemSpacing => type != typeof(Vector2),
        ImGuiStyleVar.ItemInnerSpacing => type != typeof(Vector2),
        ImGuiStyleVar.IndentSpacing => type != typeof(float),
        ImGuiStyleVar.CellPadding => type != typeof(Vector2),
        ImGuiStyleVar.ScrollbarSize => type != typeof(float),
        ImGuiStyleVar.ScrollbarRounding => type != typeof(float),
        ImGuiStyleVar.GrabMinSize => type != typeof(float),
        ImGuiStyleVar.GrabRounding => type != typeof(float),
        ImGuiStyleVar.TabRounding => type != typeof(float),
        ImGuiStyleVar.ButtonTextAlign => type != typeof(Vector2),
        ImGuiStyleVar.SelectableTextAlign => type != typeof(Vector2),
        ImGuiStyleVar.DisabledAlpha => type != typeof(bool),
        ImGuiStyleVar.COUNT => type != typeof(int),
        _ => throw new ArgumentOutOfRangeException(nameof(idx), idx, null),
      };

      if (shouldThrow) {
        throw new ArgumentException($"Unable to push {type} to {idx}.");
      }
    }

    /// <summary>
    /// Pushes a style as an <see cref="IDisposable" />. Should be used with the <see langworld="using" /> statement.
    /// </summary>
    /// <param name="idx">The type of ImGui Style to modify.</param>
    /// <param name="value">The value of the ImGui Style to change.</param>
    /// <param name="condition">Whether to set the value or not.</param>
    /// <returns>An <see cref="IDisposable" /> instance.</returns>
    public Style Push(ImGuiStyleVar idx, float value, bool condition = true) {
      // ReSharper disable once InvertIf
      if (condition) {
        CheckStyleIdx(idx, typeof(float));
        ImGui.PushStyleVar(idx, value);
        ++_count;
      }

      return this;
    }

    /// <inheritdoc cref="ImGuiRaii.Style.Push(ImGuiStyleVar, float, bool)" />
    public Style Push(ImGuiStyleVar idx, Vector2 value, bool condition = true) {
      // ReSharper disable once InvertIf
      if (condition) {
        CheckStyleIdx(idx, typeof(Vector2));
        ImGui.PushStyleVar(idx, value);
        ++_count;
      }

      return this;
    }

    /// <inheritdoc cref="ImGuiRaii.Style.Push(ImGuiStyleVar, float, bool)" />
    public Style Push(ImGuiStyleVar idx, bool value, bool condition = true) {
      // ReSharper disable once InvertIf
      if (condition) {
        CheckStyleIdx(idx, typeof(bool));
        ImGui.PushStyleVar(idx, value.ToFloat());
        ++_count;
      }

      return this;
    }

    /// <inheritdoc cref="ImGuiRaii.Style.Push(ImGuiStyleVar, float, bool)" />
    public Style Push(ImGuiStyleVar idx, int value, bool condition = true) {
      // ReSharper disable once InvertIf
      if (condition) {
        CheckStyleIdx(idx, typeof(bool));
        ImGui.PushStyleVar(idx, value);
        ++_count;
      }

      return this;
    }

    /// <summary>
    /// Pops the last <paramref name="num" /> pushes to the <see cref="Style"/>.
    /// </summary>
    /// <param name="num">The number of styles to pop.</param>
    public void Pop(int num = 1) {
      num = Math.Min(num, _count);
      _count -= num;
      ImGui.PopStyleVar(num);
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
      => Pop(_count);
  }
}