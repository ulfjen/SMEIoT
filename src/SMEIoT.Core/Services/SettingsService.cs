using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Helpers;
using NodaTime;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SMEIoT.Core.Services
{
  public class SettingsService<T> : ISettingsService<T> where T : SettingsBase, new()
  {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;
    private IList<PropertyInfo> _properties;
    private T? _settingsInstance;
    private object _object = new Object();
    private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

    public SettingsService(IServiceScopeFactory scopeFactory, ILogger<SettingsService<T>> logger)
    {
      _scopeFactory = scopeFactory;
      _logger = logger;

      var type = typeof(T);
      _properties = SettingPropertyHelpers.GetProperties(type);
      _properties = SettingPropertyHelpers.FilterProperties(_properties);
      SettingPropertyHelpers.ValidateProperties(_properties);
    }

    public async Task<IEnumerable<SettingItemDescriptor>> ListSettingDescriptorsAsync()
    {
      var instance = await GetSettingsAsync();

      return _properties.Select(prop => {
        var display = prop.GetCustomAttribute<DisplayAttribute>();
        if (display == null) { throw new InternalException($"Our setting's property {prop.Name} is not filtered when construct."); }
        return new SettingItemDescriptor {
          Name = prop.Name,
          Type = GetPropertyTypeNameForProperty(prop),
          Description = display.Description,
<<<<<<< HEAD
          DefaultValue = GetDefaultValueForProperty(prop),
          Value = prop.GetValue(instance)
=======
          DefaultValue = GetDefaultValueFromAttribute(prop).ToString(),
          Value = GetValueForProperty(prop, instance).ToString()
>>>>>>> d66be84... extract implementation from settings services and replace with helpers
        };
      });
    }

    public Task UpdateSettingItemAsync(string name, string value)
    {
      throw new NotImplementedException();
    }

    public async Task<T> GetSettingsAsync()
    {
      if (_settingsInstance != null) {
        return _settingsInstance;
      }

      await _semaphore.WaitAsync();
      try {
        // might be initialized by others while waiting.
        if (_settingsInstance != null) {
          return _settingsInstance;
        }
        
        _settingsInstance = new T();
        using var scope = _scopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetService<IApplicationDbContext>();

        var items = await dbContext.SettingItems.ToListAsync();
        var storedSettingNames = new HashSet<string>(items.Select(s => s.Name));

        foreach (var prop in _properties)
        {
          if (storedSettingNames.Contains(prop.Name)) {
            prop.SetValue(_settingsInstance, items.SingleOrDefault(i => i.Name == prop.Name).DeserializeDataToObject());
          } else { // property has DefaultValueAttribute
            prop.SetValue(_settingsInstance, GetDefaultValueFromAttribute(prop));
          }
        }
        return _settingsInstance;
      } finally {
        _semaphore.Release();
      }
    }

    public async Task UpdateSettingsAsync(T settings)
    {
      throw new NotImplementedException();

      var instance = await GetSettingsAsync();
      foreach (var prop in _properties)
      {
        var binded = prop.GetValue(settings);
        var val = prop.GetValue(instance); 
        var range = prop.GetCustomAttribute<RangeAttribute>();
        if (range != null) {
          // ValidateValueInRange(prop, range, binded);
        }
      }

      using var scope = _scopeFactory.CreateScope();
      using var dbContext = scope.ServiceProvider.GetService<IApplicationDbContext>();

      var items = await dbContext.SettingItems.ToListAsync();
      var storedSettingNames = new HashSet<string>(items.Select(s => s.Name));

      using (var transaction = dbContext.Database.BeginTransaction())
      {
        foreach (var prop in _properties)
        {
          var val = prop.GetValue(settings);

          if (storedSettingNames.Contains(prop.Name)) {
            dbContext.SettingItems.Update(new SettingItem {
              Name = prop.Name,
              Data = SettingItem.SerializeObjectToByteArray(val),
              Type = GetPropertyTypeNameForProperty(prop)
            });
          } else if (!val.Equals(GetDefaultValueFromAttribute(prop))) {
            dbContext.SettingItems.Add(new SettingItem {
              Name = prop.Name,
              Data = SettingItem.SerializeObjectToByteArray(val),
              Type = GetPropertyTypeNameForProperty(prop)
            });
          }
        }
        await dbContext.SaveChangesAsync();
        transaction.Commit();
      }

      foreach (var prop in _properties)
      {
        var val = prop.GetValue(settings);
        prop.SetValue(instance, val);
      }
    }

    private object? GetDefaultValueForProperty(PropertyInfo property)
    {
      var defaultAttr = property.GetCustomAttribute<DefaultValueAttribute>();
      if (defaultAttr == null) { return null; }
      var value = defaultAttr.Value;
      return value;
    }

    // ensure we uses the same type name
    private string GetPropertyTypeNameForProperty(PropertyInfo property)
    {
      return property.PropertyType.Name;
    }

    private T GetSettingsFromDb(bool throwConversionFailure = false)
    {
      var s = new T();
      var type = typeof(T);

      var items = _dbContext.SettingItems.ToList();
      foreach (var item in items)
      {
        try
        {
          var info = type.GetProperty(item.Name);
          info.SetValue(s, item.DeserializeDataToObject());
        }
        catch (ArgumentNullException ex)
        {
          _logger.LogWarning($"When removes a setting {item.Name}, the exception happens. {ex.Message}");
        }
        catch (Exception)
        {
          if (throwConversionFailure) {
            throw;
          }
          // just use default if not throw
        }
      }
      return s;
    }

  }
}
