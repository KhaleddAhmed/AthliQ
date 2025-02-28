using AthliQ.Core.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AthliQ.Repository.Data.Configurations
{
    internal class UserClubConfiguration : IEntityTypeConfiguration<UserClub>
    {
        public void Configure(EntityTypeBuilder<UserClub> builder)
        {
            builder.HasKey(UC => new { UC.AthliQUserId, UC.ClubName });

            builder.HasOne(UC => UC.User)
                   .WithMany(U => U.UserClubs)
                   .HasForeignKey(UC => UC.AthliQUserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
