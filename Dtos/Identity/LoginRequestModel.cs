using System.ComponentModel.DataAnnotations;

namespace BookWebAPI.Dtos.Identity
{
    public class LoginRequestModel
    {
        [Required,EmailAddress]
        public string Email { get; set; }

        [Required,StringLength(15,MinimumLength =4)]
        public string Password { get; set; }
    }
}
