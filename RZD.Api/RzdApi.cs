using RZD.API.Models.CarPricing;
using RZD.API.Models.Suggests;
using RZD.API.Models.TrainPricing;
using RZD.API.Models.TrainRoute;
using RZD.Common.Configs;

namespace RZD.API
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
            return await _client.SendPostRequestAsync<CarPricingResponseModel>(RzdApiEndpoints.CarPricing,new CarPricingRequestModel()
            {
                OriginCode = origin,
                DestinationCode = destination,
                DepartureDate = departureDate,
                TrainNumber = trainNumber
            });
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
