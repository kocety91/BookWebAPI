using BookWebAPI.Models;
using BookWebAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookWebAPI.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IRepository<Author> authorRepository;

        public AuthorService(IRepository<Author> authorRepository)
        {
            this.authorRepository = authorRepository;
        }

        public async Task<Author> CreateAsync(string authorFirstName, string authorLastName)
        {
            var addedAuthor = new Author()
            {
                FirstName = authorFirstName,
                LastName = authorLastName,
                CreatedOn = DateTime.Now
            };

            await authorRepository.AddAsync(addedAuthor);
            await authorRepository.SaveChangesAsync();

            return addedAuthor;
        }

        public async Task<Author> GetByNameAsync(string authorFirstName, string authorLastName)
        {
            var searchedAuthor = await authorRepository.AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.FirstName == authorFirstName && x.LastName == authorLastName);

            if (searchedAuthor == null)
            {
                var author = await this.CreateAsync(authorFirstName, authorLastName);
                return author;
            }

            return searchedAuthor;
        }
    }
}
