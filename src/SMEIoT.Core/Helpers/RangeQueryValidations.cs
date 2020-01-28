using SMEIoT.Core.Exceptions;

namespace SMEIoT.Core.Helpers
{
  public static class RangeQueryValidations
  {
    public static void ValidateRangeQueryParameters(int offset, int limit)
    {
      if (offset < 0)
      {
        throw new InvalidArgumentException("Offset can't be negative", nameof(offset));
      }
      if (limit < 0)
      {
        throw new InvalidArgumentException("limit can't be negative", nameof(limit)); 
      }
    }
  }
}
