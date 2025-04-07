namespace RZD.Database.Models
{
    public class RefreshToken
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public DateTimeOffset ExpiryDate { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
