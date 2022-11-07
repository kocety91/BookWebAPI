using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using BookWebAPI.Data;
using BookWebAPI.Models;

namespace BookWebAPI.Tests
{
    public class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> 
        where TEntryPoint : Program
    {
        private readonly ILogger<TestingWebAppFactory<Program>> logger;

        public TestingWebAppFactory(ILogger<TestingWebAppFactory<Program>> logger)
        {
            this.logger = logger;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                    typeof(DbContextOptions<BookDbContext>));

                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<BookDbContext>(opt =>
                {
                    opt.UseInMemoryDatabase("InMemoryBookTest");
                });

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<BookDbContext>())
                {
                    try
                    {
                        this.SeedBooks(appContext);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.StackTrace);
                    }
                }
            });
        }

        private void SeedBooks(BookDbContext db)
        {
            var books = new List<Book>();
            var bookOne = new Book()
            {
                Id = "bookOneId",
                Name = "bookOne",
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
            books.Add(bookOne);
            var bookTwo = new Book()
            {
                Id = "bookTwoId",
                Name = "bookTwo",
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
            books.Add(bookTwo);
            db.Books.AddRange(books);
            db.SaveChanges();
        }
    }
}
