using System.ComponentModel.DataAnnotations;

namespace BookWebAPI.Dtos.Identity
{
    public class TokenRequestModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
