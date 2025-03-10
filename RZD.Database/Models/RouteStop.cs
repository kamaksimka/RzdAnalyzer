namespace RZD.Database.Models
{
    public class RouteStop
    {
        public long Id { get; set; }
        public string ActualMovement { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string ArrivalTime { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string DepartureTime { get; set; }
        public DateTime LocalArrivalDateTime { get; set; }
        public string LocalArrivalTime { get; set; }
        public DateTime LocalDepartureDateTime { get; set; }
        public string LocalDepartureTime { get; set; }
        public string StationCode { get; set; }
        public int StopDuration { get; set; }
        public int TimeZoneDifference { get; set; }

        public long RouteId { get; set; }
        public virtual Route Route { get; set; }
    }
}
