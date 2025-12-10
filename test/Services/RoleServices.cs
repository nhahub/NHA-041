namespace test.Services
{
    using Microsoft.AspNetCore.Identity;
    using test.Models;

    public static class RoleServices
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            // 1. Get the RoleManager service
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 2. Define the role names you want to create
            string[] roleNames = { "Shelter", "User","Admin" };

            // 3. Loop through the role names
            foreach (var roleName in roleNames)
            {
                // 4. Check if the role already exists
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    // 5. If it doesn't exist, create it
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var adminEmail = "admin@happypaws.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
