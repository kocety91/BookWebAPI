using BookWebAPI.Data;
using BookWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookWebAPI.Seeding
{
    public class GenresSeeder : ISeeder
    {
        public async Task SeedAsync(BookDbContext dbContext, IServiceProvider serviceProvider)
        {
            if(await dbContext.Genres.AnyAsync())
            {
                return;
            }

            var genres = new List<Genre>()
            {
                new Genre(){Name = "Adventure",CreatedOn = DateTime.Now},
                new Genre(){Name = "Comedy",CreatedOn = DateTime.Now},
                new Genre(){Name = "Documentary",CreatedOn = DateTime.Now},
                new Genre(){Name = "Historical",CreatedOn = DateTime.Now},
                new Genre(){Name = "Science",CreatedOn = DateTime.Now},
                new Genre(){Name = "Drama",CreatedOn = DateTime.Now},
                new Genre(){Name = "Crime",CreatedOn = DateTime.Now},
                new Genre(){Name = "Mystery",CreatedOn = DateTime.Now},
                new Genre(){Name = "Romance",CreatedOn = DateTime.Now},
                new Genre(){Name = "Technology",CreatedOn = DateTime.Now},
                new Genre(){Name = "Classics",CreatedOn = DateTime.Now},
                new Genre(){Name = "Fantasy",CreatedOn = DateTime.Now},
                new Genre(){Name = "Sports",CreatedOn = DateTime.Now}
            };

            await dbContext.Genres.AddRangeAsync(genres);
        }
    }
}
