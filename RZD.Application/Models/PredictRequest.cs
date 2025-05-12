namespace RZD.Application.Models
{
    public class PredictRequest
    {
        public long TrackedRouteId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CarType { get; set; }
        public int CountPlace { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public DateTime DepartureDateTime { get; set; }
    }
}
