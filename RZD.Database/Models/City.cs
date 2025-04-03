namespace RZD.Database.Models
{
    public class City
    {
        public long Id { get; set; }
        public string NodeId { get; set; }
        public string? ExpressCode { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string? ForeignCode { get; set; }
        public string? ExpressCodes { get; set; }
        public virtual List<TrainStation> Stations { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
    }

}
