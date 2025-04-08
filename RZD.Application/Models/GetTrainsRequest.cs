namespace RZD.Application.Models
{
    public class GetTrainsRequest
    {
        public long TrackedRouteId { get; set; }
        public DateTime Date {  get; set; }
    }
}
