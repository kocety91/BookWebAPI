namespace BookWebAPI.Models
{
    public class Publisher : BaseModel
    {
        public Publisher()
        {
            this.Books = new HashSet<Book>();
        }

        public string Name { get; set; }

        public virtual ICollection<Book>  Books{ get; set; }
    }
}
