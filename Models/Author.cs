namespace BookWebAPI.Models
{
    public class Author : BaseModel
    {
        public Author()
        {
            this.Books = new HashSet<Book>();
        }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
