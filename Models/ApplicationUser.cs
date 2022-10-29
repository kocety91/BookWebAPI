using Microsoft.AspNetCore.Identity;

namespace BookWebAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Books = new HashSet<Book>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
        }
        public string Country { get; set; }

        public string ApplicationRoleId { get; set; }

        public virtual ApplicationRole ApplicationRole { get; set; }

        public virtual ICollection<Book> Books { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
    }
}
