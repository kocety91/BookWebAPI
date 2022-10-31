namespace BookWebAPI.Models
{
    public class RefreshToken
    {
        public RefreshToken()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public string Token { get; set; }

        public string JwtId { get; set; }

        public bool IsUsed { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime ExpiryDate { get; set; }

    }
}
