using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RZD.Api.Models;
using RZD.Integration;
using RZD.Common.Configs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RzdConfig>(builder.Configuration.GetSection(RzdConfig.Section));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.MapPost("TrainPricing", async ([FromBody] TrainPricingRequest request, IOptions<RzdConfig> options) =>
{
    var rzdApi = new RzdApi(options.Value);
    return await rzdApi.TrainPricingAsync(request.OriginCode, request.DestinationCode, request.DepartureDate);
});

app.MapPost("CarPricing", async ([FromBody] CarPricingRequest request,IOptions<RzdConfig> options) =>
{
    var rzdApi = new RzdApi(options.Value);
    return await rzdApi.CarPricingAsync(request.OriginCode, request.DestinationCode, request.DepartureDate, request.TrainNumber);
});

app.Run();

