using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Services;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Helpers;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Tests.Shared;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;

namespace SMEIoT.Tests.Core.Helpers
{
  public class SettingAttributeHelpersTest
  {
    #region Getter tests

    public class TestAttributes
    {
      [Range(0, 100)]
      public int WithoutRequired { get; set; }

      [DefaultValue(100)]
      public int OnlyDefaultValue { get; set; }

      [Display(Description = "a")]
      public int OnlyDisplay { get; set; }

      [Display(Description = "a")]
      [DefaultValue(100)]
      public int Attr { get; set; }

      [Range(0, 100)]
      [Display(Description = "a")]
      [DefaultValue(100)]
      public int RangeAttr { get; set; }

      [Display(Description = "a")]
      [DefaultValue(null)]
      public int NullAttr { get; set; }
    }

    [Fact]
    public void GetDisplayAttribute_Gets()
    {
      var type = typeof(TestAttributes);
      var propInfo = type.GetProperty("Attr");

      var display = SettingAttributeHelpers.GetDisplayAttribute(propInfo!);

      Assert.NotNull(display);
      Assert.IsType<DisplayAttribute>(display);
    }

    [Fact]
    public void GetRequiredAttributes_GetsRequiredAttributes()
    {
      var type = typeof(TestAttributes);
      var propInfo = type.GetProperty("Attr");

      var (display, defaultValue) = SettingAttributeHelpers.GetRequiredAttributes(propInfo!);

      Assert.NotNull(display);
      Assert.IsType<DisplayAttribute>(display);
      Assert.NotNull(defaultValue);
      Assert.IsType<DefaultValueAttribute>(defaultValue);
    }

    [Fact]
    public void GetRequiredAttributes_GetsOnlyDisplayAttribute()
    {
      var type = typeof(TestAttributes);
      var propInfo = type.GetProperty("OnlyDisplay");

      var (display, defaultValue) = SettingAttributeHelpers.GetRequiredAttributes(propInfo!);

      Assert.NotNull(display);
      Assert.IsType<DisplayAttribute>(display);
      Assert.Null(defaultValue);
    }
    
    [Fact]
    public void GetRequiredAttributes_GetsOnlyDefaultValueAttribute()
    {
      var type = typeof(TestAttributes);
      var propInfo = type.GetProperty("OnlyDefaultValue");

      var (display, defaultValue) = SettingAttributeHelpers.GetRequiredAttributes(propInfo!);

      Assert.Null(display);
      Assert.NotNull(defaultValue);
      Assert.IsType<DefaultValueAttribute>(defaultValue);
    }

    [Fact]
    public void GetRequiredAttributes_GetsNoRequiredAttribute()
    {
      var type = typeof(TestAttributes);
      var propInfo = type.GetProperty("WithoutRequired");

      var (display, defaultValue) = SettingAttributeHelpers.GetRequiredAttributes(propInfo!);

      Assert.Null(display);
      Assert.Null(defaultValue);
    }

    [Fact]
    public void GetRangeAttribute_GetsRangeAttribute()
    {
      var type = typeof(TestAttributes);
      var propInfo = type.GetProperty("RangeAttr");

      var range = SettingAttributeHelpers.GetRangeAttribute(propInfo!);

      Assert.NotNull(range);
      Assert.IsType<RangeAttribute>(range);
    }

    [Fact]
    public void GetRangeAttribute_ReturnsNullWhenNoRangeAttribute()
    {
      var type = typeof(TestAttributes);
      var propInfo = type.GetProperty("Attr");

      var range = SettingAttributeHelpers.GetRangeAttribute(propInfo!);

      Assert.Null(range);
    }

    private static (PropertyInfo, DisplayAttribute, DefaultValueAttribute) GetRequiredAttributeInfo<T>(string name)
    {
      var type = typeof(T);
      var propInfo = type.GetProperty(name);
      var (display, defaultValue) = SettingAttributeHelpers.GetRequiredAttributes(propInfo!);
      return (propInfo!, display!, defaultValue!);
    }

    [Fact]
    public void GetDefaultValueFromAttribute_GetsValue()
    {
      var (propInfo, display, defaultValue) = GetRequiredAttributeInfo<TestAttributes>("Attr");

      var val = SettingAttributeHelpers.GetDefaultValueFromAttribute(propInfo, defaultValue);

      Assert.Equal(100, val);
    }

    [Fact]
    public void GetDefaultValueFromAttribute_ThrowsExceptionWithNullValue()
    {
      var (propInfo, display, defaultValue) = GetRequiredAttributeInfo<TestAttributes>("NullAttr");

      Action act = () => SettingAttributeHelpers.GetDefaultValueFromAttribute(propInfo, defaultValue);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("null", details.Message);
    }
    
    #endregion

    #region Validation tests

    public class TestValidationAttributes
    {
      [Display(Description = "")]
      [DefaultValue(100)]
      public int NoDisplayDesc { get; set; }

      [Display(Description = "WrongTypeDefaultValue")]
      [DefaultValue(true)]
      public int WrongTypeDefaultValue { get; set; }

      [Display(Description = "NullDefaultValue")]
      [DefaultValue(null)]
      public string NullDefaultValue { get; set; } = null!;

      [Display(Description = "intattr")]
      [DefaultValue(100)]
      public int IntAttr { get; set; }
    }

    [Fact]
    public void ValidateDisplayAttribute_Pass()
    {
      var (propInfo, display, defaultValue) = GetRequiredAttributeInfo<TestValidationAttributes>("IntAttr");

      SettingAttributeHelpers.ValidateDisplayAttribute(propInfo, display);
    }

    [Fact]
    public void ValidateDisplayAttribute_ThrowsWithoutDescription()
    {
      var (propInfo, display, defaultValue) = GetRequiredAttributeInfo<TestValidationAttributes>("NoDisplayDesc");

      Action act = () => SettingAttributeHelpers.ValidateDisplayAttribute(propInfo, display);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("description", details.Message);
    }


    [Fact]
    public void ValidateDefaultValueAttribute_Pass()
    {
      var (propInfo, display, defaultValue) = GetRequiredAttributeInfo<TestValidationAttributes>("IntAttr");

      SettingAttributeHelpers.ValidateDefaultValueAttribute(propInfo, defaultValue);
    }

    [Fact]
    public void ValidateDefaultValueAttribute_ThrowsWithWrongType()
    {
      var (propInfo, display, defaultValue) = GetRequiredAttributeInfo<TestValidationAttributes>("WrongTypeDefaultValue");

      Action act = () => SettingAttributeHelpers.ValidateDefaultValueAttribute(propInfo, defaultValue);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("type", details.Message);
    }

    [Fact]
    public void ValidateDefaultValueAttribute_ThrowsWithNull()
    {
      var (propInfo, display, defaultValue) = GetRequiredAttributeInfo<TestValidationAttributes>("NullDefaultValue");

      Action act = () => SettingAttributeHelpers.ValidateDefaultValueAttribute(propInfo, defaultValue);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("null", details.Message);
    }

    class TestRangeValidation
    {
      [Display(Description = "intrangeattr")]
      [DefaultValue(100)]
      [Range(0, 500)]
      public int IntRangeAttr { get; set; }

      [Display(Description = "outrangeattr")]
      [DefaultValue(1000)]
      [Range(0, 500)]
      public int OutOfRangeAttr { get; set; }

      [Display(Description = "doublerangeattr")]
      [DefaultValue(100.0)]
      [Range(0.0, 500.0)]
      public double DoubleRangeAttr { get; set; }

      [Display(Description = "stringrangeattr")]
      [DefaultValue("bbb")]
      [Range(typeof(string), "aaa", "zzz")]
      public string StringRangeAttr { get; set; } = null!;

      [Display(Description = "rangeattrnullmin")]
      [DefaultValue(100)]
      [Range(typeof(string), null, "500")]
      public string RangeAttrNullMin { get; set; } = null!;

      [Display(Description = "rangeattrnullmax")]
      [DefaultValue(100)]
      [Range(typeof(string), "0", null)]
      public string RangeAttrNullMax { get; set; } = null!;
    }

    private static (PropertyInfo, RangeAttribute, object) GetRangeAttributeInfo<T>(string name)
    {
      var (propInfo, display, defaultValue) = GetRequiredAttributeInfo<T>(name);
      var range = SettingAttributeHelpers.GetRangeAttribute(propInfo);
      var val = SettingAttributeHelpers.GetDefaultValueFromAttribute(propInfo, defaultValue);
      return (propInfo, range!, val);
    }

    [Fact]
    public void ValidateRangeAttribute_Pass()
    {
      var (propInfo, range, val) = GetRangeAttributeInfo<TestRangeValidation>("IntRangeAttr");

      SettingAttributeHelpers.ValidateRangeAttribute(propInfo, range!);
    }

    [Fact]
    public void ValidateRangeAttribute_ThrowsWithDoubleType()
    {
      var (propInfo, range, val) = GetRangeAttributeInfo<TestRangeValidation>("DoubleRangeAttr");

      Action act = () =>  SettingAttributeHelpers.ValidateRangeAttribute(propInfo, range!);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("Double", details.Message);
    }

    [Fact]
    public void ValidateRangeAttribute_ThrowsWithStringType()
    {      
      var (propInfo, range, val) = GetRangeAttributeInfo<TestRangeValidation>("StringRangeAttr");

      Action act = () =>  SettingAttributeHelpers.ValidateRangeAttribute(propInfo, range!);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("String", details.Message);
    }

#if false // Reference type not allowed in the method. RangeAttribute constructor wouldn't allow these cases.
    [Fact]
    public void ValidateRangeAttribute_ThrowsWithNullMaximum()
    {
      var (propInfo, range, val) = GetRangeAttributeInfo<TestRangeValidation>("RangeAttrNullMax");

      Action act = () =>  SettingAttributeHelpers.ValidateRangeAttribute(propInfo, range!);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("null", details.Message);
    }

    [Fact]
    public void ValidateRangeAttribute_ThrowsWithNullMinimum()
    {
      var (propInfo, range, val) = GetRangeAttributeInfo<TestRangeValidation>("RangeAttrNullMin");

      Action act = () =>  SettingAttributeHelpers.ValidateRangeAttribute(propInfo, range!);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("null", details.Message);
    }
#endif

    [Fact]
    public void ValidateRangeAttribute_PassWithNotInRangeValue()
    {
      var (propInfo, range, val) = GetRangeAttributeInfo<TestRangeValidation>("OutOfRangeAttr");

      SettingAttributeHelpers.ValidateRangeAttribute(propInfo, range!);

    }

    [Fact]
    public void ValidateValueInRange_ThrowsWithNotInRangeValue()
    {
      var (propInfo, range, val) = GetRangeAttributeInfo<TestRangeValidation>("OutOfRangeAttr");

      Action act = () => SettingAttributeHelpers.ValidateValueInRange(propInfo, range!, val);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("between", details.Message);
    }
    #endregion
  }
}
