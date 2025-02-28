using AthliQ.Core.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AthliQ.Repository.Data.Configurations
{
    internal class TestConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            builder.Property(T => T.Name)
                  .IsRequired()
                  .HasMaxLength(50);

            builder.HasIndex(T => T.Name).IsUnique();

            builder.Property(T => T.ArabicName)
                   .IsRequired()
                   .HasMaxLength(80);

            builder.HasIndex(T => T.ArabicName).IsUnique();
        }
    }
}
