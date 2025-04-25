using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Common.Lua;

using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;

/// <summary>
/// Class to wrap methods and properties using for Pagination.
/// </summary>
public class Pagination {
  public int CurrentPage { get; set; }
  public int CurrentMainPage { get; set; }
  internal int CurrentValidSubPage { get; set; }
  public HousingType CurrentSubPage {
    get {
      if (this.ValidSubPages.Length == 0) {
        return HousingType.Unknown;
      }

      if (this.CurrentValidSubPage < 0) {
        this.CurrentValidSubPage = this.ValidSubPages.Length - 1;
      }

      if (this.CurrentValidSubPage >= this.ValidSubPages.Length) {
        this.CurrentValidSubPage = 0;
      }

      return this.ValidSubPages[this.CurrentValidSubPage];
    }
  }

  public static int TotalPages => System.PluginConfig.PlayerConfigsWithWarnings
    .Select((PerPlayerConfig playerConfig) => GetValidSubPagesCount(playerConfig).Length) is { } subPageCounts
    && subPageCounts.Any()
    ? subPageCounts.Aggregate((int total, int current) => total + current)
    : 0;

  public static int TotalMainPages => System.PluginConfig.PlayerConfigsWithWarnings.Count;

  public HousingType[] ValidSubPages => this.CurrentPlayerConfig is { } playerConfig ? GetValidSubPagesCount(playerConfig) : [];

  public PerPlayerConfig? CurrentPlayerConfig {
    get {
      try {
        if (TotalMainPages != 0 && this.CurrentMainPage < TotalMainPages) {
          return System.PluginConfig.PlayerConfigsWithWarnings[this.CurrentMainPage];
        }
      } catch {
        // Do nothing...
      }

      return null;
    }
  }

  internal static HousingType[] GetValidSubPagesCount(PerPlayerConfig playerConfig) {
    return [..Internal()];
    IEnumerable<HousingType> Internal() {
      if (playerConfig.FreeCompanyEstate.IsLate)
        yield return HousingType.FreeCompanyEstate;
      if (playerConfig.PrivateEstate.IsLate)
        yield return HousingType.PrivateEstate;
      if (playerConfig.Apartment.IsLate)
        yield return HousingType.Apartment;
    }
  }

  public void WrapPages() {
    if (this.CurrentValidSubPage < 0) {
      this.CurrentValidSubPage = this.ValidSubPages.Length - 1;
    }

    if (this.CurrentValidSubPage >= this.ValidSubPages.Length) {
      this.CurrentValidSubPage = 0;
    }

    if (this.CurrentMainPage < 0) {
      this.CurrentMainPage = TotalMainPages - 1;
    }

    if (this.CurrentMainPage >= TotalMainPages) {
      this.CurrentMainPage = 0;
    }

    if (this.CurrentPage < 0) {
      this.CurrentPage = TotalPages - 1;
    }

    if (this.CurrentPage >= TotalPages) {
      this.CurrentPage = 0;
    }
  }

  public void NextPage() {
    this.CurrentValidSubPage++;

    if (this.CurrentValidSubPage >= this.ValidSubPages.Length) {
      this.CurrentValidSubPage = 0;
      this.CurrentMainPage++;

      if (this.CurrentMainPage >= TotalMainPages) {
        this.CurrentMainPage = 0;
        this.CurrentValidSubPage = 0;
      }
    }

    this.CurrentPage++;

    if (this.CurrentPage >= TotalPages) {
      this.CurrentPage = 0;
    }
  }

  public void PreviousPage() {
    this.CurrentValidSubPage--;

    if (this.CurrentValidSubPage < 0) {
      this.CurrentValidSubPage = this.ValidSubPages.Length - 1;
      this.CurrentMainPage--;

      if (this.CurrentMainPage < 0) {
        this.CurrentMainPage = TotalMainPages - 1;
        this.CurrentValidSubPage = this.ValidSubPages.Length - 1;
      }
    }

    this.CurrentPage--;

    if (this.CurrentPage < 0) {
      this.CurrentPage = TotalPages - 1;
    }
  }
}
