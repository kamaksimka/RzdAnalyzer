namespace RZD.Application.Models
{
    public class RefreshRequest
    {
        public long UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
