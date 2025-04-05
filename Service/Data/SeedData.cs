using Service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Создание ролей
                string[] roles = { "Admin", "Operator", "Engineer" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Создание администратора
                var adminEmail = "admin@example.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Admin"
                    };
                    var result = await userManager.CreateAsync(admin, "Admin123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }

                // Создание оператора
                var operatorEmail = "operator@example.com";
                if (await userManager.FindByEmailAsync(operatorEmail) == null)
                {
                    var operatorUser = new ApplicationUser
                    {
                        UserName = operatorEmail,
                        Email = operatorEmail,
                        FullName = "Operator"
                    };
                    var result = await userManager.CreateAsync(operatorUser, "Operator123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(operatorUser, "Operator");
                    }
                }

                // Создание инженера
                var engineerEmail = "engineer@example.com";
                if (await userManager.FindByEmailAsync(engineerEmail) == null)
                {
                    var engineer = new ApplicationUser
                    {
                        UserName = engineerEmail,
                        Email = engineerEmail,
                        FullName = "Engineer"
                    };
                    var result = await userManager.CreateAsync(engineer, "Engineer123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(engineer, "Engineer");
                    }
                }
            }
        }
    }
}