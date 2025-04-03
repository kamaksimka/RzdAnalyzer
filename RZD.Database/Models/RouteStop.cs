namespace RZD.Database.Models
{
    public class RouteStop
    {
        public long Id { get; set; }
        public string ActualMovement { get; set; }
        public DateTimeOffset ArrivalDateTime { get; set; }
        public string ArrivalTime { get; set; }
        public DateTimeOffset DepartureDateTime { get; set; }
        public string DepartureTime { get; set; }
        public DateTimeOffset LocalArrivalDateTime { get; set; }
        public string LocalArrivalTime { get; set; }
        public DateTimeOffset LocalDepartureDateTime { get; set; }
        public string LocalDepartureTime { get; set; }
        public string StationCode { get; set; }
        public int StopDuration { get; set; }
        public int TimeZoneDifference { get; set; }

        public long RouteId { get; set; }
        public virtual Route Route { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
    }
}
