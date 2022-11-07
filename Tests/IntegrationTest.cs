using BookWebAPI.Data;
using BookWebAPI.Dtos.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http.Headers;

namespace BookWebAPI.Tests
{
    public class IntegrationTest
    {
        protected readonly HttpClient _httpClient;
        private readonly BookDbContext _dbContext;


        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(BookDbContext));
                        services.AddDbContext<BookDbContext>(opt =>
                        {
                            opt.UseInMemoryDatabase("TestDb");
                        });
                    });
                });
            _httpClient = appFactory.CreateClient();
        }



        protected async Task AuthenticateAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await _httpClient.PostAsJsonAsync("identity/register", new RegisterRequestModel()
            {
                UserName = "integrationTest",
                Email = "integrationTest@gmail.com",
                Password = "12345",
                Country = "Bulgaria"
            });

            var registrationResponseObject = await response.Content.ReadAsAsync<AuthenticationResponseModel>();
            return registrationResponseObject.Token;
        }
    }
}
