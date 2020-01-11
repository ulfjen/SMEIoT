using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface ISettingStore<T> where T : SettingsBase, new()
  {
    // users must ensure instance is not changed along this function call
    Task DeserializeFromStorageAsync(T instance);

    // does not validates the value in the instance and properties
    Task SerializeToStorageAsyncWithoutValidation(T settings);
    Task SerializeToSettingItemAsync(T settings, string name, object value);
  }
}
