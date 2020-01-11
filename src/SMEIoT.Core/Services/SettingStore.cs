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
  public class SettingStore<T> : ISettingStore<T> where T : SettingsBase, new()
  {
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger _logger;

    public SettingStore(IApplicationDbContext dbContext, ILogger<SettingStore<T>> logger)
    {
      _dbContext = dbContext;
      _logger = logger;
    }

    public Task SerializeToSettingItemAsync(T settings, string name, object value)
    {
      throw new NotImplementedException();
    }

    private async Task<Dictionary<string, SettingItem>> PrepareItemByNameAsync()
    {
      var items = await _dbContext.SettingItems.ToListAsync();
      var itemByName = new Dictionary<string, SettingItem>();
      foreach (var i in items) {
        itemByName[i.Name] = i;
      }
      return itemByName;
    }

    public async Task SerializeToStorageAsyncWithoutValidation(T instance, IList<PropertyInfo> properties)
    {
      var itemByName = await PrepareItemByNameAsync();
      var names = itemByName.Keys;
      var defaultInstance = new T();
      var properties = SettingPropertyHelpers.GetProperties<T>();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(instance);
        var item = itemByName[prop.Name];
        val stored = item.DeserializeDataToObject();

        if (names.Contains(prop.Name) && !value.Equals(stored)) {
          item.Data = SettingItem.SerializeObjectToByteArray(value);
          item.Type = SettingPropertyHelpers.GetPropertyTypeName(prop);
          _dbContext.SettingItems.Update(item);
        } else if (!value.Equals(prop.GetValue(defaultInstance))) {
          _dbContext.SettingItems.Add(new SettingItem {
            Name = prop.Name,
            Data = SettingItem.SerializeObjectToByteArray(value),
            Type = SettingPropertyHelpers.GetPropertyTypeName(prop)
          });
        }
      }
      await _dbContext.SaveChangesAsync();
    }

    private object GetDefaultValueFromAttribute(PropertyInfo prop)
    {
      var attr = SettingAttributeHelpers.GetDefaultValueAttribute(prop);
      return SettingAttributeHelpers.GetDefaultValueFromAttribute(prop, attr!);
    }

    private object GetValueForProperty(PropertyInfo property, object instance)
    {
      var val = property.GetValue(instance);
      return val!;
    }

    // discard value from db with a warning if value not validated
    public async Task DeserializeFromStorageAsync(T instance)
    {
      var itemByName = await PrepareItemByNameAsync();
      var names = itemByName.Keys;
      var properties = SettingPropertyHelpers.GetProperties<T>();

      foreach (var prop in properties)
      {
        var item = itemByName[prop.Name];
        val stored = item.DeserializeDataToObject();

        if (!names.Contains(prop.Name)) { continue; }
        
        // validates this value from db
        try {
          SettingPropertyHelpers.ValidateValueForProperty(prop, stored);
        } catch {
          _logger.LogWarning($"{prop.Name} has an invalid value in the database.");
        } 
        prop.SetValue(instance, stored);
      }
    }
  }
}
