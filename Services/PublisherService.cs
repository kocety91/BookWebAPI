using BookWebAPI.Models;
using BookWebAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookWebAPI.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IRepository<Publisher> publisherRepository;

        public PublisherService(IRepository<Publisher> publisherRepository)
        {
            this.publisherRepository = publisherRepository;
        }
        public async Task<Publisher> CreateAsync(string publisher)
        {
            var addedPublisher = new Publisher()
            {
                Name = publisher,
                CreatedOn = DateTime.Now
            };

            await publisherRepository.AddAsync(addedPublisher);
            await publisherRepository.SaveChangesAsync();

            return addedPublisher;
        }

        public async Task<Publisher> GetByNameAsync(string publisher)
        {
            var searchedPublisher = await publisherRepository.AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == publisher);

            if (searchedPublisher == null)
            {
                var currentPublisher = await this.CreateAsync(publisher);
                return currentPublisher;
            }

            return searchedPublisher;
        }
    }
}
