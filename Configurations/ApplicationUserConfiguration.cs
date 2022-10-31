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

            builder.HasMany(x => x.Books)
               .WithOne(b => b.ApplicationUser).HasForeignKey(x => x.ApplicationUserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Roles)
                .WithOne().HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Claims)
                .WithOne().HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Logins)
                .WithOne().HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
