

using RZD.API;
using System.Text.Json.Nodes;
using JsonSerializer = System.Text.Json.JsonSerializer;

using var httpClient = new HttpClient();

var api = new RzdApi(new RZD.Common.Configs.RzdConfig
{
    BaseAddress = "https://ticket.rzd.ru",
    TimeBetweenRequests = 2000,
});

var r = await api.CarPricingAsync("2006004", "2004006", DateTimeOffset.Parse("2025-04-02T23:00:00").DateTime, "038А");
var a = 1;