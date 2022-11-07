using BookWebAPI.Dtos.Books;
using BookWebAPI.Dtos.Identity;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
        public async Task GetByIdEndpointsReturnsSuccesAndCorrectContentType()
        {
            var id = "bookOneId";
            var response = await _httpClient.GetAsync($"/books/getbyid/{id}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
            response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
            responseString.Should().Contain("bookOne");
            responseString.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetByIdEndpointsRetrunsNotFound()
        {
            var id = "xxxxxx";
            var response = await _httpClient.GetAsync($"/books/getbyid/{id}");

            response.ToString().Equals(HttpStatusCode.NotFound);
            Assert.False(response.IsSuccessStatusCode);
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


        [Fact]
        public async Task DeleteEndpointsReturnsNotFound()
        {
            await AuthenticateAsync();

            var id = "xoxoxo";
            var response = await _httpClient.DeleteAsync($"/books/delete/{id}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateEndpointsReturnsSuccess()
        {
            var updateModel = new InputBookDto()
            {
             Name = "updatedBook",
             Price = 22.50M,
             AuthorFirstName ="Bate Koce",
             AuthorLastName = "Pak toi..",
             PublisherName = "Koce & KOce",
             Genre = "Documentary",
             UserId = "SomeUserId"
            };

            var book = JsonSerializer.Serialize(updateModel);
            var requstContent = new StringContent(book, Encoding.UTF8, "application/json");
            var id = "bookOneId";
            var response = await _httpClient.PutAsync($"/books/update/{id}", requstContent);
           
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }


        [Fact]
        public async Task UpdateEndpointsReturnsNotFound()
        {
            var updateModel = new InputBookDto()
            {
                Name = "updatedBooknotFound",
                Price = 22.50M,
                AuthorFirstName = "Bate Koce",
                AuthorLastName = "Pak toi..",
                PublisherName = "Koce & KOce",
                Genre = "Documentary",
                UserId = "SomeUserId"
            };

            var book = JsonSerializer.Serialize(updateModel);
            var requstContent = new StringContent(book, Encoding.UTF8, "application/json");
            var id = "x1uu77";
            var response = await _httpClient.PutAsync($"/books/update/{id}", requstContent);

            response.ToString().Equals(HttpStatusCode.NotFound);
            Assert.False(response.IsSuccessStatusCode);
        }


        [Fact]
        public async Task CreateEndpointsReturnsSuccesAndCorrectContentType()
        {
            var createModel = new InputBookDto()
            {
                Name = "createBook",
                Price = 122.50M,
                AuthorFirstName = "Bate C Koce",
                AuthorLastName = "Pak c toi..",
                PublisherName = "Koce -- KOce",
                Genre = "Comedy",
                UserId = "SomeUserId"
            };

            var book = JsonSerializer.Serialize(createModel);
            var request = new HttpRequestMessage(HttpMethod.Post, $"/books/create");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(book, Encoding.UTF8);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task CreateEndpointsReturnsBadRequest()
        {
            var createModel = new InputBookDto()
            {
                Name = "bookTwo",
                Price = 122.50M,
                AuthorFirstName = "Bate C Koce",
                AuthorLastName = "Pak c toi..",
                PublisherName = "Koce -- KOce",
                Genre = "Comedy",
                UserId = "SomeUserId"
            };

            var book = JsonSerializer.Serialize(createModel);
            var request = new HttpRequestMessage(HttpMethod.Post, $"/books/create");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(book, Encoding.UTF8);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _httpClient.SendAsync(request);

            response.ToString().Equals(HttpStatusCode.Conflict);
            Assert.False(response.IsSuccessStatusCode);
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
