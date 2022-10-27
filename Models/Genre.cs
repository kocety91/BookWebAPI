namespace BookWebAPI.Models
{
    public class Genre : BaseModel
    {
        public Genre()
        {
            this.Books = new HashSet<BooksGenres>();
        }
        public string Name { get; set; }

        public virtual ICollection<BooksGenres> Books { get; set; }
    }
}
