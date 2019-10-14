using System;
using System.Runtime.Serialization;

namespace SMEIoT.Core.Exceptions
{
  public class IncorrectUnitOfWorkUsageException : InvalidOperationException
  {
    public IncorrectUnitOfWorkUsageException()
    {
    }

    public IncorrectUnitOfWorkUsageException(string message)
      : base(message)
    {
    }

    public IncorrectUnitOfWorkUsageException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected IncorrectUnitOfWorkUsageException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
