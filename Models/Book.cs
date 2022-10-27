namespace BookWebAPI.Models
{
    public class Book : BaseModel
    {
        public Book()
        {
            this.Genres = new HashSet<BooksGenres>();
        }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public bool IsInDiscount { get; set; }

        public string AuthorId { get; set; }

        public virtual Author Author { get; set; }

        public string PublisherId { get; set; }

        public virtual Publisher Publisher { get; set; }

        public virtual ICollection<BooksGenres> Genres { get; set; }


    }
}
