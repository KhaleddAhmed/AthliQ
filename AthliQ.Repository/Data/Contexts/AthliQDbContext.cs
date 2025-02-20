using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AthliQ.Repository.Data.Contexts
{
    public class AthliQDbContext : IdentityDbContext<AthliQUser>
    {
        public AthliQDbContext(DbContextOptions<AthliQDbContext> dbContextOptions)
            : base(dbContextOptions) { }
    }
}
