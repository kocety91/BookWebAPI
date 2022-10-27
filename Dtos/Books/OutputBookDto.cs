namespace BookWebAPI.Dtos.Books
{
    public class OutputBookDto
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string AuthorFullName { get; set; }

        public string PublisherName { get; set; }

        public string Genre { get; set; }
    }
}
