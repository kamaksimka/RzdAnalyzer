using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RZD.Api.Models;
using RZD.Integration;
using RZD.Common.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RZD.Application.Services;
using RZD.Database;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RzdConfig>(builder.Configuration.GetSection(RzdConfig.Section));
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(JwtConfig.Section));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Введите токен в формате Bearer {ваш токен}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
builder.Services.AddControllers();

builder.Services.AddDbContextFactory<DataContext>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TrackedRouteService>();
builder.Services.AddScoped<FeedbackService>();


var jwtConfig = builder.Configuration.GetSection(JwtConfig.Section).Get<JwtConfig>()!;

// Настройка аутентификации с использованием JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();


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


using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    await SeedData.SeedAsync(context);
}


app.Run();

