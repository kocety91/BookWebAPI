using Microsoft.AspNetCore.Identity;

namespace BookWebAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Books = new HashSet<Book>();
        }
        public string Country { get; set; }

        public virtual ICollection<Book> Books { get; set; }

        public string ApplicationRoleId { get; set; }

        public virtual ApplicationRole ApplicationRole { get; set; }
    }
}
