using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Services;
using SMEIoT.Core.Interfaces;
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

namespace SMEIoT.Tests.Core.Services
{
  public class SettingTestOnlyDefaultValueAttribute : Settings
  {
    [DefaultValue(200)]
    public int Attr { get; set; }
  }
  public class SettingTestNoAttributes : Settings
  {
    public int Attr { get; set; } = 1;
  }
  public class SettingTestDefaultValueTypeMismatch : Settings
  {
    [Display(Description = "a")]
    [DefaultValue("200")]
    public int Attr { get; set; }
  }
  public class SettingTestDefaultValueEmptyDescription : Settings
  {
    [Display]
    [DefaultValue(200)]
    public int Attr { get; set; }
  }
  public class SettingTestDefaultValueNull : Settings
  {
    [Display(Description = "a")]
    [DefaultValue(null)]
    public string Attr { get; set; } = null!;
  }

  public class SettingTestIntRange : SettingsBase
  {
    [Range(100, 200, ErrorMessage="bewteen")]
    [Display(Description = "a")]
    [DefaultValue(150)]
    public int Attr { get; set; }
  }

  public class SettingTestIntRangeLowerDefault : SettingsBase
  {
    [Range(100, 200, ErrorMessage="bewteen")]
    [Display(Description = "a")]
    [DefaultValue(99)]
    public int Attr { get; set; }
  }

  public class SettingTestIntRangeHigherDefault : SettingsBase
  {
    [Range(100, 200, ErrorMessage="bewteen")]
    [Display(Description = "a")]
    [DefaultValue(201)]
    public int Attr { get; set; }
  }

  public class SettingTestRangeStringType : SettingsBase
  {
    [Range(typeof(string), "aaa", "zzz", ErrorMessage = "Value for {0} must be between {1} and {2}")]
    [Display(Description = "a")]
    [DefaultValue("bbb")]
    public string Attr { get; set; }
  }

  public class SettingTestRangeDoubleType : SettingsBase
  {
    [Range(0.0, 100.0, ErrorMessage = "Value for {0} must be between {1} and {2}")]
    [Display(Description = "a")]
    [DefaultValue(2.2)]
    public double Attr { get; set; }
  }

#pragma warning disable CA1063 // Implement IDisposable Correctly
  public class SettingsConsturctServiceTest : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;

    public SettingsConsturctServiceTest()
    {
      _scopeFactoryMock = new Mock<IServiceScopeFactory>();
      var scopeMock = new Mock<IServiceScope>();
      var serviceCollection = new ServiceCollection();
       
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext();
      serviceCollection.AddScoped<IApplicationDbContext>(provider => _dbContext);
      scopeMock.Setup(s => s.ServiceProvider).Returns(serviceCollection.BuildServiceProvider());
      _scopeFactoryMock.Setup(s => s.CreateScope()).Returns(scopeMock.Object);
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
      _dbContext.Dispose();
    }

    [Fact]
    public async Task Constructor_WorksIfPropertyDoesNotHaveDefaultValueAttribute()
    {
      new SettingsService<SettingTestOnlyDisplayAttribute>(_scopeFactoryMock.Object, new NullLogger<SettingsService<SettingTestOnlyDisplayAttribute>>());
    }

    [Fact]
    public async Task Constructor_WorksIfPropertyDoesNotHaveDisplayAttribute()
    {
      new SettingsService<SettingTestOnlyDefaultValueAttribute>(_scopeFactoryMock.Object, new NullLogger<SettingsService<SettingTestOnlyDefaultValueAttribute>>());
    }
    [Fact]
    public async Task Constructor_WorksIfPropertyDoesNotHaveAttribute()
    {
      new SettingsService<SettingTestNoAttributes>(_scopeFactoryMock.Object, new NullLogger<SettingsService<SettingTestNoAttributes>>());
    }
    [Fact]
    public async Task Constructor_ThrowsIfDefaultValueDoesNotHaveRightType()
    {

      Action act = () => new SettingsService<SettingTestDefaultValueTypeMismatch>(_scopeFactoryMock.Object, new NullLogger<SettingsService<SettingTestDefaultValueTypeMismatch>>());

      var details = Assert.Throws<InvalidArgumentException>(act);
      Assert.Equal("Attr", details.ParamName);
      Assert.Contains("type", details.Message);
    }

    [Fact]
    public async Task Constructor_ThrowsIfDisplayDoesNotHaveDescription()
    {

      Action act = () => new SettingsService<SettingTestDefaultValueEmptyDescription>(_scopeFactoryMock.Object, new NullLogger<SettingsService<SettingTestDefaultValueEmptyDescription>>());

      var details = Assert.Throws<InvalidArgumentException>(act);
      Assert.Equal("Attr", details.ParamName);
      Assert.Contains("description", details.Message);
    }

    [Fact]
    public async Task Constructor_ThrowsIfDefaultValueIsNull()
    {

      Action act = () => new SettingsService<SettingTestDefaultValueNull>(_scopeFactoryMock.Object, new NullLogger<SettingsService<SettingTestDefaultValueNull>>());

      var details = Assert.Throws<InvalidArgumentException>(act);
      Assert.Equal("Attr", details.ParamName);
      Assert.Contains("null", details.Message);
    }

    [Fact]
    public async Task Constructor_AllowsIntRange()
    {

      new SettingsService<SettingTestIntRange>(_scopeFactoryMock.Object, new NullLogger<SettingsService<SettingTestIntRange>>());

    }


    [Fact]
    public async Task Constructor_ThrowsWhenIntValueLowerThanRangeLowerBound()
    {

      Action act = () => new SettingsService<SettingTestIntRangeLowerDefault>(_scopeFactoryMock.Object, new NullLogger<SettingsService<SettingTestIntRangeLowerDefault>>());

      var details = Assert.Throws<InvalidArgumentException>(act);
      Assert.Equal("Attr", details.ParamName);
      Assert.Contains("bewteen", details.Message);
    }

    [Fact]
    public async Task Constructor_ThrowsWhenIntValueLargerThanRangeUpperBound()
    {


      Action act = () => new SettingsService<SettingTestIntRangeHigherDefault>(_scopeFactoryMock.Object, new NullLogger<SettingsService<SettingTestIntRangeHigherDefault>>());

      var details = Assert.Throws<InvalidArgumentException>(act);
      Assert.Equal("Attr", details.ParamName);
      Assert.Contains("bewteen", details.Message);
    }

    [Fact]
    public async Task Constructor_ThrowsIfRangeHasStringType()
    {

      Action act = () => new SettingsService<SettingTestRangeStringType>(_scopeFactoryMock.Object, new NullLogger<SettingsService<SettingTestRangeStringType>>());

      var details = Assert.Throws<InvalidArgumentException>(act);
      Assert.Equal("Attr", details.ParamName);
      Assert.Contains(typeof(string).Name, details.Message);
    }

    [Fact]
    public async Task Constructor_ThrowsIfRangeHasDoubleType()
    {

      Action act = () => new SettingsService<SettingTestRangeDoubleType>(_scopeFactoryMock.Object, new NullLogger<SettingsService<SettingTestRangeDoubleType>>());

      var details = Assert.Throws<InvalidArgumentException>(act);
      Assert.Equal("Attr", details.ParamName);
      Assert.Contains(typeof(double).Name, details.Message);
    }
  }
}
