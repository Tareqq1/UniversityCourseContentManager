using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace LMS_DEPI.APP.Roles
{
    public class SeedingRoles
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User", "Teacher" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Create role if it doesn't exist
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
