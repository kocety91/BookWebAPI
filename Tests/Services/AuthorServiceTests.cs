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
    public class AuthorServiceTests
    {
        private readonly BookDbContext _dbContext;

        public AuthorServiceTests()
        {
            var options = new DbContextOptionsBuilder<BookDbContext>()
           .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _dbContext = new BookDbContext(options);
        }

        [Theory]
        [InlineData("Aleksandur", "Simeonov")]
        [InlineData("Kiril", "Evtimov")]
        [InlineData("Petur", "Petrov")]
        public async Task CreateAsyncShouldWorksCorrectly(string firstName, string lastName)
        {
            var mockedRepo = new Mock<EfRepository<Author>>(_dbContext); 

            mockedRepo.Setup(x => x.AddAsync(It.IsAny<Author>()))
                .Callback((Author author) => _dbContext.Authors.AddAsync(author));

            var authorService = new AuthorService(mockedRepo.Object);

            await authorService.CreateAsync(firstName, lastName);

            var actualAuthor = await _dbContext.Authors.FirstOrDefaultAsync();

            _dbContext.Authors.Should().HaveCount(1);
            actualAuthor.FirstName.Should().BeEquivalentTo(firstName);
            actualAuthor.LastName.Should().BeEquivalentTo(lastName);
        }

        [Theory]
        [InlineData("Aleksandur", "Simeonov")]
        [InlineData("Kiril", "Evtimov")]
        [InlineData("Petur", "Petrov")]
        public async Task GetAuthorNameShouldReturnCorrectValue(string firstName, string lastName)
        {
            var mockedRepo = new Mock<EfRepository<Author>>(_dbContext);

            mockedRepo.Setup(x => x.AddAsync(It.IsAny<Author>()))
                .Callback((Author author) => _dbContext.Authors.Add(author));

            mockedRepo.Setup(x => x.AllAsNoTracking()).Returns(
                _dbContext.Authors.AsNoTracking());

            var service = new AuthorService(mockedRepo.Object);
            await service.CreateAsync(firstName, lastName);

            var expected = await service.GetByNameAsync(firstName, lastName);

            expected.FirstName.Should().BeEquivalentTo(firstName);
            expected.LastName.Should().BeEquivalentTo(lastName);
        }
    }
}
