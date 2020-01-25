using Hangfire.Dashboard;

namespace SMEIoT.Web.Hangfire
{
  public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
  {
    public bool Authorize(DashboardContext context)
    {
      var httpContext = context.GetHttpContext();
      return httpContext.User.IsInRole("Admin");
    }
  }

}
