using BookWebAPI.Models;

namespace BookWebAPI.Services
{
    public interface IAuthorService
    {
        Task<Author> CreateAsync(string authorFirstName, string authorLastName);

        Task<Author> GetByNameAsync(string authorFirstName,string authorLastName);
    }
}
