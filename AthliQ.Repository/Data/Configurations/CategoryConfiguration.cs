using AthliQ.Core.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AthliQ.Repository.Data.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {

            builder.Property(C => C.Id).UseIdentityColumn(100, 100);

            builder.Property(C => C.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(c => c.Name).IsUnique();

            builder.Property(C => C.ArabicName)
                   .IsRequired()
                   .HasMaxLength(80);

            builder.HasIndex(c => c.ArabicName).IsUnique();

            builder.HasMany(C => C.Sports)
                   .WithOne(S => S.Category)
                   .HasForeignKey(S => S.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(C => C.Tests)
                   .WithOne(T => T.Category)
                   .HasForeignKey(T => T.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
