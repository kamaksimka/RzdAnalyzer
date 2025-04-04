namespace RZD.Integration.Models.TrainPricing
{
    public class TrainPricingResponseModel
    {
        public string DestinationCode { get; set; }
        public string DestinationStationCode { get; set; }
        public string OriginCode { get; set; }
        public string OriginStationCode { get; set; }
        public List<RzdTrain> Trains { get; set; }
    }
}
