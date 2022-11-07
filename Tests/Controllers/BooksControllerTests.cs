using BookWebAPI.Dtos.Identity;
using BookWebAPI.Models;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace BookWebAPI.Tests.Controllers
{
    public class BooksControllerTests : IClassFixture<TestingWebAppFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        public BooksControllerTests(TestingWebAppFactory<Program> factory)
        {
            this._httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllEndpointsReturnSuccessAndCorrectContentType()
        {
            var response = await _httpClient.GetAsync("/books/getall");
            response.EnsureSuccessStatusCode();
            var responseString =  await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
            response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
            responseString.Should().Contain("bookOne");
            responseString.Should().Contain("bookTwo");
            responseString.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task DeleteEndpointsReturnsSuccess()
        {
            await AuthenticateAsync();

            var id = "bookOneId";
            var response = await _httpClient.DeleteAsync($"/books/delete/{id}");
           
            response.EnsureSuccessStatusCode();

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }



        protected async Task AuthenticateAsync()
        {
          _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetJwtAsync());
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
