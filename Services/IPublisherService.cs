using BookWebAPI.Models;

namespace BookWebAPI.Services
{
    public interface IPublisherService
    {
        Task<Publisher> CreateAsync(string publisher);

        Task<Publisher> GetByNameAsync(string publisher);
    }
}
