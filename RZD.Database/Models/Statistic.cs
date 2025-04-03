namespace RZD.Database.Models
{
    public class Statistic
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DateStart { get; set; }
        public DateTimeOffset? DateFinish { get; set; }
        public string Comment { get; set; }
        public bool IsSuccess { get; set; }
    }
}
