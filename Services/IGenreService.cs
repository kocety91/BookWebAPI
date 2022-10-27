using BookWebAPI.Models;

namespace BookWebAPI.Services
{
    public interface IGenreService
    {
        Task<Genre> CreateAsync(string genre);

        Task<Genre> GetByNameAsync(string genre);
    }
}
