namespace RZD.Application.Models
{
    public class TrackedRouteModel
    {
        public long Id { get; set; }
        public string OriginName { get; set; }
        public string OriginRegion { get; set; }
        public string DestinationName { get; set; }
        public string DestinationRegion { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
