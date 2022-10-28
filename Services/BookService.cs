using AutoMapper;
using BookWebAPI.Data;
using BookWebAPI.Dtos.Books;
using BookWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using static BookWebAPI.Common.CustomExceptions;

namespace BookWebAPI.Services
{
    public class BookService : IBookService
    {
        private readonly BookDbContext db;
        private readonly IMapper mapper;
        private readonly IPublisherService publisherService;
        private readonly IGenreService genreService;
        private readonly IAuthorService authorService;

        public BookService(BookDbContext db,
            IMapper mapper,
            IPublisherService publisherService,
            IGenreService genreService,
            IAuthorService authorService)
        {
            this.db = db;
            this.mapper = mapper;
            this.publisherService = publisherService;
            this.genreService = genreService;
            this.authorService = authorService;
        }

        public async Task<OutputBookDto> CreateAsync(InputBookDto model)
        {
            if (await db.Books.AnyAsync(x => x.Name == model.Name)) throw new ArgumentException($"Book with {model.Name} already exist!");
            
            if(model == null) throw new NullReferenceException(nameof(model));

            var book = new Book()
            {
                Name = model.Name,
                Price = model.Price,
                CreatedOn = DateTime.Now
            };

            await SetBookPropertiesAsync(model, book, authorService, publisherService, genreService);

            await db.Books.AddAsync(book);
            await db.SaveChangesAsync();

            var mappedBook = mapper.Map<OutputBookDto>(book);

            return mappedBook;

        }

        public async Task<OutputBookDto> GetByIdAsync(string id)
        {
            var bookBefore = db.Books.Where(x => x.Id == id).AsQueryable();
            bookBefore = bookBefore.Include(x => x.Author);
            bookBefore = bookBefore.Include(x => x.Genre);
            bookBefore = bookBefore.Include(x => x.Publisher);

            var book = await bookBefore.FirstOrDefaultAsync();

            if (book == null) throw new NotFoundException($"No book with this id: {id}");

            var mappedBook = mapper.Map<OutputBookDto>(book);
            return mappedBook;
        }

        public async Task<IEnumerable<OutputBookDto>> GetAllAsync()
        {
            var books = db.Books.Include(x => x.Author).AsQueryable();
            books = books.Include(x => x.Genre);
            books = books.Include(x => x.Publisher);

            var booksAfter = await books.ToListAsync();

            var mappedBooks = mapper.Map<IEnumerable<OutputBookDto>>(booksAfter);
            return mappedBooks;
        }

        public async Task<OutputBookDto> UpdateAsync(string id, InputBookDto model)
        {
            if (model == null) throw new NullReferenceException(nameof(model));

            var bookForUpdate = await db.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (bookForUpdate == null) throw new NotFoundException($"No book with this id : {id}");
            
            await SetBookPropertiesAsync(model, bookForUpdate, authorService, publisherService, genreService);

            bookForUpdate.Name = model.Name;
            bookForUpdate.Price = model.Price;
            bookForUpdate.Author.FirstName = model.AuthorFirstName;
            bookForUpdate.Author.LastName = model.AuthorLastName;
            bookForUpdate.Publisher.Name = model.PublisherName;
            bookForUpdate.Genre.Name = model.Genre;
            bookForUpdate.ModifiedOn = DateTime.Now;

            db.Books.Update(bookForUpdate);
            await db.SaveChangesAsync();

            var mappedBook = mapper.Map<OutputBookDto>(bookForUpdate);
            return mappedBook;
        }

        public async Task DeleteAsync(string id)
        {
            var bookForDelete = await db.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (bookForDelete == null) throw new NotFoundException($"Book with id : {id} not found !");

            db.Books.Remove(bookForDelete);
            await db.SaveChangesAsync();
        }


        private static async Task SetBookPropertiesAsync(InputBookDto model,
            Book book, IAuthorService authorService, IPublisherService publiherService,
            IGenreService genreService)
        {
            var author = await authorService.GetByNameAsync(model.AuthorFirstName, model.AuthorLastName);
            book.Author = author;
            book.AuthorId = author.Id;

            var publisher = await publiherService.GetByNameAsync(model.PublisherName);
            book.Publisher = publisher;
            book.PublisherId = publisher.Id;

            var genre = await genreService.GetByNameAsync(model.Genre);
            book.Genre = genre;
            book.GenreId = genre.Id;
        }

    }
}
