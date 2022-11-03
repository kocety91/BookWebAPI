using BookWebAPI.Data;
using BookWebAPI.Models;
using BookWebAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookWebAPI.Services
{
    public class GenreService : IGenreService
    {
        private readonly IRepository<Genre> genreReposiory;

        public GenreService(IRepository<Genre> genreReposiory)
        {
            this.genreReposiory = genreReposiory;
        }

        public async Task<Genre> CreateAsync(string genre)
        {
            var addedGenre = new Genre()
            {
                Name = genre,
                CreatedOn = DateTime.Now
            };

            await genreReposiory.AddAsync(addedGenre);
            await genreReposiory.SaveChangesAsync();

            return addedGenre;
        }

        public async Task<Genre> GetByNameAsync(string genre)
        {
            var searchedGenre = await genreReposiory.AllAsNoTracking().FirstOrDefaultAsync(x => x.Name == genre);

            if (searchedGenre == null)
            {
                var currentGenre = await this.CreateAsync(genre);
                return currentGenre;
            }

            return searchedGenre;
        }
    }
}
