using System;
using System.ComponentModel;

using System.Linq;
using System.Reflection;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Configuration;
/// <summary>
/// Enum detailing the different districts
/// </summary>
public enum District {
  [Description("Unknown")]
  Unknown = 0,
  [Description("The Goblet")]
  Goblet = 1,
  [Description("The Mist")]
  Mist = 2,
  [Description("The Lavender Beds")]
  LavenderBeds = 3,
  [Description("Empyreum")]
  Empyreum = 4,
  [Description("Shirogane")]
  Shirogane = 5
}

public static class DistrictEnumHelper {
  public static string GetName(District district) {
    return Enum.GetName(typeof(District), (int)district) ?? district.ToString();
  }

  public static string GetDescriptor(District district) {
    var member = Array.Find(typeof(District).GetMember(GetName(district)), (MemberInfo member) => member.MemberType == MemberTypes.Field);
    if (member?.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute description) {
      return description.Description;
    }

    return district.ToString();
  }

  public static (string Name, District ID)[] GetDescriptors() {
    return [..typeof(District).GetFields(BindingFlags.Static | BindingFlags.Public)
      .Select((FieldInfo field) => {
        District district = (District?)field.GetValue(null) ?? District.Unknown;
        string descriptor = field.GetCustomAttribute<DescriptionAttribute>()?.Description ?? district.ToString();
        return (Name: descriptor, ID: district);
      }
    ).Where(tuple => tuple.ID != District.Unknown)];
  }
}
