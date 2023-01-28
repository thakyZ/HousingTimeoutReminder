using HousingTimeoutReminder.UI;

using ImGuiNET;

using System;
using System.Numerics;

namespace NekoBoiNick.HousingTimeoutReminder {

  // It is good to have this be disposable in general, in case you ever need it to do any cleanup
  class PluginUI : IDisposable {
    private readonly Configuration configuration;

    // this extra bool exists for ImGui, since you can't ref a property
    private bool visible = false;
    public bool Visible {
      get { return this.visible; }
      set { this.visible = value; }
    }

    private bool settingsVisible = false;
    public bool SettingsVisible {
      get { return this.settingsVisible; }
      set { this.settingsVisible = value; }
    }

    private SettingsUI settingsUI { get; set; }

    public PluginUI(Configuration configuration) {
      this.configuration = configuration;
      settingsUI = new SettingsUI(configuration);
    }

    public void Dispose() {
      this.Dispose(true);
    }

    private bool _isDisposed = false;

    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed && disposing) {
        settingsUI.Dispose();
        this._isDisposed = true;
      }
    }

    public void Draw() {
      // This is our only draw handler attached to UIBuilder, so it needs to be
      // able to draw any windows we might have open.
      // Each method checks its own visibility/state to ensure it only draws when
      // it actually makes sense.
      // There are other ways to do this, but it is generally best to keep the number of
      // draw delegates as low as possible.
      DrawMainWindow();
      DrawSettingsWindow();
    }

    public void DrawMainWindow() {
      if (!Visible) {
        return;
      }

      ImGui.SetNextWindowSize(new Vector2(375, 330), ImGuiCond.FirstUseEver);
      ImGui.SetNextWindowSizeConstraints(new Vector2(375, 330), new Vector2(float.MaxValue, float.MaxValue));
      if (ImGui.Begin("My Amazing Window", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)) {
        ImGui.Text($"The random config bool is {this.configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings")) {
          SettingsVisible = true;
        }
      }
      ImGui.End();
    }

    public void DrawSettingsWindow() {
      if (!SettingsVisible) {
        return;
      }

      settingsUI.Draw();
    }
  }
}
