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
  public class SettingTestOnlyDisplayAttribute : Settings
  {
    [Display(Description = "a")]
    public int Attr { get; set; }
  }

  [Collection("Database collection")]
#pragma warning disable CA1063 // Implement IDisposable Correctly
  public class SettingsServiceTest : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly SettingsService<Settings> _service;
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<IServiceScope> _scopeMock;

    public SettingsServiceTest()
    {
      _scopeFactoryMock = new Mock<IServiceScopeFactory>();
      _scopeMock = new Mock<IServiceScope>();
      var serviceCollection = new ServiceCollection();

      _dbContext = ApplicationDbContextHelper.BuildTestDbContext();
      serviceCollection.AddScoped<IApplicationDbContext>(provider => ApplicationDbContextHelper.BuildTestDbContext());
      _scopeMock.Setup(s => s.ServiceProvider).Returns(() => serviceCollection.BuildServiceProvider());
      _scopeFactoryMock.Setup(s => s.CreateScope()).Returns(_scopeMock.Object);
      
      _service = new SettingsService<Settings>(_scopeFactoryMock.Object, new NullLogger<SettingsService<Settings>>());
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
      _dbContext.Database.ExecuteSqlInterpolated($"TRUNCATE setting_items CASCADE;");
      _dbContext.Dispose();
    }

    [Fact]
    public async Task GetSettingsAsync_ReturnsSettingsWithDefault()
    {
      // arrange
      
      // act
      var settings = await _service.GetSettingsAsync();
      
      // assert
      Assert.Equal(500, settings.MosquittoClientRunLoopInterval);
    }
    
    [Fact]
    public async Task GetSettingsAsync_CanBeAccessedFromAnotherThreads()
    {
      var settings = await _service.GetSettingsAsync();
      var threads = new List<Thread>();
      for (int i = 0; i < 10; ++i) {
        threads.Add(new Thread(async (object? state) => {
          var settingsInAnotherThread = await _service.GetSettingsAsync();
          Assert.Equal(settingsInAnotherThread, settings);
        }));
      }

      foreach (var t in threads) { t.Start(); }

      foreach (var t in threads) { t.Join(); }
    }

    [Fact]
    public async Task GetSettingsAsync_GetIntItemFromDb()
    {
      _dbContext.SettingItems.Add(new SettingItem {
        Name = "MosquittoClientRunLoopInterval",
        Data = SettingItem.SerializeObjectToByteArray(675),
        Type = typeof(int).Name
      });
      await _dbContext.SaveChangesAsync();
      
      var settings = await _service.GetSettingsAsync();

      Assert.Equal(675, settings.MosquittoClientRunLoopInterval);
    }

    [Fact]
    public async Task GetSettingsAsync_ResolvesScopeWhenNotInitialized()
    {
      
      var settings = await _service.GetSettingsAsync();

      _scopeMock.Verify(s => s.ServiceProvider, Times.Once());
      _scopeFactoryMock.Verify(s => s.CreateScope(), Times.Once());
    }

    [Fact]
    public async Task UpdateSettingsAsync_DoNothingWhileNoChangesAreMade()
    {
      var settings = await _service.GetSettingsAsync();

      await _service.UpdateSettingsAsync(settings);

      Assert.Equal(0, await _dbContext.SettingItems.CountAsync());
    }

    [Fact]
    public async Task UpdateSettingsAsync_StoreIntItemWhenChanges()
    {
      var settings = await _service.GetSettingsAsync();
      settings.MosquittoClientRunLoopInterval = 675;

      await _service.UpdateSettingsAsync(settings);

      var first = await _dbContext.SettingItems.FirstOrDefaultAsync();
      Assert.Equal(1, await _dbContext.SettingItems.CountAsync());
      Assert.Equal("MosquittoClientRunLoopInterval", first.Name);
      Assert.Equal(675, (int)first.DeserializeDataToObject());
      Assert.Equal("Int32", first.Type);
    }

    [Fact]
    public async Task UpdateSettingsAsync_ThrowsWhenIntValueLowerThanRangeLowerBound()
    {
      var settings = await _service.GetSettingsAsync();
      settings.MosquittoClientRunLoopInterval = -1;

      Task Act() => _service.UpdateSettingsAsync(settings);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("MosquittoClientRunLoopInterval", details.ParamName);
      Assert.Contains("between", details.Message);
    }

    [Fact]
    public async Task UpdateSettingsAsync_ThrowsWhenIntValueLargerThanRangeUpperBound()
    {
      var settings = await _service.GetSettingsAsync();
      settings.MosquittoClientRunLoopInterval = 50000000;

      Task Act() => _service.UpdateSettingsAsync(settings);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("MosquittoClientRunLoopInterval", details.ParamName);
      Assert.Contains("between", details.Message);
    }

    [Fact]
    public async Task ListSettingDescriptorsAsync_ListsSettings()
    {

      var descriptors = await _service.ListSettingDescriptorsAsync();
      
      var descriptor = descriptors.Where(n => n.Name == "MosquittoClientRunLoopInterval").SingleOrDefault();
      Assert.Equal("MosquittoClientRunLoopInterval", descriptor.Name);
      Assert.Contains("Milliseconds", descriptor.Description);
      Assert.Equal("Int32", descriptor.Type);
      Assert.Equal((object)500, descriptor.DefaultValue);
      Assert.Equal((object)500, descriptor.Value);
    }

    [Fact]
    public async Task ListSettingDescriptorsAsync_DoesNotListSettingsWithoutDefaultValue()
    {
      var service = new SettingsService<SettingTestOnlyDisplayAttribute>(_scopeFactoryMock.Object, new NullLogger<SettingsService<SettingTestOnlyDisplayAttribute>>());

      var descriptors = await service.ListSettingDescriptorsAsync();
      
      var descriptor = descriptors.Where(n => n.Name == "Attr").SingleOrDefault();
      Assert.Null(descriptor);
    }

    [Fact]
    public async Task UpdateSettingItemAsync_ResolvesScopeWhenNotInitialized()
    {
      var settings = await _service.GetSettingsAsync();
      
      await _service.UpdateSettingItemAsync("MosquittoClientRunLoopInterval", "500");

      _scopeMock.Verify(s => s.ServiceProvider, Times.Once());
      _scopeFactoryMock.Verify(s => s.CreateScope(), Times.Once());
    }
  }
}
