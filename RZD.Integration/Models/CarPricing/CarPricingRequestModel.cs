namespace RZD.Integration.Models.CarPricing
{
    public class CarPricingRequestModel
    {
        public string OriginCode { get; set; }
        public string DestinationCode { get; set; }
        public DateTime DepartureDate { get; set; }
        public string TrainNumber { get; set; }
        public string SpecialPlacesDemand { get; set; } = "StandardPlacesAndForDisabledPersons";
        public bool OnlyFpkBranded { get; set; } = false;
        public string CarIssuingType { get; set; } = "All";
    }
}
