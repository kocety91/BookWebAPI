namespace BookWebAPI.Models
{
    public class BooksGenres
    {
        public string BookId { get; set; }

        public virtual Book Book { get; set; }

        public string GenreId { get; set; }

        public virtual Genre Genre { get; set; }
    }
}
