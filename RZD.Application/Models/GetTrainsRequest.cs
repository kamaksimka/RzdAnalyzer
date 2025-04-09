namespace RZD.Application.Models
{
    public class GetTrainsRequest
    {
        public long TrackedRouteId { get; set; }
        public DateTime DateFrom {  get; set; }
        public DateTime DateTo { get; set; }
    }
}
