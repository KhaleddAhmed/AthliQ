using AthliQ.Core.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AthliQ.Repository.Data.Configurations
{
    internal class ChildResultConfiguration : IEntityTypeConfiguration<ChildResult>
    {
        public void Configure(EntityTypeBuilder<ChildResult> builder)
        {
            builder.HasKey(CR => new { CR.ChildId, CR.CategoryId });

            builder.HasOne(CR => CR.Category)
                   .WithMany(C => C.ChildCategories)
                   .HasForeignKey(CR => CR.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(CR => CR.Child)
                   .WithMany(C => C.ChildResults)
                   .HasForeignKey(CR => CR.ChildId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
