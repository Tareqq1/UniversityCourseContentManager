using Microsoft.AspNetCore.Identity;

namespace LMS_DEPI.APP.Roles
{
    public class SeedingRoles
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User", "Teacher" }; // Define your roles
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName)); // Create role
                }
            }
        }

    }
}
