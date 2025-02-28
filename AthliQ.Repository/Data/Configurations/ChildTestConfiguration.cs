using AthliQ.Core.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AthliQ.Repository.Data.Configurations
{
    internal class ChildTestConfiguration : IEntityTypeConfiguration<ChildTest>
    {
        public void Configure(EntityTypeBuilder<ChildTest> builder)
        {
            builder.HasKey(CT => new { CT.ChildId, CT.TestId });

            builder.HasOne(CT => CT.Test)
                   .WithMany(T => T.TestChilderen)
                   .HasForeignKey(CT => CT.TestId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(CT => CT.Child)
                    .WithMany(C => C.ChildTests)
                    .HasForeignKey(CT => CT.ChildId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
