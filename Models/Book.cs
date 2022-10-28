namespace BookWebAPI.Models
{
    public class Book : BaseModel
    {
        public Book()
        {
        }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public bool IsInDiscount { get; set; }

        public string AuthorId { get; set; }

        public virtual Author Author { get; set; }

        public string PublisherId { get; set; }

        public virtual Publisher Publisher { get; set; }

        public string GenreId { get; set; }

        public virtual Genre Genre { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
