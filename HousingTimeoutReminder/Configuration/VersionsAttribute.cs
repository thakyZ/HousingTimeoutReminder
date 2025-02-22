namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class VersionsAttribute : Attribute {
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  public int Introduced { get; }
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  public int Removed { get; }

  public VersionsAttribute(int introduced, int removed = 0) {
    this.Introduced = introduced;
    this.Removed = removed;
  }
}