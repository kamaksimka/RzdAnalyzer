

using System.Text.Json.Nodes;
using JsonSerializer = System.Text.Json.JsonSerializer;

using var httpClient = new HttpClient();

//var s1 = GetCities("Шахты");
//var s2 = GetCities("Санкт-Петербург");

//var client = new RzdClient(new HttpClient());
//var api = new RzdApi(client);

//var model = await api.SuggestsAsync("Москва");

//foreach (var city in model.City)
//{
//    Console.WriteLine(city.Name +" " + city.Region + city.NodeId );
//}

//var handler = new HttpClientHandler
//{
//    CookieContainer = new System.Net.CookieContainer()
//};


var t1 = DateTime.Now;
var client = new HttpClient();

var s = client.GetAsync(
    "https://pass.rzd.ru/timetable/public/?layer_id=5827&dir=0&code0=2004000&code1=2064170&tfl=3&checkSeats=0&dt0=07.03.2025&md=0").Result.Content.ReadAsStringAsync().Result;

var jsonRes = JsonObject.Parse(s);

var res = client
    .PostAsync("https://pass.rzd.ru/timetable/public/ru?layer_id=5827&json=y",
        new FormUrlEncodedContent(new Dictionary<string, string>() { { "rid", jsonRes["RID"].ToString() } })).Result
    .Content.ReadAsStringAsync().Result;


var t2 = DateTime.Now;

Console.WriteLine(t2-t1);

var t3 = DateTime.Now;
var client1 = new HttpClient();

var s1 = client1.GetAsync(
    "https://ticket.rzd.ru/api/v1/railway-service/prices/train-pricing?service_provider=B2B_RZD&origin=2004000&destination=2064170&departureDate=2025-03-07T00:00:00").Result.Content.ReadAsStringAsync().Result;


var t4 = DateTime.Now;

Console.WriteLine(t4 - t3);