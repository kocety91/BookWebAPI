using System.ComponentModel.DataAnnotations;

namespace BookWebAPI.Dtos.Books
{
    public class InputBookDto
    {
        [Required, StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }

        [Required,Range(1,200)]
        public decimal Price { get; set; }


        [Required, StringLength(15, MinimumLength = 5)]
        public string AuthorFirstName { get; set; }


        [Required, StringLength(15, MinimumLength = 5)]
        public string AuthorLastName { get; set; }


        [Required, StringLength(30, MinimumLength = 10)]

        public string PublisherName { get; set; }


        [Required]
        public string Genre { get; set; }
    }
}
