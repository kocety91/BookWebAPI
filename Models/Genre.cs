namespace BookWebAPI.Models
{
    public class Genre : BaseModel
    {
        public Genre()
        {
            this.Books = new HashSet<Book>();
        }
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
