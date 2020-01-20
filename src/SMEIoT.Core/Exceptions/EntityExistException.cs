using System;

namespace SMEIoT.Core.Exceptions
{
  public class EntityExistException : Exception 
  {
    public string ParamName { get; internal set; }
    
    public EntityExistException(string message, string paramName)
      : base(message)
    {
      ParamName = paramName;
    }
    
  }
}
