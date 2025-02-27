using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using ECommons;
using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Data;
using Lumina.Excel.Sheets;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration.Data;
using NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder;

internal sealed class Systems : IDisposable {
  /// <summary>
  /// A <see langword="bool" /> indicating if the instance of this <see cref="IDisposable" /> is disposed already.
  /// </summary>
  private bool _isDisposed;

  /// <summary>
  /// Defines the name for the long version of the plugin's main command.
  /// </summary>
  private const string _LONG_COMMAND = "/htimeout";

  /// <summary>
  /// Defines the name for the short version of the plugin's main command.
  /// </summary>
  private const string _SHORT_COMMAND = "/ht";

  /// <summary>
  /// Gets the help message for the command.
  /// </summary>
  private static string CommandHelp
    => // Loc.Localize(nameof(HousingTimeoutReminder) + "_" + nameof(CommandHelp),
       """
       The main command of this plugin.
           SUBCOMMANDS:
             - check:             Re-checks if there are any reminders and/or restores the warning dialog.
             - restore:           Restores the warning dialog if there is anything to warn about.
             - settings:          Toggles the config window
             - config:            Toggles the config window
             - dismiss [current]: Dismisses the warning dialog.
                - current:        Dismisses the current warning in the warning dialog.
       """
#if DEBUG || xPersonalRelease
       + "\n      - debug:      Toggles the debug window."
#endif
       + "\n      - help:       Displays this help message."; //);

  /// <summary>
  /// Gets the help message for the command.
  /// </summary>
  private static string UnknownSubcommand
    => // Loc.Localize(nameof(HousingTimeoutReminder) + "_" + nameof(CommandHelp),
      """
      Unknown subcommand {0}.
      Please use {1} help to see the list of subcommands.
      """; //);

  /// <summary>
  /// Gets an instance of the plugin's configuration.
  /// </summary>
  internal PluginConfig Config { get; }

  /// <summary>
  /// Gets an instance of the plugin's configuration.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Compiler", "CA1822:Mark members as static")]
  internal PlayerManager PlayerManager { get; }

  /// <summary>
  /// Gets the settings window of the plugin.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local"),
   SuppressMessage("Compiler", "CA1822:Mark members as static")]
  private SettingsWindow SettingsWindow { get; }

  /// <summary>
  /// Gets the warning dialog of the plugin.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local"),
   SuppressMessage("Compiler", "CA1822:Mark members as static")]
  private WarningDialog WarningDialog { get; }

#if DEBUG || xPersonalRelease
  /// <summary>
  /// Gets the debug window of the plugin.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local"),
   SuppressMessage("Compiler", "CA1822:Mark members as static")]
  private DebugWindow DebugWindow { get; }
#endif

  /// <summary>
  /// Gets the window system of the plugin.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local"),
   SuppressMessage("Compiler", "CA1822:Mark members as static")]
  private WindowSystem WindowSystem { get; }

  /// <summary>
  /// Gets or sets the number entries which have warnings.
  /// </summary>
  public static WarningEntryCollection WarningEntries => [];

  /// <summary>
  /// Gets or sets the number of failed player config read entries.
  /// </summary>
  public static int NumberFailed { get; internal set; }

  /// <summary>
  /// Gets a <see cref="bool" /> indicating if the game is logged into a world.
  /// </summary>
  [MemberNotNullWhen(true, nameof(CurrentPlayer), nameof(CurrentWorld), nameof(CurrentPlayerHomeWorld), nameof(CurrentPlayerName), nameof(CurrentPlayerID))]
  internal static bool IsLoggedIn
    => Svc.ClientState.IsLoggedIn;

  /// <summary>
  /// Gets a <see cref="bool" /> indicating if the game is logged into a world.
  /// </summary>
  internal static IPlayerCharacter? CurrentPlayer {
    get {
      if (IsLoggedIn && Svc.ClientState.LocalPlayer is IPlayerCharacter player) {
        return player;
      }
      return null;
    }
  }

  /// <summary>
  /// Gets a <see langword="string" /> indicating the current world name.
  /// </summary>
  internal static string? CurrentWorld => CurrentPlayer?.CurrentWorld.GetName();

  /// <summary>
  /// Gets a <see langword="string" /> indicating the current player's home world name.
  /// </summary>
  internal static string? CurrentPlayerHomeWorld => CurrentPlayer?.HomeWorld.GetName();

  /// <summary>
  /// Gets a <see langword="string" /> indicating the current player's name world name.
  /// </summary>
  internal static string? CurrentPlayerName => CurrentPlayer?.Name.ExtractText();

  /// <summary>
  /// Gets an <see langword="uint" /> dermining the current player's id.
  /// </summary>
  internal static ulong? CurrentPlayerID => IsLoggedIn ? Svc.ClientState.LocalContentId : null;

  internal Systems() {
    this.Config = PluginConfig.LoadConfig();
    this.PlayerManager = new PlayerManager();
    this.WindowSystem = new WindowSystem(Plugin.InternalName);
    this.SettingsWindow = new SettingsWindow();
    this.WarningDialog = new WarningDialog();
    this.WindowSystem.AddWindow(SettingsWindow);
    this.WindowSystem.AddWindow(WarningDialog);
#if DEBUG || xPersonalRelease
    this.DebugWindow = new DebugWindow();
    this.WindowSystem.AddWindow(DebugWindow);
#endif
    Svc.PluginInterface.UiBuilder.Draw += DrawUI;
    Svc.PluginInterface.UiBuilder.ShowUi += ShowUI;
    Svc.PluginInterface.UiBuilder.OpenMainUi += ShowWarningDialog;
    Svc.PluginInterface.UiBuilder.HideUi += HideUI;
    Svc.PluginInterface.UiBuilder.OpenConfigUi += ToggleSettingsWindow;
    Svc.ClientState.TerritoryChanged += ClientStateOnTerritoryChanged;
    this.TryAddCommand(_LONG_COMMAND, OnCommand, true, CommandHelp);
    this.TryAddCommand(_SHORT_COMMAND, OnCommand, true, CommandHelp);
    Svc.Log?.Verbose("Loaded {0}", Plugin.Name);
  }

  private void ClientStateOnTerritoryChanged(ushort territoryType)
    => WarningDialog.CheckForReminders();

  /// <summary>
  /// Tries to add a command by <paramref name="name"/> or logs to the console.
  /// </summary>
  /// <param name="name">The name of the command to try to add.</param>
  /// <param name="handler">The command method to add.</param>
  /// <param name="showInHelp">Show the command in the help command.</param>
  /// <param name="helpMessage">The help message to print in the description of the command.</param>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
  private void TryAddCommand(string name, IReadOnlyCommandInfo.HandlerDelegate handler, bool showInHelp, string helpMessage) {
    try {
      Svc.Commands.AddHandler(name, new CommandInfo(handler) {
        ShowInHelp = showInHelp,
        HelpMessage = helpMessage,
      });
    } catch (Exception ex) {
#if DEBUG || xPersonalRelease
      Svc.Log.Error(ex, "Failed to add command with name {0}", name);
#endif
    }
  }

  /// <inheritdoc cref="IReadOnlyCommandInfo.HandlerDelegate"/>
  private void OnCommand(string command, string arguments) {
    string[] args = arguments.Split(' ');
    string subcommand = args[0];
    bool current = args.Length == 2
                   && args[0].Equals("dismiss", StringComparison.OrdinalIgnoreCase)
                   && args[1].Equals("current", StringComparison.OrdinalIgnoreCase);

    switch (subcommand.ToLower()) {
      case "check":
        WarningDialog.CheckForReminders();
        goto case "restore";
      case "restore":
        this.WarningDialog.Restore();
        break;
      case "dismiss" when current:
        this.WarningDialog.DismissCurrent();
        break;
      case "dismiss" when !current:
        this.WarningDialog.DismissAll();
        break;
      case "settings":
      case "config":
        this.ToggleSettingsWindow();
        break;
#if DEBUG || xPersonalRelease
      case "debug":
        this.ToggleDebugWindow();
        break;
#endif
      case "help":
        PrintToChat(CommandHelp);
        break;
      default:
        PrintToChat(string.Format(UnknownSubcommand, subcommand, command));
        break;
    }
  }

  /// <summary>
  /// Handles printing to the game chat.
  /// </summary>
  private static void PrintToChat(string message)
    => Svc.Chat.Print(message);

  /// <summary>
  /// Handles restoring all warning messages.
  /// </summary>
  internal void RestoreAllDismissals()
    => this.WarningDialog.RestoreAll();

  /// <summary>
  /// Handles restoring all warning messages.
  /// </summary>
  private void DrawUI()
    => this.WindowSystem.Draw();

  /// <summary>
  /// Handles toggling the <see cref="SettingsWindow" />.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
  private void ToggleSettingsWindow() {
    Svc.Log.Verbose("Toggling {0} to {1} state.", nameof(SettingsWindow), !this.SettingsWindow.IsOpen);
    bool @bool = this.SettingsWindow.IsOpen;
    @bool.Toggle();
    this.SettingsWindow.IsOpen = @bool;
  }

  /// <summary>
  /// Handles showing the <see cref="WarningDialog" /> and checking for reminders or hiding if none found.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
  private void ShowWarningDialog()
    => this.WarningDialog.CheckForRemindersOrHide();

#if DEBUG || xPersonalRelease
  /// <summary>
  /// Handles toggling the <see cref="DebugWindow" />.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
  private void ToggleDebugWindow() {
    Svc.Log.Verbose("Toggling {0} to {1} state.", nameof(DebugWindow), !this.DebugWindow.IsOpen);
    // TODO: Find a way to toggle this bool via a method on a property.
    bool @bool = this.DebugWindow.IsOpen;
    @bool.Toggle();
    this.DebugWindow.IsOpen = @bool;
  }
#endif

  /// <summary>
  /// Proxy for handling hiding the <see cref="WarningDialog" /> based on configuration.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
  private void HideUI()
    => this.WarningDialog.OnUIHide();

  /// <summary>
  /// Proxy for handling showing the <see cref="WarningDialog" /> based on configuration.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
  private void ShowUI()
    => this.WarningDialog.OnUIShow();

  internal static List<WorldInfo> WorldInfos { get; } = GetWorldInfos();

  internal readonly struct WorldInfo(string worldName, uint worldId, string dataCemter, ushort dataCenterId, string region, ushort regionId) {
    public string WorldName { get; } = worldName;
    public string DataCemter { get; } = dataCemter;
    public string Region { get; } = region;
    public uint WorldId { get; } = worldId;
    public ushort DataCenterId { get; } = dataCenterId;
    public ushort RegionId { get; } = regionId;
  }

  private static List<WorldInfo> GetWorldInfos() {
    List<WorldInfo> output = [];
    try {
      Svc.Data.GameData.GetExcelSheet<World>(Language.English)?.ToDictionary(x => x.RowId, x => x.Name.ExtractText()) ?? [];
      Svc.Data.GameData.GetExcelSheet<>(Language.English)?.ToDictionary(x => x.RowId, x => x.Name.ExtractText()) ?? [];
      unsafe {
        var instance = DataCenterHelper.Instance();
        if (instance == null)
          return [];
        var dataCenters = instance->DataCenters;
        for (int i = 0; i < dataCenters.Count; i++) {
          var dataCenter = dataCenters[i];
          var dataCenterId = dataCenter.Index;
          var dataCenterName = dataCenter.NameString;
          byte regionId = dataCenter.Region;
          string regionName = ((ExcelWorldHelper.Region)regionId).ToString();
        }
      }

    } catch (Exception exception) {
      Svc.Log.Error(exception, "Failed to get collection of world infos");
    }

    return output;
  }

  /// <summary>
  /// Tries to get the <see cref="World" /> name via the provided id.
  /// </summary>
  /// <param name="homeWorldID">The game world ID.</param>
  /// <returns>The name of the <see cref="World" /> if successfully found or null.</returns>
  internal static string? GetWorldName(uint? homeWorldID) {
    if (!homeWorldID.HasValue) {
      return null;
    }

    if (!Svc.Data.GameData.GetExcelSheet<World>(Language.English).TryGetFirst(x => x.RowId.Equals(homeWorldID.Value), out World value)) {
      return null;
    }

    return value.Name.ExtractText();
  }

  /// <summary>
  /// Handles toggling the repositioning of the <see cref="WarningDialog" />.
  /// </summary>
  public void ToggleWarningDialogRepositioning()
    => this.WarningDialog.ToggleRepositioning();

  public void CheckForRemindersForPlayer(PlayerConfigEntry entry, PlayerConfig playerConfig)
    => this.WarningDialog.CheckForReminders(entry, playerConfig);

  public void CheckForReminders()
    => this.WarningDialog.CheckForReminders();

  private void Dispose(bool disposing) {
    // ReSharper disable once InvertIf
    if (!_isDisposed) {
      if (disposing) {
        // Save the plugin config at the end.
        PluginConfig.SavePlayerConfigs();
        PluginConfig.Save();
        // NOTE: dispose managed state (managed objects)
      }

      Svc.PluginInterface.UiBuilder.Draw -= this.WindowSystem.Draw;
      Svc.PluginInterface.UiBuilder.ShowUi -= ShowWarningDialog;
      Svc.PluginInterface.UiBuilder.OpenMainUi -= ShowWarningDialog;
      Svc.PluginInterface.UiBuilder.HideUi -= HideUI;
      Svc.PluginInterface.UiBuilder.OpenConfigUi -= ToggleSettingsWindow;
      Svc.ClientState.TerritoryChanged -= ClientStateOnTerritoryChanged;
      this.SettingsWindow.Dispose();
      this.WarningDialog.Dispose();
#if DEBUG || xPersonalRelease
      this.DebugWindow.Dispose();
#endif
      this.WindowSystem.RemoveAllWindows();
      Svc.Commands.RemoveHandler(_LONG_COMMAND);
      Svc.Commands.RemoveHandler(_SHORT_COMMAND);

      // NOTE: free unmanaged resources (unmanaged objects) and override finalizer
      // NOTE: set large fields to null
      _isDisposed = true;
    }
  }

  // NOTE: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
  ~Systems() {
    this.Dispose(disposing: false);
  }

  /// <inheritdoc cref="IDisposable.Dispose"/>
  public void Dispose() {
    this.Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}
