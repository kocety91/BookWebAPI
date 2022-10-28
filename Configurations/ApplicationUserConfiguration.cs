using BookWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookWebAPI.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.Country).IsRequired();
            builder.HasOne(x => x.ApplicationRole)
                .WithOne(r => r.ApplicationUser)
                .HasForeignKey<ApplicationUser>(x => x.ApplicationRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Books)
                .WithOne(b => b.ApplicationUser).HasForeignKey(x => x.ApplicationUserId);
        }
    }
}
