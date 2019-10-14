using System.Collections.Generic;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.ApiModels
{
  public class AdminUserApiModelList
  {
    public IEnumerable<AdminUserApiModel> Users { get; set; }
  }
}
