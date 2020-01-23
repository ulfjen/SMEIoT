using System;

namespace SMEIoT.Core.Entities
{
  public abstract class SettingsBase
  {
    // throws if some conditions not met. If some properties need more validation logic,
    // this should be the place to implement them.
    public virtual void ValidateItems()
    {
    }
  }
}
