using BookWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookWebAPI.Configurations
{
    public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
    {
        public void Configure(EntityTypeBuilder<Publisher> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(20).IsRequired();
            //builder.HasMany(x => x.Books).WithOne(b => b.Publisher)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
