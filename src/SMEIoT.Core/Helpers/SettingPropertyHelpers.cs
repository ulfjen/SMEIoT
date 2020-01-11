using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Core.Helpers
{
  public static class SettingPropertyHelpers
  {
    // must be public instance property
    public static IList<PropertyInfo> GetProperties(Type type)
    {
      return type.GetProperties(BindingFlags.Public|BindingFlags.Instance);
    }

    public static IList<PropertyInfo> GetProperties<T>()
    {
      return typeof(T).GetProperties(BindingFlags.Public|BindingFlags.Instance);
    }

    public static IList<PropertyInfo> FilterPropertiesByAttribute<T>(IList<PropertyInfo> properties)
    {
      return properties.Where(prop => prop.GetCustomAttribute<T>() != null);
    }

    public static IList<PropertyInfo> GetPropertiesWithAttribute<T>(Type type)
    {
      var props = GetProperties(type);
      props = FilterPropertiesByAttribute<T>(props);
      return props;
    }

    // must has display and default attributes
    [Obsolete("deprecated")]
    public static IList<PropertyInfo> FilterProperties(IList<PropertyInfo> properties)
    {
      return properties.Where(prop => {
        var (displayAttr, defaultAttr) = SettingAttributeHelpers.GetRequiredAttributes(prop);
        return displayAttr != null && defaultAttr != null;
      }).ToList();
    }

    // ensure we uses the same type name
    public static string GetPropertyTypeName(PropertyInfo property)
    {
      return property.PropertyType.Name;
    }
    
    // Validate attributes
    public static void ValidateProperty(PropertyInfo prop)
    {
      var (displayAttr, defaultAttr) = SettingAttributeHelpers.GetRequiredAttributes(prop);

      // we assumes displayAttr, defaultAttr are filtered.
      SettingAttributeHelpers.ValidateDisplayAttribute(prop, displayAttr!);
      SettingAttributeHelpers.ValidateDefaultValueAttribute(prop, defaultAttr!);
      var range = SettingAttributeHelpers.GetRangeAttribute(prop);
      if (range != null) {
        SettingAttributeHelpers.ValidateRangeAttribute(prop, range);
        var defaultValue = SettingAttributeHelpers.GetDefaultValueFromAttribute(prop, defaultAttr!);
        ValidateValueForProperty(prop, defaultValue);
      }
    }

    [Obsolete("deprecated")]
    public static void ValidateProperties(IList<PropertyInfo> properties)
    {
      foreach (var prop in properties) {
        ValidateProperty(prop);
      }
    }

    public static IList<ValidationResult> GetValidateResultForPropertyValue(PropertyInfo property, object value)
    {
      var range = SettingAttributeHelpers.GetRangeAttribute(property);
      var defaultAttr = SettingAttributeHelpers.GetDefaultValueAttribute(property);
      var defaultValue = SettingAttributeHelpers.GetDefaultValueFromAttribute(property, defaultAttr!);
      SettingAttributeHelpers.ValidateValueInRange(property, range!, defaultValue);
    }

    [Obsolete("deprecated")]
    public static void ValidateValueForProperty(PropertyInfo property, object value)
    {
      var range = SettingAttributeHelpers.GetRangeAttribute(property);
      var defaultAttr = SettingAttributeHelpers.GetDefaultValueAttribute(property);
      var defaultValue = SettingAttributeHelpers.GetDefaultValueFromAttribute(property, defaultAttr!);
      SettingAttributeHelpers.ValidateValueInRange(property, range!, defaultValue);
    }
  }

}
