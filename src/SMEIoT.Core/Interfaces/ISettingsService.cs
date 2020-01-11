using System.Threading.Tasks;
using System.Collections.Generic;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  // thread safe singleton
  public interface ISettingsService<T> where T : SettingsBase, new()
  {
    // Get site settings in memory. Or load items from db
    Task<T> GetSettingsAsync();

    // list all setting items with DefaultValueAttribute
    Task<IEnumerable<SettingItemDescriptor>> ListSettingDescriptorsAsync();

    // Send updates to the db and then refresh the setting we cached.
    Task UpdateSettingsAsync(T settings);

    Task UpdateSettingItemAsync(string name, string value);
  }
}
