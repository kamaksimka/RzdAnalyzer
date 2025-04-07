namespace RZD.Database.Models
{
    public class TrackedRoute
    {
        public long Id { get; set; }
        public string OriginExpressCode { get; set; }
        public string DestinationExpressCode { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual List<Train> Trains { get; set; }
    }
}
