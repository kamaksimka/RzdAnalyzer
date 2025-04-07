namespace RZD.Application.Models
{
    public class FeedbackModel
    {
        public string UserEmail { get; set; } = null!;
        public string Body { get; set; } = null!;
        public DateTimeOffset CreatedDate { get; set; }
    }
}
