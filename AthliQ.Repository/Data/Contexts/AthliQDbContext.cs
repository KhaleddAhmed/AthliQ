using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.Entities;
using AthliQ.Core.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AthliQ.Repository.Data.Contexts
{
    public class AthliQDbContext : IdentityDbContext<AthliQUser>
    {
        public AthliQDbContext(DbContextOptions<AthliQDbContext> dbContextOptions)
            : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<UserClub> UserClubs { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<ChildTest> ChildTests { get; set; }
        public DbSet<ChildResult> ChildResults { get; set; }

    }
}
