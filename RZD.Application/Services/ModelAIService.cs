using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RZD.Application.Models;
using RZD.Common.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RZD.Application.Services
{
    public class ModelAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public ModelAIService(HttpClient httpClient, IOptions<RzdConfig> rzdConfig)
        {
            _httpClient = httpClient;
            _apiUrl = rzdConfig.Value.ModelAIUrl;
        }

        public async Task<Dictionary<DateTime,int>> PredictFreePlacesAsync(PredictRequest request)
        {
            var content = JsonContent.Create(request);

            var response = await _httpClient.PostAsync($"{_apiUrl}/predictFreePlaces", content);
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<Dictionary<DateTime, decimal>>();

            return model.ToDictionary(k => k.Key,v => Convert.ToInt32(v.Value));
        }

        public async Task<Dictionary<DateTime, decimal>> PredictMinPriceAsync(PredictRequest request)
        {
            var content = JsonContent.Create(request);

            var response = await _httpClient.PostAsync($"{_apiUrl}/predictMinPrice", content);
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<Dictionary<DateTime, decimal>>();

            return model;
        }

        public async Task<Dictionary<DateTime, decimal>> PredictMaxPriceAsync(PredictRequest request)
        {
            var content = JsonContent.Create(request);

            var response = await _httpClient.PostAsync($"{_apiUrl}/predictMaxPrice", content);
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<Dictionary<DateTime, decimal>>();

            return model;
        }
    }
}
