using System;
using System.Reflection;
using System.Linq;
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
using NodaTime;
using NodaTime.Testing;

namespace SMEIoT.Tests.Core.Services
{
  public class TestSetting : SettingsBase
  {
    [Range(0, 5000, ErrorMessage = "Setting for {0} must be between {1} and {2}.")]
    [Display(Description = "intopt")]
    public int IntOpt { get; set; } = 500;

    [Display(Description = "boolopt")]
    public bool BoolOpt { get; set; } = true;

    [StringLength(10240, ErrorMessage = "Setting for {0} must be shorter than {1}.", MinimumLength = 10)]
    [Display(Description = "stropt")]
    public string StrOpt { get; set; } = "strdefaultvalue";
  }

  [Collection("Database collection")]
#pragma warning disable CA1063 // Implement IDisposable Correctly
  public class SettingStoreTest : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly Instant _initial;
    private readonly FakeClock _clock;
    private readonly SettingStore<TestSetting> _store;
    private readonly IList<PropertyInfo> _filteredProps;
    private TestSetting _settings;

    public SettingStoreTest()
    {
      _initial = SystemClock.Instance.GetCurrentInstant();
      _clock = new FakeClock(_initial);
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext(_clock);
      _store = new SettingStore<TestSetting>(_dbContext, new NullLogger<SettingStore<TestSetting>>());
      _settings = new TestSetting {
        IntOpt = 500,
        BoolOpt = true,
        StrOpt = "strdefaultvalue"
      };
      _filteredProps = SettingPropertyHelpers.GetProperties(typeof(TestSetting));
      _filteredProps = SettingPropertyHelpers.FilterProperties(_filteredProps);
      SettingPropertyHelpers.ValidateProperties(_filteredProps);
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
      _dbContext.Database.ExecuteSqlInterpolated($"TRUNCATE setting_items CASCADE;");
      _dbContext.Dispose();
    }

    [Fact]
    public async Task DeserializeFromStorageAsync_ReturnsInstanceWithValuesFromDatabase()
    {
      _dbContext.SettingItems.Add(new SettingItem {
        Name = "IntOpt",
        Data = SettingItem.SerializeObjectToByteArray(600),
        Type = typeof(int).Name
      });
      _dbContext.SettingItems.Add(new SettingItem {
        Name = "BoolOpt",
        Data = SettingItem.SerializeObjectToByteArray(false),
        Type = typeof(bool).Name
      });
      _dbContext.SettingItems.Add(new SettingItem {
        Name = "StrOpt",
        Data = SettingItem.SerializeObjectToByteArray("strstoreddbvalue"),
        Type = typeof(string).Name
      });
      await _dbContext.SaveChangesAsync();
      var instance = new TestSetting();
      
      await _store.DeserializeFromStorageAsync(instance);
      
      Assert.Equal(600, instance.IntOpt);
      Assert.False(instance.BoolOpt);
      Assert.Equal("strstoreddbvalue", instance.StrOpt);
    }

    [Fact]
    public async Task DeserializeFromStorageAsync_SetWithDefaultWithoutRecords()
    {
      var instance = new TestSetting();
      
      await _store.DeserializeFromStorageAsync(instance);

      Assert.Equal(500, _settings.IntOpt);
      Assert.True(_settings.BoolOpt);
      Assert.Equal("strdefaultvalue", _settings.StrOpt);
    }
    
    [Fact]
    public async Task SerializeToStorageAsyncWithoutValidation_UpdatesItemWhenThereIsSettingItemInDb()
    {
      _dbContext.SettingItems.Add(new SettingItem {
        Name = "StrOpt",
        Data = SettingItem.SerializeObjectToByteArray("strdefaultvalue"),
        Type = typeof(string).Name
      });
      await _dbContext.SaveChangesAsync();
      _settings.StrOpt = "updatedstrvalue";

      await _store.SerializeToStorageAsyncWithoutValidation(_settings, _filteredProps);

      var item = _dbContext.SettingItems.FirstOrDefault(s => s.Name == "StrOpt");
      Assert.NotNull(item);
      Assert.Equal("updatedstrvalue", item.DeserializeDataToObject());
    }

    [Fact]
    public async Task SerializeToStorageAsyncWithoutValidation_AddItemIfDifferentThanDefaultAndNotInDb()
    {
      _settings.BoolOpt = false;

      await _store.SerializeToStorageAsyncWithoutValidation(_settings, _filteredProps);

      var item = _dbContext.SettingItems.FirstOrDefault(s => s.Name == "BoolOpt");
      Assert.NotNull(item);
      Assert.Equal(1, await _dbContext.SettingItems.CountAsync());
      Assert.Equal(SettingItem.SerializeObjectToByteArray(false), item.Data);
    }

    [Fact]
    public async Task SerializeToStorageAsyncWithoutValidation_DoesNotStoreWhenItIsDefaultAndNotInDb()
    {

      await _store.SerializeToStorageAsyncWithoutValidation(_settings, _filteredProps);

      Assert.Empty(await _dbContext.SettingItems.ToListAsync());
    }

    [Fact(Skip = "Not implemented")]
    public async Task SerializeToSettingItemAsync_SetItemWithoutRecord()
    {
      await _store.SerializeToSettingItemAsync(_settings, "IntOpt", 12);

      var item = _dbContext.SettingItems.FirstOrDefault(s => s.Name == "IntOpt");
      Assert.NotNull(item);
      Assert.Equal(1, await _dbContext.SettingItems.CountAsync());
      Assert.Equal(SettingItem.SerializeObjectToByteArray(12), item.Data);
    }

    [Fact(Skip = "Not implemented")]
    public async Task SerializeToSettingItemAsync_SetItemWithRecord()
    {
      _dbContext.SettingItems.Add(new SettingItem {
        Name = "IntOpt",
        Data = SettingItem.SerializeObjectToByteArray(234),
        Type = typeof(int).Name
      });
      await _dbContext.SaveChangesAsync();
      
      await _store.SerializeToSettingItemAsync(_settings, "IntOpt", 25);

      var item = _dbContext.SettingItems.FirstOrDefault(s => s.Name == "IntOpt");
      Assert.NotNull(item);
      Assert.Equal(1, await _dbContext.SettingItems.CountAsync());
      Assert.Equal(SettingItem.SerializeObjectToByteArray(25), item.Data);
    }
  }
}
