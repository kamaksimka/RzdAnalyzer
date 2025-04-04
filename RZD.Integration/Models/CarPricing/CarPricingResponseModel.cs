namespace RZD.Integration.Models.CarPricing
{
    public class CarPricingResponseModel
    {
        public string DestinationCode { get; set; }
        public string OriginCode { get; set; }

        public List<RzdCar> Cars { get; set; }
    }
}
