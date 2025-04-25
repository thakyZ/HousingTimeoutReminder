using System;
using ImGuiNET;

using Newtonsoft.Json;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
public abstract class WardProperty : IWardProperty {
  /// <inheritdoc />
  public virtual short Room { get; set; }

  /// <summary>
  /// The Apartment plot number based off of if the apartment is in a subdistrict.
  /// </summary>
  public virtual sbyte Plot { get; set; }

  /// <inheritdoc />
  public virtual bool Enabled { get; set; }

  /// <inheritdoc />
  public virtual long LastVisit { get; set; }

  /// <inheritdoc />
  public virtual District District { get; set; } = District.Unknown;

  /// <inheritdoc />
  public virtual sbyte Ward { get; set; }

  /// <inheritdoc />
  public abstract bool IsValid { get; }

  /// <inheritdoc />
  public abstract void Reset();

  [JsonIgnore]
  public virtual bool IsDismissed { get; set; }

  /// <summary>
  /// Gets a <see langword="bool" /> that determines if the player is late to visiting their house.
  /// </summary>
  [JsonIgnore]
  public bool IsLate => this.IsValid && this.Enabled && !this.IsDismissed && DateTimeOffset.Now.ToUnixTimeSeconds() - this.LastVisit > System.PluginConfig.DateToWaitSeconds;
}
