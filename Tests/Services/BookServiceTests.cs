using AutoMapper;
using BookWebAPI.Data;
using BookWebAPI.Dtos.Books;
using BookWebAPI.Models;
using BookWebAPI.Repositories;
using BookWebAPI.Services;
using BookWebAPI.Tests.InputModels;
using FluentAssertions;
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

        [Theory]
        [ClassData(typeof(InputBookData))]
        public async Task CreateAsyncWorksCorrectly(string name, decimal price,
            string authorFirstName, string authorLastName, string publisherName, string genre, string userId)
        {
            var booksService = GenerateBookService(userId);

            var model = new InputBookDto()
            {
                Name = name,
                AuthorFirstName = authorFirstName,
                AuthorLastName = authorLastName,
                Price = price,
                Genre = genre,
                PublisherName = publisherName,
                UserId = userId
            };

            var bookId = await booksService.CreateAsync(model);
            var expectedBook = await _dbContext.Books.FindAsync(bookId);

            expectedBook.Should().NotBeNull();
            expectedBook.Name.Should().BeEquivalentTo(model.Name);
            expectedBook.Author.FirstName.Should().BeEquivalentTo(model.AuthorFirstName);
            expectedBook.Author.LastName.Should().BeEquivalentTo(model.AuthorLastName);
            expectedBook.Price.Should().Be(model.Price);
            expectedBook.Genre.Name.Should().BeEquivalentTo(model.Genre);
            expectedBook.Publisher.Name.Should().BeEquivalentTo(model.PublisherName);
            expectedBook.ApplicationUserId.Should().BeEquivalentTo(model.UserId);
        }

        [Theory]
        [ClassData(typeof(InputBookData))]
        public async Task CreateAsyncShouldThrowArgumentException(string name, decimal price,
            string authorFirstName, string authorLastName, string publisherName, string genre, string userId)
        {
            var bookService = this.GenerateBookService(userId);

            var book = new InputBookDto()
            {
                Name = name,
                AuthorFirstName = authorFirstName,
                AuthorLastName = authorLastName,
                Price = price,
                Genre = genre,
                PublisherName = publisherName,
                UserId = userId
            };
            await bookService.CreateAsync(book);

            await bookService.Invoking(y => y.CreateAsync(book))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Book with {book.Name} already exist!");
        }



        private BookService GenerateBookService(string userId)
        {
            //books
            var bookMockedRepo = new Mock<EfRepository<Book>>(_dbContext);
            bookMockedRepo.Setup(x => x.AddAsync(It.IsAny<Book>()))
                .Callback((Book book) => _dbContext.Books.AddAsync(book));
            bookMockedRepo.Setup(x => x.All()).Returns(_dbContext.Books);

            //usermanager
            var userManagerMocked = new Mock<UserManager<ApplicationUser>>
                (Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            userManagerMocked.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser() { Id = userId });

            //author
            var authorMockedRepo = new Mock<EfRepository<Author>>(_dbContext);
            authorMockedRepo.Setup(x => x.AllAsNoTracking())
                .Returns(_dbContext.Authors.AsNoTracking());
            authorMockedRepo.Setup(x => x.AddAsync(It.IsAny<Author>()))
                .Callback((Author author) => _dbContext.Authors.AddAsync(author));

            //publisher
            var publisherMockedRepo = new Mock<EfRepository<Publisher>>(_dbContext);
            publisherMockedRepo.Setup(x => x.AllAsNoTracking())
                .Returns(_dbContext.Publishers.AsNoTracking());
            publisherMockedRepo.Setup(x => x.AddAsync(It.IsAny<Publisher>()))
               .Callback((Publisher publisher) => _dbContext.Publishers.AddAsync(publisher));

            //genre
            var genreMockedRepo = new Mock<EfRepository<Genre>>(_dbContext);
            genreMockedRepo.Setup(x => x.AllAsNoTracking())
                .Returns(_dbContext.Genres.AsNoTracking());
            genreMockedRepo.Setup(x => x.AddAsync(It.IsAny<Genre>()))
               .Callback((Genre genre) => _dbContext.Genres.AddAsync(genre));


            var mapperMocked = new Mock<IMapper>();


            var publisherService = new PublisherService(publisherMockedRepo.Object);
            var genreService = new GenreService(genreMockedRepo.Object);
            var authorService = new AuthorService(authorMockedRepo.Object);

            var booksService = new BookService(bookMockedRepo.Object, mapperMocked.Object,
                publisherService, genreService, authorService, userManagerMocked.Object);
            return booksService;
        }
    }
}
