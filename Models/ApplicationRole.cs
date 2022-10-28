using Microsoft.AspNetCore.Identity;

namespace BookWebAPI.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
