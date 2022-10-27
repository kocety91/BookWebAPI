using BookWebAPI.Data;
using BookWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookWebAPI.Seeding
{
    public class PublishersSeeder : ISeeder
    {
        public async Task SeedAsync(BookDbContext dbContext, IServiceProvider serviceProvider)
        {
            if(await dbContext.Publishers.AnyAsync())
            {
                return;
            }

            var publishers = new List<Publisher>()
            {
                new Publisher(){Name = "Hachette Book Group",CreatedOn =DateTime.Now},
                new Publisher(){Name = "HarperCollins",CreatedOn =DateTime.Now},
                new Publisher(){Name = "Macmillan Publishers",CreatedOn =DateTime.Now},
                new Publisher(){Name = "Penguin Random House",CreatedOn =DateTime.Now},
                new Publisher(){Name = "Simon and Schuster",CreatedOn =DateTime.Now},
            };

            await dbContext.Publishers.AddRangeAsync(publishers);
        }
    }
}
