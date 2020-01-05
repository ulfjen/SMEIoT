using System;

namespace SMEIoT.Core.Exceptions
{
  public class InternalException : Exception 
  {
    public InternalException(string message)
      : base(message)
    {
    }
    
  }
}
