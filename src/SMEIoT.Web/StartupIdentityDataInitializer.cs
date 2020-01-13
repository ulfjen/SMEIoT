using Microsoft.AspNetCore.Identity;

namespace SMEIoT.Web
{
  public class StartupIdentityDataInitializer
  {
    public static void SeedRoles(RoleManager<IdentityRole<long>> roleManager)
    {
      if (!roleManager.RoleExistsAsync("Admin").Result)
      {
        var role = new IdentityRole<long>();
        role.Name = "Admin";
        IdentityResult roleResult = roleManager.CreateAsync(role).Result;
      }
    }
  }
}
