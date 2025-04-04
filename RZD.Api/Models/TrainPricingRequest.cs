namespace RZD.Api.Models
{
    public class TrainPricingRequest
    {
        public string OriginCode { get; set; }
        public string DestinationCode { get; set; }
        public DateTime DepartureDate { get; set; }
    }
}
