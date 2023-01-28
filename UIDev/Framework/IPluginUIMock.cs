using ImGuiScene;

using System;

namespace NekoBoiNick.HousingTimeoutReminder.UIDev {
  internal interface IPluginUIMock : IDisposable {
    void Initialize(SimpleImGuiScene scene);
  }
}
