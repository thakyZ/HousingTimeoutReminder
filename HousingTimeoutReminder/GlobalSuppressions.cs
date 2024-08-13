// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Critical Code Smell", "S2223:Non-constant static fields should not be visible", Justification = "No they can be visible")]
[assembly: SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "No we don't need to use LINQ", Scope = "member", Target = "~M:NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI.SettingsUI.Draw")]
[assembly: SuppressMessage("Style", "RCS1163:Unused parameter.", Justification = "Kinda sorta required for passing.", Scope = "member", Target = "~M:NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler.HousingTimer.OnTerritoryChanged(System.UInt16)")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "This member is required for inherited class.", Scope = "member", Target = "~P:NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Plugin.Name")]
