using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.DataSeeding
{
    public static class IdentityDataSeeding
    {
        public static async Task SeedIdentityDataAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger logger,
            CancellationToken ct = default
            )
        {
            try
            {
                var hasUsers = await userManager.Users.AnyAsync();
                var hasRoles = await roleManager.Roles.AnyAsync();

                if (hasUsers && hasRoles) return;

                if (!hasRoles)
                {
                    var roles = new List<IdentityRole>()
                {
                    new IdentityRole("SuperAdmin"),
                    new IdentityRole("Admin"),
                };
                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role.Name))
                        {
                            await roleManager.CreateAsync(role);
                        }
                    }
                }

                if (!hasUsers)
                {
                    var superAdmin = new ApplicationUser()
                    {
                        FirstName = "Ahmed",
                        lastName = "Omar",
                        UserName = "ahmedOmar",
                        Email = "ahmedOmar123@gmail.com",
                        PhoneNumber = "01001111111"
                    };
                    await userManager.CreateAsync(superAdmin, "P@ssW0rd");
                    await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");

                    var admin = new ApplicationUser()
                    {
                        FirstName = "Mohamed",
                        lastName = "Ahmed",
                        UserName = "mohamedAhmed",
                        Email = "mohamedAhmed123@gmail.com",
                        PhoneNumber = "01101111111"
                    };
                    await userManager.CreateAsync(admin, "P@ssW0rd");
                    await userManager.AddToRoleAsync(admin, "Admin");

                }
            }
            catch( Exception ex )
            {
                logger.LogError(ex.Message);
            }
        }
    }
}
