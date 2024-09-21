using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using ImGuiNET;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions;

[SuppressMessage("Usage", "IDE0022"), SuppressMessage("Usage", "CA1816"), SuppressMessage("ReSharper", "ArrangeMethodOrOperatorBody")]
public static partial class ImGuiRaii
{
  public static Color PushColor( ImGuiCol idx, uint color, bool condition = true )
    => new Color().Push( idx, color, condition );

  public static Color PushColor( ImGuiCol idx, Vector4 color, bool condition = true )
    => new Color().Push( idx, color, condition );

  public class Color : IDisposable
  {
    private int _count;

    public Color Push( ImGuiCol idx, uint color, bool condition = true )
    {
      if( condition )
      {
        ImGui.PushStyleColor( idx, color );
        ++_count;
      }

      return this;
    }

    public Color Push( ImGuiCol idx, Vector4 color, bool condition = true )
    {
      if( condition )
      {
        ImGui.PushStyleColor( idx, color );
        ++_count;
      }

      return this;
    }

    public void Pop( int num = 1 )
    {
      num    =  Math.Min( num, _count );
      _count -= num;
      ImGui.PopStyleColor( num );
    }

    public void Dispose()
      => Pop( _count );
  }
}
