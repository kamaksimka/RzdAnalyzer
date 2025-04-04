


using RZD.Integration;
using System.Text.Json.Nodes;
using JsonSerializer = System.Text.Json.JsonSerializer;

using var httpClient = new HttpClient();

var api = new RzdApi(new RZD.Common.Configs.RzdConfig
{
    BaseAddress = "https://ticket.rzd.ru",
    TimeBetweenRequests = 2000,
});

var r = await api.CarPricingAsync("2006000", "2004000", DateTimeOffset.Parse("2025-04-09T09:40:00").DateTime, "762А");
var a = 1;
