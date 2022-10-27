using BookWebAPI.Data;
using BookWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookWebAPI.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly BookDbContext db;

        public AuthorService(BookDbContext db)
        {
            this.db = db;
        }

        public async Task<Author> CreateAsync(string authorFirstName, string authorLastName)
        {
            var addedAuthor = new Author()
            {
                FirstName = authorFirstName,
                LastName = authorLastName,
                CreatedOn = DateTime.Now
            };

            await db.Authors.AddAsync(addedAuthor);
            await db.SaveChangesAsync();

            return addedAuthor;
        }

        public async Task<Author> GetByNameAsync(string authorFirstName, string authorLastName)
        {
            var searchedAuthor = await db.Authors
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
