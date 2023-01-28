using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;

using Dalamud.Interface.Windowing;

using NekoBoiNick.HousingTimeoutReminder;

namespace HousingTimeoutReminder.UI {
  public class SettingsUI : Window, IDisposable {
    private readonly Configuration configuration;

    public SettingsUI(Configuration configuration) : base("Housing Timeout Reminder Settings") {
      this.configuration = configuration;
    }

    public void Dispose() {
      this.Dispose(true);
    }

    private bool _isDisposed = false;

    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed && disposing) {
        this._isDisposed = true;
      }
    }
    

    public override void Draw()
    {
      ImGui.SetNextWindowSize(new Vector2(232, 75), ImGuiCond.Always);
      if (ImGui.Begin("A Wonderful Configuration Window", ref this.settingsVisible,
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)) {
        // can't ref a property, so use a local copy
        var configValue = this.configuration.SomePropertyToBeSavedAndWithADefault;
        if (ImGui.Checkbox("Random Config Bool", ref configValue)) {
          this.configuration.SomePropertyToBeSavedAndWithADefault = configValue;
          // can save immediately on change, if you don't want to provide a "Save and Close" button
          this.configuration.Save();
        }
      }
      ImGui.End();
    }
  }
}
