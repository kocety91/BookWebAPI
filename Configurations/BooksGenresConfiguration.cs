using BookWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookWebAPI.Configurations
{
    public class BooksGenresConfiguration : IEntityTypeConfiguration<BooksGenres>
    {
        public void Configure(EntityTypeBuilder<BooksGenres> builder)
        {
            builder.HasKey(x => new { x.GenreId, x.BookId });
        }
    }
}
