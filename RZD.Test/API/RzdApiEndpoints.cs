namespace RZD.Test.API;

public struct RzdApiEndpoints
{
    public static string Suggests(string query)
        => $"/api/v1/suggests?GroupResults=true&Query={query}";

    public static string CarPricing => $"/apib2b/p/Railway/V1/Search/CarPricing";

    public static string TrainPricing(string origin, string destination, DateTime departureDate)
        => $"/api/v1/railway-service/prices/train-pricing?service_provider=B2B_RZD&origin={origin}&destination={destination}&departureDate={departureDate}";

    public static string TrainRoute(string trainNumber, string origin, string destination, DateTime departureDate)
        => $"/apib2b/p/Railway/V1/Search/TrainRoute?TrainNumber={trainNumber}&Origin={origin}&Destination={destination}&DepartureDate={departureDate}";
}

