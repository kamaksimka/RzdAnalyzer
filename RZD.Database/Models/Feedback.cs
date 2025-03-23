namespace RZD.Database.Models
{
    public class Feedback
    {
        public long Id { get; set; }
        public string Body { get; set; }
        
        public DateTimeOffset CreatedDate { get; set; }

        public long UserId { get; set; }
        public virtual User User { get; set; }
    }
}
