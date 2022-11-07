using BookWebAPI.Dtos.Books;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace BookWebAPI.Tests.Controllers
{
    public class BooksControllerTests : IntegrationTest
    {

        [Fact]
        public async Task GetAllEndpointsReturnSuccessAndCorrectContentType()
        {
            //Arrenge
            
            //Act
            var response = await _httpClient.GetAsync("/koce");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            //(await response.Content.ReadAsAsync<List<OutputBookDto>>()).Should/*().BeEmpty();*/
        }
    }
}
