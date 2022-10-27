using BookWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookWebAPI.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(20).IsRequired();
            builder.Property(x => x.IsInDiscount).HasDefaultValue(false);
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.AuthorId).IsRequired();
            builder.Property(x => x.PublisherId).IsRequired();
        }
    }
}
