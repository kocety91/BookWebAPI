using BookWebAPI.Dtos.Books;
using System.Collections;
using System.Collections.Generic;

namespace BookWebAPI.Tests.InputModels
{
    public class InputBookData : IEnumerable<Object[]>
    {
        private readonly List<Object[]> data = new List<Object[]>()
        {
            new object[]
            {
               "BookOne",
               151.50M,
               "AuthorOneFirstName",
               "AuthorOneLastName",
               "PublisherOneName",
               "Documentary",
               "someUserId-321391231"
            },
            new object[]
            {
              "BookTwo",
              120.50M,
              "AuthorTwoFirstName",
              "AuthorTwoLastName",
              "PublisherTwoName",
              "Documentary",
              "someUserId-321391231"
            },
            new object[]
            {
               "BookThree",
               130.50M,
               "AuthorThreeFirstName",
               "AuthorThreeLastName",
               "PublisherThreeName",
               "Documentary",
               "someUserId-321391231"
            },
        };

        public IEnumerator<Object[]> GetEnumerator() => this.data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
