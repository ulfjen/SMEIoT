using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SMEIoT.Views.Helpers
{
  public static class ValidationJsonResultExtension
  {
    public static object JsonValidation(this ModelStateDictionary state)
    {
      return from e in state
        where e.Value.Errors.Count > 0
        select new
        {
          name = e.Key,
          errors = e.Value.Errors.Select(x => x.ErrorMessage)
            .Concat(e.Value.Errors.Where(x => x.Exception != null).Select(x => x.Exception.Message))
        };
    }
  }
}
