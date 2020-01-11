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
  public class SettingPropertyHelpersTest
  {
    class TestProperty
    {
      [Display(Description = "a")]
      [DefaultValue(100)]
      public int Public { get; set; }

      [Display(Description = "a")]
      [DefaultValue(100)]
      protected int Protected { get; set; }

      [Display(Description = "a")]
      [DefaultValue(100)]
      private int Private { get; set; }

      [Display(Description = "a")]
      [DefaultValue(100)]
      static public int StaticPublic { get; set; }
    }

    [Fact]
    public void GetProperties_GetsPublicInstance()
    {
      var props = SettingPropertyHelpers.GetProperties(typeof(TestProperty));

      Assert.Single(props);
      Assert.Equal("Public", props[0].Name);
    }
    
    [Fact]
    public void GetPropertyTypeName_GetsName()
    {
      var prop = GetFilteredProperty<PassIntProp>();

      var name = SettingPropertyHelpers.GetPropertyTypeName(prop);

      Assert.Equal("Int32", name);
      Assert.Equal("System.Int32", prop.PropertyType.FullName);
    }

    class TestFilters
    {
      [Display(Description = "Int32")]
      [DefaultValue(200)]
      public int Attr { get; set; }

      [Display(Description = "String")]
      [DefaultValue("200")]
      public string StrAttr { get; set; } = null!;

      [Display(Description = "Bool")]
      [DefaultValue(true)]
      public bool BooleanAttr { get; set; }

      [Display(Description = "Bool")]
      public bool NoDefaultAttr { get; set; }

      [DefaultValue(true)]
      public bool NoDisplayAttr { get; set; }

      [Display(Description = null)]
      [DefaultValue(true)]
      public bool OkWithDisplayNullAttr { get; set; }
    }

    [Fact]
    public void FilterPropertiesByAttribute_FiltersDisplay()
    {
      var props = SettingPropertyHelpers.GetProperties(typeof(TestFilters));

      props = SettingPropertyHelpers.FilterPropertiesByAttribute<DisplayAttribute>(props);
      
      Assert.Equal(5, props.Count);
    }

    [Fact]
    public void FilterProperties_FiltersDisplayAndDefaultValueAnnotatedProperty()
    {
      var props = SettingPropertyHelpers.GetProperties(typeof(TestFilters));

      props = SettingPropertyHelpers.FilterProperties(props);

      Assert.Equal(4, props.Count);
      Assert.DoesNotContain(props, p => p.Name == "NoDefaultAttr");
      Assert.DoesNotContain(props, p => p.Name == "NoDisplayAttr");
    }

    class PassIntProp
    {
      [Display(Description = "a")]
      [DefaultValue(200)]
      public int Attr { get; set; }
    }
    class PassStrProp
    {
      [Display(Description = "attr")]
      [DefaultValue("")]
      public string Attr { get; set; } = null!;
    }

    private PropertyInfo GetFilteredProperty<T>()
    {
      var type = typeof(T);
      var props = SettingPropertyHelpers.GetProperties(type);
      props = SettingPropertyHelpers.FilterProperties(props);
      return props[0];
    }

    [Fact]
    public void ValidateProperty_PassInt()
    {
      var props = GetFilteredProperty<PassIntProp>();

      SettingPropertyHelpers.ValidateProperty(props);
    }

    [Fact]
    public void ValidateProperty_PassString()
    {
      var props = GetFilteredProperty<PassStrProp>();

      SettingPropertyHelpers.ValidateProperty(props);
    }

    class NoDisplayDescProp
    {
      [Display]
      [DefaultValue(200)]
      public int Attr { get; set; }
    }

    class NullDisplayDescProp
    {
      [Display(Description = null)]
      [DefaultValue(200)]
      public int Attr { get; set; }
    }
    class EmptyDisplayDescProp
    {
      [Display(Description = "")]
      [DefaultValue(200)]
      public int Attr { get; set; }
    }

    [Fact]
    public void ValidateProperty_ThrowsIfNoDisplayDescription()
    {
      var prop = GetFilteredProperty<NoDisplayDescProp>();

      Action act = () => SettingPropertyHelpers.ValidateProperty(prop);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("description", details.Message);
    }

    [Fact]
    public void ValidateProperty_ThrowsIfNullDisplayDescription()
    {
      var prop = GetFilteredProperty<NullDisplayDescProp>();

      Action act = () => SettingPropertyHelpers.ValidateProperty(prop);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("null", details.Message);
    }

    [Fact]
    public void ValidateProperty_ThrowsIfEmptyDisplayDescription()
    {
      var prop = GetFilteredProperty<EmptyDisplayDescProp>();

      Action act = () => SettingPropertyHelpers.ValidateProperty(prop);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("empty", details.Message);
    }

    class TypeMismatchDefaultValueProp
    {
      [Display(Description = "attr")]
      [DefaultValue("200")]
      public int Attr { get; set; }
    }

    class NullDefaultValueProp
    {
      [Display(Description = "attr")]
      [DefaultValue(null)]
      public string Attr { get; set; } = null!;
    }

    [Fact]
    public void ValidateProperty_ThrowsIfMismatchDefault()
    {
      var prop = GetFilteredProperty<TypeMismatchDefaultValueProp>();

      Action act = () => SettingPropertyHelpers.ValidateProperty(prop);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("String", details.Message);
    }

    [Fact]
    public void ValidateProperty_ThrowsIfNullDefault()
    {
      var prop = GetFilteredProperty<NullDefaultValueProp>();

      Action act = () => SettingPropertyHelpers.ValidateProperty(prop);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("null", details.Message);
    }

    class LowerIntRangeProp
    {
      [Range(0, 500)]
      [Display(Description = "attr")]
      [DefaultValue(-1)]
      public int Attr { get; set; }
    }
    class UpperIntRangeProp
    {
      [Range(0, 500)]
      [Display(Description = "attr")]
      [DefaultValue(501)]
      public int Attr { get; set; }
    }

    [Fact]
    public void ValidateProperty_ThrowsIfLowerThanMin()
    {
      var prop = GetFilteredProperty<LowerIntRangeProp>();

      Action act = () => SettingPropertyHelpers.ValidateProperty(prop);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("between", details.Message);
    }

    [Fact]
    public void ValidateProperty_ThrowsIfHigherThanMax()
    {
      var prop = GetFilteredProperty<UpperIntRangeProp>();

      Action act = () => SettingPropertyHelpers.ValidateProperty(prop);

      var exce = Record.Exception(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Contains("between", details.Message);
    }
  }
}
