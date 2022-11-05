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
    public class GenreServiceTests
    {
        private readonly BookDbContext _dbContext;

        public GenreServiceTests()
        {
            var options = new DbContextOptionsBuilder<BookDbContext>()
          .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _dbContext = new BookDbContext(options);
        }

        [Theory]
        [InlineData("Comedy")]
        [InlineData("Drama")]
        [InlineData("Documentary")]
        public async Task CreateAsyncShouldWorksCorrectly(string name)
        {
            var mockedRepo = new Mock<EfRepository<Genre>>(_dbContext);

            mockedRepo.Setup(x => x.AddAsync(It.IsAny<Genre>()))
                .Callback((Genre genre) => _dbContext.Genres.AddAsync(genre));

            var genreService = new GenreService(mockedRepo.Object);

            var expected =  await genreService.CreateAsync(name);

            var actual = await _dbContext.Genres.FirstOrDefaultAsync();

            _dbContext.Genres.Should().HaveCount(1);
            actual.Name.Should().BeEquivalentTo(expected.Name);
        }

        [Theory]
        [InlineData("Comedy")]
        [InlineData("Drama")]
        [InlineData("Documentary")]
        public async Task GetByNameAsyncShouldReturnCorrectValue(string genre)
        {
            var mockedRepo = new Mock<EfRepository<Genre>>(_dbContext);
            mockedRepo.Setup(x => x.AddAsync(It.IsAny<Genre>()))
                .Callback((Genre genre) => _dbContext.Genres.Add(genre));
            mockedRepo.Setup(x => x.AllAsNoTracking()).Returns(
                _dbContext.Genres.AsNoTracking());

            var service = new GenreService(mockedRepo.Object);

            var expected = await service.GetByNameAsync(genre);

            expected.Name.Should().BeEquivalentTo(genre);
        }
    }
}
