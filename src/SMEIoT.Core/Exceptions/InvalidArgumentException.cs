using System;
using System.Runtime.Serialization;

namespace SMEIoT.Core.Exceptions
{
  public class InvalidArgumentException : ArgumentException 
  {
    public InvalidArgumentException()
    {
    }

    public InvalidArgumentException(string message, string paramName)
      : base(message, paramName)
    {
    }

    public InvalidArgumentException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected InvalidArgumentException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
