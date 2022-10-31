using BookWebAPI.Data;
using BookWebAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace BookWebAPI.Seeding
{
    public class BookDbContextSeeder : ISeeder
    {
        public async Task SeedAsync(BookDbContext dbContext, IServiceProvider serviceProvider)
        {
            if(dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }   

            if(serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var seeders = new List<ISeeder>()
            {
                new PublishersSeeder(),
                new GenresSeeder()
            };

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync(dbContext, serviceProvider);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
