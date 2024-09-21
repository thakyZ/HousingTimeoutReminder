using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using ImGuiNET;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions;

[SuppressMessage("Usage", "IDE0022"), SuppressMessage("Usage", "CA1816"), SuppressMessage("ReSharper", "ArrangeMethodOrOperatorBody")]
public static partial class ImGuiRaii
{
  public static Style PushStyle(ImGuiStyleVar idx, float value, bool condition = true)
    => new Style().Push(idx, value, condition);

  public static Style PushStyle(ImGuiStyleVar idx, Vector2 value, bool condition = true)
    => new Style().Push(idx, value, condition);

  public class Style : IDisposable
  {
    private int _count;

    [Conditional("DEBUG")]
    private static void CheckStyleIdx(ImGuiStyleVar idx, Type type)
    {
      var shouldThrow = idx switch
      {
        ImGuiStyleVar.Alpha               => type != typeof(float),
        ImGuiStyleVar.WindowPadding       => type != typeof(Vector2),
        ImGuiStyleVar.WindowRounding      => type != typeof(float),
        ImGuiStyleVar.WindowBorderSize    => type != typeof(float),
        ImGuiStyleVar.WindowMinSize       => type != typeof(Vector2),
        ImGuiStyleVar.WindowTitleAlign    => type != typeof(Vector2),
        ImGuiStyleVar.ChildRounding       => type != typeof(float),
        ImGuiStyleVar.ChildBorderSize     => type != typeof(float),
        ImGuiStyleVar.PopupRounding       => type != typeof(float),
        ImGuiStyleVar.PopupBorderSize     => type != typeof(float),
        ImGuiStyleVar.FramePadding        => type != typeof(Vector2),
        ImGuiStyleVar.FrameRounding       => type != typeof(float),
        ImGuiStyleVar.FrameBorderSize     => type != typeof(float),
        ImGuiStyleVar.ItemSpacing         => type != typeof(Vector2),
        ImGuiStyleVar.ItemInnerSpacing    => type != typeof(Vector2),
        ImGuiStyleVar.IndentSpacing       => type != typeof(float),
        ImGuiStyleVar.CellPadding         => type != typeof(Vector2),
        ImGuiStyleVar.ScrollbarSize       => type != typeof(float),
        ImGuiStyleVar.ScrollbarRounding   => type != typeof(float),
        ImGuiStyleVar.GrabMinSize         => type != typeof(float),
        ImGuiStyleVar.GrabRounding        => type != typeof(float),
        ImGuiStyleVar.TabRounding         => type != typeof(float),
        ImGuiStyleVar.ButtonTextAlign     => type != typeof(Vector2),
        ImGuiStyleVar.SelectableTextAlign => type != typeof(Vector2),
        ImGuiStyleVar.DisabledAlpha       => type != typeof(bool),
        ImGuiStyleVar.COUNT               => type != typeof(int),
        _                                 => throw new ArgumentOutOfRangeException(nameof(idx), idx, null),
      };

      if(shouldThrow)
      {
        throw new ArgumentException($"Unable to push {type} to {idx}.");
      }
    }

    public Style Push(ImGuiStyleVar idx, float value, bool condition = true)
    {
      if(condition)
      {
        CheckStyleIdx(idx, typeof(float));
        ImGui.PushStyleVar(idx, value);
        ++_count;
      }

      return this;
    }

    public Style Push(ImGuiStyleVar idx, Vector2 value, bool condition = true)
    {
      if(condition)
      {
        CheckStyleIdx(idx, typeof(Vector2));
        ImGui.PushStyleVar(idx, value);
        ++_count;
      }

      return this;
    }

    public void Pop(int num = 1)
    {
      num    =  Math.Min(num, _count);
      _count -= num;
      ImGui.PopStyleVar(num);
    }

    public void Dispose()
      => Pop(_count);
  }
}
