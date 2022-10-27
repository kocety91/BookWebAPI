using BookWebAPI.Data;
using BookWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookWebAPI.Services
{
    public class GenreService : IGenreService
    {
        private readonly BookDbContext db;

        public GenreService(BookDbContext db)
        {
            this.db = db;
        }

        public async Task<Genre> CreateAsync(string genre)
        {
            var addedGenre = new Genre()
            {
                Name = genre
            };

            await db.Genres.AddAsync(addedGenre);
            await db.SaveChangesAsync();

            return addedGenre;
        }

        public async Task<Genre> GetByNameAsync(string genre)
        {
            var searchedGenre = await db.Genres.FirstOrDefaultAsync(x => x.Name == genre);

            if (searchedGenre == null)
            {
                var currentGenre = await this.CreateAsync(genre);
                return currentGenre;
            }

            return searchedGenre;
        }
    }
}
