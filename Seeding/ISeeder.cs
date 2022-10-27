using BookWebAPI.Data;

namespace BookWebAPI.Seeding
{
    public interface ISeeder
    {
        Task SeedAsync(BookDbContext dbContext, IServiceProvider serviceProvider);
    }
}
