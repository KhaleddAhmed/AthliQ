using AthliQ.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace AthliQ.Repository.Data.Seed
{
    public static class AdminDbContextSeed
    {
        public static async Task SeedAdminAsync(UserManager<AthliQUser> userManager)
        {
            if (userManager.Users.Count() == 0)
            {
                var email = "Admin.Support@gmail.com";
                var admin = new AthliQUser()
                {
                    Email = email,
                    UserName = email.Split("@")[0],
                    FirstName = "Admin",
                    LastName = "Athliq",
                    Address = "12-Dokki-Giza-Egypt",
                    PhoneNumber = "01012313987",
                    Gender = "Male"
                };

                await userManager.CreateAsync(admin, "P@ssw0rd");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
