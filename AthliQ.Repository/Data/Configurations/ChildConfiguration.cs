using AthliQ.Core.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AthliQ.Repository.Data.Configurations
{
    internal class ChildConfiguration : IEntityTypeConfiguration<Child>
    {
        public void Configure(EntityTypeBuilder<Child> builder)
        {
            builder.Property(C => C.Name)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.HasIndex(c => new { c.AthliQUserId, c.Name }).IsUnique();

            builder.HasOne(C => C.AthliQUser)
                   .WithMany(U => U.Childs)
                   .HasForeignKey(C => C.AthliQUserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
