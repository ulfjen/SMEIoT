using System;

namespace SMEIoT.Core.Exceptions
{
  public class InvalidUserInputException : Exception 
  {
    public InvalidUserInputException(string message)
      : base(message)
    {
    }
    
  }
}
