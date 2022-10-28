using BookWebAPI.Dtos.Books;

namespace BookWebAPI.Services
{
    public interface IBookService
    {
        Task<OutputBookDto> CreateAsync(InputBookDto model);

        Task<OutputBookDto> GetByIdAsync(string id);

        Task<IEnumerable<OutputBookDto>> GetAllAsync();

        Task<OutputBookDto> UpdateAsync(string id, InputBookDto model);

        Task DeleteAsync(string id);
    }
}
