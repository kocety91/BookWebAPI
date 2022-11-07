using AutoMapper;
using BookWebAPI.Dtos.Books;
using BookWebAPI.Models;
using BookWebAPI.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static BookWebAPI.Common.CustomExceptions;

namespace BookWebAPI.Services
{
    public class BookService : IBookService
    {
        private readonly IRepository<Book> bookRepository;
        private readonly IMapper mapper;
        private readonly IPublisherService publisherService;
        private readonly IGenreService genreService;
        private readonly IAuthorService authorService;
        private readonly UserManager<ApplicationUser> userManager;

        public BookService(IRepository<Book> bookRepository,
            IMapper mapper,
            IPublisherService publisherService,
            IGenreService genreService,
            IAuthorService authorService,
            UserManager<ApplicationUser> userManager)
        {
            this.bookRepository = bookRepository;
            this.mapper = mapper;
            this.publisherService = publisherService;
            this.genreService = genreService;
            this.authorService = authorService;
            this.userManager = userManager;
        }

        public async Task<string> CreateAsync(InputBookDto model)
        {
            var searchedBook = await bookRepository.All().FirstOrDefaultAsync(x => x.Name == model.Name);
            if (searchedBook != null) throw new ExistsException($"Book with name: {model.Name} already exist!");

            var user = await userManager.FindByIdAsync(model.UserId);
            var author = await authorService.GetByNameAsync(model.AuthorFirstName, model.AuthorLastName);
            var publisher = await publisherService.GetByNameAsync(model.PublisherName);
            var genre = await genreService.GetByNameAsync(model.Genre);

            var book = new Book()
            {
                Name = model.Name,
                Price = model.Price,
                CreatedOn = DateTime.Now,
                ApplicationUser = user,
                ApplicationUserId = model.UserId,
                AuthorId = author.Id,
                PublisherId = publisher.Id,
                GenreId = genre.Id
            };

            await bookRepository.AddAsync(book);
            await bookRepository.SaveChangesAsync();

            return book.Id;
        }

        public async Task<OutputBookDto> GetByIdAsync(string id)
        {
            var book = await bookRepository
                         .All()
                         .Where(x => x.Id == id)
                         .Include(x => x.Author)
                         .Include(x => x.Genre)
                         .Include(x => x.Publisher)
                         .Include(x => x.ApplicationUser)
                         .FirstOrDefaultAsync();

            if (book == null) throw new NotFoundException($"No book with this id: {id}");

            var mappedBook = mapper.Map<OutputBookDto>(book);
            return mappedBook;
        }

        public async Task<IEnumerable<OutputBookDto>> GetAllAsync()
        {
            var books = bookRepository
                .All()
                .Include(x => x.Author)
                .Include(x => x.Genre)
                .Include(x => x.Publisher)
                .Include(x => x.ApplicationUser);

            var booksAfter = await books.ToListAsync();

            var mappedBooks = mapper.Map<IEnumerable<OutputBookDto>>(booksAfter);
            return mappedBooks;
        }

        public async Task<OutputBookDto> UpdateAsync(string id, InputBookDto model)
        {
            if (model == null) throw new NullReferenceException(nameof(model));

            var bookForUpdate = await bookRepository
                .All()
                .Include(x => x.Author)
                .Include(x => x.Genre)
                .Include(x => x.Publisher)
                .Include(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (bookForUpdate == null) throw new NotFoundException($"No book with this id : {id}");

            var user = await userManager.FindByIdAsync(model.UserId);

            bookForUpdate.Name = model.Name;
            bookForUpdate.Price = model.Price;
            bookForUpdate.Author.FirstName = model.AuthorFirstName;
            bookForUpdate.Author.LastName = model.AuthorLastName;
            bookForUpdate.Publisher.Name = model.PublisherName;
            bookForUpdate.Genre.Name = model.Genre;
            bookForUpdate.ModifiedOn = DateTime.Now;
            bookForUpdate.ApplicationUserId = user.Id;

            bookRepository.Update(bookForUpdate);
            await bookRepository.SaveChangesAsync();

            var mappedBook = mapper.Map<OutputBookDto>(bookForUpdate);
            return mappedBook;
        }

        public async Task DeleteAsync(string id)
        {
            var bookForDelete = await bookRepository.All().FirstOrDefaultAsync(x => x.Id == id);

            if (bookForDelete == null) throw new NotFoundException($"Book with id : {id} not found !");

            bookRepository.Delete(bookForDelete);
            await bookRepository.SaveChangesAsync();
        }
    }
}
