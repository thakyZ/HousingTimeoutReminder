// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Yes they can in this context", Scope = "type", Target = "~T:NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Services")]
[assembly: SuppressMessage("Critical Code Smell", "S2223:Non-constant static fields should not be visible", Justification = "No they can be visible")]
//[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
[assembly: SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "No we don't need to use LINQ", Scope = "member", Target = "~M:NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.UI.SettingsUI.Draw")]
[assembly: SuppressMessage("Style", "RCS1169:Make field read-only.", Justification = "This field doesn't need to be readonly as it is static", Scope = "member", Target = "~F:NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Plugin.name")]
[assembly: SuppressMessage("CodeQuality", "IDE0052:Remove unread private member.", Justification = "Needs to be instanced.", Scope = "member", Target = "~P:NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Plugin._services")]
[assembly: SuppressMessage("Style", "RCS1163:Unused parameter.", Justification = "Kinda sorta required for passing.", Scope = "member", Target = "~M:NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Plugin.CheckTimers")]
[assembly: SuppressMessage("Style", "RCS1170:Use read-only auto-implemented property.", Justification = "This field doesn't need to be static", Scope = "type", Target = "~T:NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Services")]
[assembly: SuppressMessage("Style", "RCS1102:Make class static.", Justification = "This field doesn't need to be static", Scope = "type", Target = "~T:NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Services")]
[assembly: SuppressMessage("Style", "RCS1163:Unused parameter.", Justification = "Kinda sorta required for passing.", Scope = "member", Target = "~M:NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler.HousingTimer.OnTerritoryChanged(System.UInt16)")]
