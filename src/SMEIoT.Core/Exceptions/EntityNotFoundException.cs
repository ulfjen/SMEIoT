using System;
using System.Runtime.Serialization;

namespace SMEIoT.Core.Exceptions
{
  public class EntityNotFoundException : ArgumentException 
  {
    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string message, string paramName)
      : base(message, paramName)
    {
    }

    public EntityNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
