using System;

namespace SMEIoT.Core.Exceptions
{
  public class InvalidArgumentException : Exception 
  {
    public string ParamName { get; internal set; }
    
    public InvalidArgumentException(string message, string paramName)
      : base(message)
    {
      ParamName = paramName;
    }
  }
}
