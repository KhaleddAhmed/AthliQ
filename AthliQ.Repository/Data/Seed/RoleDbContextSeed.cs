using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AthliQ.Repository.Data.Seed
{
    public static class RoleDbContextSeed
    {
        public static async Task SeedRoleAsync(RoleManager<IdentityRole> _roleManager)
        {
            if (_roleManager.Roles.Count() == 0)
            {
                var Adminrole = new IdentityRole() { Name = "Admin" };

                var userRole = new IdentityRole() { Name = "User" };

                await _roleManager.CreateAsync(Adminrole);
                await _roleManager.CreateAsync(userRole);
            }
        }
    }
}
