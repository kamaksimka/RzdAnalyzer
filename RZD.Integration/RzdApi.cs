using RZD.Integration.Models.CarPricing;
using RZD.Integration.Models.Suggests;
using RZD.Integration.Models.TrainPricing;
using RZD.Integration.Models.TrainRoute;
using RZD.Common.Configs;

namespace RZD.Integration
{
    public class RzdApi:IDisposable
    {
        private readonly RzdClient _client;

        public RzdApi(RzdConfig config)
        {
            _client = new RzdClient(config);
        }

        public async Task<SuggestsResponseModel> SuggestsAsync(string query)
        {
            return await _client.SendGetRequestAsync<SuggestsResponseModel>(RzdApiEndpoints.Suggests(query));
        }

        public async Task<TrainRouteResponseModel> TrainRouteAsync(string trainNumber, string origin, string destination, DateTime departureDate)
        {
            return await _client.SendGetRequestAsync<TrainRouteResponseModel>(RzdApiEndpoints.TrainRoute(trainNumber,origin,destination,departureDate));
        }

        public async Task<TrainPricingResponseModel> TrainPricingAsync(string origin, string destination, DateTime departureDate)
        {
            return await _client.SendGetRequestAsync<TrainPricingResponseModel>(RzdApiEndpoints.TrainPricing(origin, destination, departureDate));
        }

        public async Task<CarPricingResponseModel> CarPricingAsync(string origin, string destination, DateTime departureDate, string trainNumber)
        {
            var query = new Dictionary<string, string>()
            {
                ["service_provider"] = "B2B_RZD",
                ["isBonusPurchase"] = "false"
            };
            return await _client.SendPostRequestAsync<CarPricingResponseModel>(RzdApiEndpoints.CarPricing,new CarPricingRequestModel()
            {
                OriginCode = origin,
                DestinationCode = destination,
                DepartureDate = departureDate,
                TrainNumber = trainNumber
            }, query);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
