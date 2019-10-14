using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Web.BindingModels
{
  public class UserRolesBindingModel
  {
    [Required]
    public IEnumerable<string> Roles;
  }
}
