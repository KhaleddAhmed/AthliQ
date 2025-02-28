using AthliQ.Core.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Repository.Data.Configurations
{
    internal class ChildConfiguration : IEntityTypeConfiguration<Child>
    {
        public void Configure(EntityTypeBuilder<Child> builder)
        {
            builder.Property(C => C.Name)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.HasIndex(c => c.Name).IsUnique();

            builder.HasOne(C => C.AthliQUser)
                   .WithMany(U => U.Childs)
                   .HasForeignKey(C => C.AthliQUserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
