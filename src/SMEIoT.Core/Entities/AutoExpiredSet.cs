using System.Collections.Concurrent;
using System.Collections.Generic;
using NodaTime;

namespace SMEIoT.Core.Entities
{
  public class AutoExpiredSet<T>
  {
    private readonly ConcurrentDictionary<T, Instant> _set = new ConcurrentDictionary<T, Instant>();

    public Duration ExpirePeriod = Duration.FromMinutes(15);

    public IEnumerable<T> List(Instant now)
    {
      var keys = _set.Keys;
      var expired = new List<T>();
      var result = new List<T>();
      Instant instant;
      foreach (var key in keys)
      {
        _set.TryGetValue(key, out instant);
        if (instant + ExpirePeriod >= now)
        {
          result.Add(key);
        }
        else
        {
          expired.Add(key);
        }
      }
      foreach (var key in expired)
      {
        _set.TryRemove(key, out instant);
      }
      return result;
    }

    public bool ContainsKey(T item)
    {
      return _set.ContainsKey(item);
    }

    public bool TryAdd(T item, Instant now)
    {
      return _set.TryAdd(item, now);
    }
  }
}
