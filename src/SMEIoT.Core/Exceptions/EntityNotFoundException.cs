using System;

namespace SMEIoT.Core.Exceptions
{
  public class EntityNotFoundException : Exception 
  {
    public string ParamName { get; internal set; }
    
    public EntityNotFoundException(string message, string paramName)
      : base(message)
    {
      ParamName = paramName;
    }
    
  }
}
