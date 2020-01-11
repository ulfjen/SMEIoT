using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SMEIoT.Core.Exceptions;

namespace SMEIoT.Core.Helpers
{
  public static class SettingAttributeHelpers
  {
    [Obsolete("deprecated")]
    public static (DisplayAttribute?, DefaultValueAttribute?) GetRequiredAttributes(PropertyInfo property)
    {
      return (property.GetCustomAttribute<DisplayAttribute>(), GetDefaultValueAttribute(property));
    }

    [Obsolete("GetDefaultValueAttribute is deprecated, uses plain value")]
    internal static DefaultValueAttribute? GetDefaultValueAttribute(PropertyInfo property)
    {
      return property.GetCustomAttribute<DefaultValueAttribute>();
    }

    [Obsolete("deprecated")]
    public static RangeAttribute? GetRangeAttribute(PropertyInfo property)
    {
      return property.GetCustomAttribute<RangeAttribute>();
    }

    // ValidateDefaultValueAttribute should validate property before any usage of that property
    // hence no check for null
    [Obsolete("deprecated")]
    public static object GetDefaultValueFromAttribute(PropertyInfo property, DefaultValueAttribute attribute)
    {
      if (attribute.Value == null) {
        throw new InvalidArgumentException("Has a null default value.", property.Name);
      }
      return attribute.Value!;
    }

    public static void ValidateDisplayAttribute(PropertyInfo property, DisplayAttribute display)
    {
      if (string.IsNullOrEmpty(display.Description)) {
        throw new InvalidArgumentException("Has a null or empty display description.", property.Name);
      }
    }

    [Obsolete("deprecated")]
    public static void ValidateDefaultValueAttribute(PropertyInfo property, DefaultValueAttribute defaultValue)
    {
      if (defaultValue.Value == null) {
        throw new InvalidArgumentException($"Has a null default value.", property.Name);
      }
      var defaultValueType = defaultValue.Value.GetType();
      if (defaultValueType != property.PropertyType) {
        throw new InvalidArgumentException($"Has a wrong default value type {defaultValueType}.", property.Name);
      }
    }

    public static void ValidateRangeAttribute(PropertyInfo property, RangeAttribute range)
    {
      if (range.OperandType != typeof(int)) {
        throw new InvalidArgumentException($"Type {range.OperandType.Name} can't be used with RangeAttribute in settings.", property.Name);
      }
#if false  // if we don't allow other use of reference type, RangeAttribute constructor wouldn't allow these cases.
      if (range.Maximum == null) {
        throw new InvalidArgumentException($"Has a null maximum value.", property.Name);
      }
      if (range.Minimum == null) {
        throw new InvalidArgumentException($"Has a null minimum value.", property.Name);
      }
#endif
    }

    public static void ValidateValueInRange(PropertyInfo property, RangeAttribute range, object value)
    {
      var result = range.GetValidationResult(value, new ValidationContext(value));
      if (result != ValidationResult.Success) {
        throw new InvalidArgumentException(result.ErrorMessage, property.Name);
      }
    }
  }
}
