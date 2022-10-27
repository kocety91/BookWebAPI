using BookWebAPI.Data;
using BookWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookWebAPI.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly BookDbContext db;

        public PublisherService(BookDbContext db)
        {
            this.db = db;
        }
        public async Task<Publisher> CreateAsync(string publisher)
        {
            var addedPublisher = new Publisher()
            {
                Name = publisher,
                CreatedOn = DateTime.Now
            };

            await db.Publishers.AddAsync(addedPublisher);
            await db.SaveChangesAsync();

            return addedPublisher;
        }

        public async Task<Publisher> GetByNameAsync(string publisher)
        {
            var searchedPublisher = await db.Publishers.FirstOrDefaultAsync(x => x.Name == publisher);

            if (searchedPublisher == null)
            {
                var currentPublisher = await this.CreateAsync(publisher);
                return currentPublisher;
            }

            return searchedPublisher;
        }
    }
}
