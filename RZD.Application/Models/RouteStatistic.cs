namespace RZD.Application.Models
{
    public class RouteStatistic
    {
        public DateTimeOffset StartTrackedDate { get; set; }
        public string OriginStationName { get; set; }
        public string OriginRegion { get; set; }
        public string DestinationStationName { get; set; }
        public string DestinationRegion { get; set; }

        public int NumberTrains { get; set; }
        public int NumberCarPlaces { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
        public TimeSpan? FastestTrain {  get; set; }
        public TimeSpan? SlowestTrain { get; set; }
        public double AvarageTripDistance { get; set; }
    }
}
