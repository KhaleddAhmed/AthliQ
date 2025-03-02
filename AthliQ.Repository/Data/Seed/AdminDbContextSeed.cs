using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var email = "Ahmed.Abbas@gmail.com";
                var admin = new AthliQUser()
                {
                    Email = email,
                    UserName = email.Split("@")[0],
                    FirstName = "Ahmed",
                    LastName = "Abbas",
                    Address = "12-Dokki-Giza-Egypt",
                    PhoneNumber = "01012313987",
                    Gender = "Male",
                    IsAccepted = true,
                };

                await userManager.CreateAsync(admin, "P@ssw0rd");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
