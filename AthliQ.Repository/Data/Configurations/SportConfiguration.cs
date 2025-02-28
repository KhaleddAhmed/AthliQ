using AthliQ.Core.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AthliQ.Repository.Data.Configurations
{
    internal class SportConfiguration : IEntityTypeConfiguration<Sport>
    {
        public void Configure(EntityTypeBuilder<Sport> builder)
        {
            builder.Property(S => S.Id).UseIdentityColumn(10, 10);

            builder.Property(S => S.Name)
                  .IsRequired()
                  .HasMaxLength(30);

            builder.HasIndex(S => S.Name).IsUnique();

            builder.Property(S => S.ArabicName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(S => S.ArabicName).IsUnique();
        }
    }
}
