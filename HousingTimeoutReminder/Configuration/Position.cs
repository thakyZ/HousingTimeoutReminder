using System;

using FFXIVClientStructs.FFXIV.Common.Math;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

/// <summary>
/// The position for a window module from writable to readable.
/// </summary>
[Serializable]
public class Position {
  /// <summary>
  /// The X coordinate on the screen.
  /// </summary>
  public float X { get; set; } = 0;
  /// <summary>
  /// The Y coordinate on the screen.
  /// </summary>
  public float Y { get; set; } = 0;

  /// <summary>
  /// Converts the <see cref="Vector2"/> to a <see cref="Position"/> class.
  /// </summary>
  /// <param name="vector">The <see cref="Vector2"/> to convert.</param>
  /// <returns>Returns <see cref="Position"/> from <see cref="Vector2"/>.</returns>
  public static Position FromVector2(Vector2 vector) {
    return new Position() { X = vector.X, Y = vector.Y };
  }

  /// <summary>
  /// Converts the <see cref="Position"/> to a <see cref="Vector2"/> class.
  /// </summary>
  /// <param name="position">The <see cref="Position"/> to convert.</param>
  /// <returns>Returns <see cref="Vector2"/> from <see cref="Position"/>.</returns>
  public static Vector2 ToVector2(Position position) {
    return new(position.X, position.Y);
  }
}
