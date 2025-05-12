namespace RZD.Application.Models
{
    public class PickUpTrainRequest
    {
        public long TrackedRouteId { get; set; }
        public DateTime StartArrivalTime { get; set; }

        public DateTime EndArrivalTime { get; set; }

        public DateTime? StartDepartureTime { get; set; }

        public DateTime? EndDepartureTime { get; set; }

        public List<string> CarTypes { get; set; }

        public List<string> CarServices { get; set; }

        public bool IsUpperSeat {get; set;}

        public bool IsLowerSeat { get; set; }
        public bool IsAnySeat { get; set; }

        public int? TravelTimeInMinutes { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

    }
}
