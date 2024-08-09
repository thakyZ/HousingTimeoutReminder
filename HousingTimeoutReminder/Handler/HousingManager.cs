using System;
using CSHousingManager = FFXIVClientStructs.FFXIV.Client.Game.HousingManager;
namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

public sealed class HousingManager {
  public static HousingManager GetCurrentLoc() {
    HousingManager output;
    try {
      unsafe {
        var manager = CSHousingManager.Instance();
        var currentTerritory = manager->CurrentTerritory;
        var isInside = manager->IsInside();
        var room = manager->GetCurrentRoom();
        var plot = manager->GetCurrentPlot();
        var isApartment = isInside && plot <= -127;
        var ward = manager->GetCurrentWard();
        var division = manager->GetCurrentDivision();
        output = new HousingManager(isInside, isApartment, room, plot + 1, ward + 1, division);
      }
    } catch (Exception ex) {
      Services.Log.Error(ex, "Failed to get current housing location.");
      output = GetBlank();
    }
    return output;
  }
  public bool IsInside { get; }
  public bool IsApartment { get; }
  public int ApartmentWing => IsApartment ? (Plot == -127 ? 1 : (Plot == -126 ? 2 : 0)) : 0;
  public int Room { get; }
  public int Plot { get; }
  public int Ward { get; }
  public uint Division { get; }
  private HousingManager(bool isInside, bool isApartment, int room, int plot, int ward, uint division) {
    this.IsInside = isInside;
    this.IsApartment = isApartment;
    this.Room = room;
    this.Plot = plot;
    this.Ward = ward;
    this.Division = division;
  }
  private static HousingManager GetBlank() {
    return new HousingManager(false, false, 0, 0, 0, 0);
  }
}
