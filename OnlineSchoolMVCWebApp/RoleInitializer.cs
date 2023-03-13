using Microsoft.AspNetCore.Identity;
using OnlineSchoolMVCWebApp.Models;
using Task = System.Threading.Tasks.Task;

namespace OnlineSchoolMVCWebApp
{
    public class RoleInitializer
    {

        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {

            string adminEmail = configuration.GetSection(SettingStrings.AdminSettingsSection).GetSection(SettingStrings.Email).Value;
            string password = configuration.GetSection(SettingStrings.AdminSettingsSection).GetSection(SettingStrings.Password).Value;

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("author") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("author"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail, FirstName = adminEmail, LastName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }

    }
}
