using BookWebAPI.Data;
using BookWebAPI.Models;
using BookWebAPI.Repositories;
using BookWebAPI.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BookWebAPI.Tests.Services
{
    public class PublisherServiceTests
    {

        private readonly BookDbContext _dbContext;
        public PublisherServiceTests()
        {
            var options = new DbContextOptionsBuilder<BookDbContext>()
          .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _dbContext = new BookDbContext(options);
        }

        [Theory]
        [InlineData("Something nice ....")]
        [InlineData("Something bad ...")]
        [InlineData("Something random ...")]
        public async Task CreateAsyncShouldWorksCorrectly(string name)
        {
            var mockedRepo = new Mock<EfRepository<Publisher>>(_dbContext);

            mockedRepo.Setup(x => x.AddAsync(It.IsAny<Publisher>()))
                .Callback((Publisher publisher) => _dbContext.Publishers.AddAsync(publisher));

            var publisherService = new PublisherService(mockedRepo.Object);

            await publisherService.CreateAsync(name);

            var actualPublisher= await _dbContext.Publishers.FirstOrDefaultAsync();

            _dbContext.Publishers.Should().HaveCount(1);
            actualPublisher.Name.Should().BeEquivalentTo(name);
            actualPublisher.Should().NotBeNull();
        }


        [Theory]
        [InlineData("Something nice ....")]
        [InlineData("Something bad ...")]
        [InlineData("Something random ...")]
        public async Task GetAuthorNameShouldReturnCorrectValue(string name)
        {
            var mockedRepo = new Mock<EfRepository<Publisher>>(_dbContext);
            mockedRepo.Setup(x => x.AddAsync(It.IsAny<Publisher>()))
                .Callback((Publisher publisher) => _dbContext.Publishers.Add(publisher));
            mockedRepo.Setup(x => x.AllAsNoTracking()).Returns(
                _dbContext.Publishers.AsNoTracking());

            var service = new PublisherService(mockedRepo.Object);

            var expected = await service.GetByNameAsync(name);

            expected.Name.Should().BeEquivalentTo(name);
            expected.Should().NotBeNull();
        }
    }
}
