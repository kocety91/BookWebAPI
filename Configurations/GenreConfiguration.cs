using BookWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookWebAPI.Configurations
{
    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(20).IsRequired();
            //builder.HasMany(x => x.Books).WithOne(b => b.Genre)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
