using AutoMapper;
using BookWebAPI.Data;
using BookWebAPI.Dtos.Books;
using BookWebAPI.Models;
using BookWebAPI.Profiles;
using BookWebAPI.Repositories;
using BookWebAPI.Services;
using BookWebAPI.Tests.InputModels;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using static BookWebAPI.Common.CustomExceptions;

namespace BookWebAPI.Tests.Services
{
    public class BookServiceTests
    {
        private readonly BookDbContext _dbContext;
        private readonly IMapper _mapper;
        public BookServiceTests()
        {
            var options = new DbContextOptionsBuilder<BookDbContext>()
           .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _dbContext = new BookDbContext(options);

            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new BookProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }
        }

        [Theory]
        [ClassData(typeof(InputBookData))]
        public async Task CreateAsyncShouldWorksCorrectly(string name, decimal price,
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


        [Theory]
        [InlineData("SomeBookId")]
        public async Task GetByIdAsyncShouldWorksCorrectly(string id)
        {
            this.SeedBooks(_dbContext);

            var bookMockedRepo = new Mock<EfRepository<Book>>(_dbContext);
            bookMockedRepo.Setup(x => x.All()).Returns(_dbContext.Books);

            var booksService = new BookService(bookMockedRepo.Object, _mapper, null, null, null, null);

            var expected = await _dbContext.Books.FirstOrDefaultAsync();

            var actual = await booksService.GetByIdAsync(id);

            actual.Id.Should().BeEquivalentTo(expected.Id);
            actual.Id.Should().NotBeNull().Should().NotBeOfType<NotFoundException>();
            actual.Name.Should().BeEquivalentTo(expected.Name);
            actual.AuthorFullName.Should().BeEquivalentTo(expected.Author.FirstName + " " + expected.Author.LastName);
            actual.Price.Should().Be(expected.Price);
        }


        [Theory]
        [InlineData("RandomBookId")]
        public async Task GetByIdAsyncShouldThrowNotFoundException(string id)
        {
            this.SeedBooks(_dbContext);

            var bookMockedRepo = new Mock<EfRepository<Book>>(_dbContext);
            bookMockedRepo.Setup(x => x.All()).Returns(_dbContext.Books);

            var booksService = new BookService(bookMockedRepo.Object, _mapper, null, null, null, null);

            await booksService.Invoking(x => x.GetByIdAsync(id))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage($"No book with this id: {id}");
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

        private void SeedBooks(BookDbContext db)
        {
            var book = new Book()
            {
                Id = "SomeBookId",
                Name = "BookName",
                Author = new Author() { Id = "SomeAuthorId", FirstName = "Koce", LastName = "Kocev" },
                AuthorId = "SomeAuthorId",
                ApplicationUser = new ApplicationUser() { Id = "SomeUserId", },
                ApplicationUserId = "SomeUserId",
                Genre = new Genre() { Id = "SomeGenreId", Name = "GenreName" },
                GenreId = "SomeGenreId",
                Publisher = new Publisher() { Id = "SOmePublisherId", Name = "PublisherName" },
                PublisherId = "SOmePublisherId",
                Price = 199
            };
            db.Books.Add(book);
            db.SaveChanges();
        }
    }
}
