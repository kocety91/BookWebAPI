using BookWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookWebAPI.Configurations
{
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasOne(x => x.ApplicationUser)
                .WithOne(r => r.ApplicationRole)
                .HasForeignKey<ApplicationRole>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.Property(x => x.CreatedOn).IsRequired();

        }
    }
}
