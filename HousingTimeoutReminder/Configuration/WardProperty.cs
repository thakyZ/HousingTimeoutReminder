namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;
/// <summary>
/// Interface for housing Properties.
/// </summary>
public interface IWardProperty {
  public abstract bool IsValid();
}

/// <summary>
/// Base class for housing Properties.
/// </summary>
public class WardProperty : IWardProperty {
  /// <summary>
  /// Whether or not this housing plot is enabled.
  /// </summary>
  public bool Enabled { get; set; }
  /// <summary>
  /// The last visit of the player in Unix epoch timestamp format.
  /// </summary>
  public long LastVisit { get; set; }
  /// <summary>
  /// The district the housing location is in.
  /// </summary>
  public District District { get; set; } = District.Unknown;
  /// <summary>
  /// The specific ward the property is in.
  /// </summary>
  public ushort Ward { get; set; }
  /// <summary>
  /// Checks if this property is valid.
  /// </summary>
  /// <returns>Returns <see langword="true"/> if the property is valid.</returns>
  public virtual bool IsValid() { return false; }
}
