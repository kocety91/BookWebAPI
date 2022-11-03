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


            builder.HasOne(b => b.Genre).WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId);


            builder.HasOne(b => b.Author).WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);


            builder.HasOne(b => b.ApplicationUser).WithMany(ap => ap.Books)
                .HasForeignKey(b => b.ApplicationUserId);

            builder.HasOne(b => b.Publisher).WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId);
        }
    }
}
