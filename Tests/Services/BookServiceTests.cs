using AutoMapper;
using BookWebAPI.Data;
using BookWebAPI.Dtos.Books;
using BookWebAPI.Models;
using BookWebAPI.Repositories;
using BookWebAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BookWebAPI.Tests.Services
{
    public class BookServiceTests
    {
        private readonly BookDbContext _dbContext;

        public BookServiceTests()
        {
            var options = new DbContextOptionsBuilder<BookDbContext>()
           .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _dbContext = new BookDbContext(options);
        }

        [Fact]
        public async Task CreateAsyncWorksCorrectly(InputBookDto model)
        {
            var bookMockedRepo = new Mock<EfRepository<Book>>(_dbContext);
            var mapperMocked = new Mock<IMapper>();
            var userManagerMocked = new Mock<UserManager<ApplicationUser>>();
            var publisherMockedRepo = new Mock<EfRepository<Publisher>>(_dbContext);
            var genreMockedRepo = new Mock<EfRepository<Genre>>(_dbContext);
            var authorMockedRepo = new Mock<EfRepository<Author>>(_dbContext);




            bookMockedRepo.Setup(x => x.AddAsync(It.IsAny<Book>()))
                .Callback((Book book) => _dbContext.Books.AddAsync(book));

            var publisherService = new PublisherService(publisherMockedRepo.Object);
            var genreService = new GenreService(genreMockedRepo.Object);
            var authorService = new AuthorService(authorMockedRepo.Object);

            var booksService = new BookService(bookMockedRepo.Object, mapperMocked.Object,
                publisherService, genreService, authorService, userManagerMocked.Object);



        }
    }
}
