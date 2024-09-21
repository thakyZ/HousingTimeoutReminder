using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Handler;

/// <summary>
/// An enum for different housing types.
/// </summary>
public enum HousingType {
  [Description("Unknown")]
  Unknown = 0,
  [Description("Free Company Estate")]
  FreeCompanyEstate = 1,
  [Description("Private Estate")]
  PrivateEstate = 2,
  [Description("Apartment")]
  Apartment = 3,
}

public static class HousingTypeEnumHelper {
  public static string GetName(HousingType district) {
    return Enum.GetName(typeof(HousingType), (int)district) ?? district.ToString();
  }

  public static string GetDescriptor(HousingType district) {
    var member = Array.Find(typeof(HousingType).GetMember(GetName(district)), (MemberInfo member) => member.MemberType == MemberTypes.Field);
    if (member?.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute description) {
      return description.Description;
    }
    return district.ToString();
  }

  public static (string Name, HousingType ID)[] GetDescriptors() {
    return [..typeof(HousingType).GetFields(BindingFlags.Static | BindingFlags.Public)
      .Select((FieldInfo field) => {
          HousingType district = (HousingType?)field.GetValue(null) ?? HousingType.Unknown;
          string descriptor = field.GetCustomAttribute<DescriptionAttribute>()?.Description ?? district.ToString();
          return (Name: descriptor, ID: district);
        }
      ).Where(tuple => tuple.ID != HousingType.Unknown)];
  }
}
