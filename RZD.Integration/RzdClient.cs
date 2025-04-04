using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Text.Json;
using RZD.Common.Configs;
using System.Web;
using System.Diagnostics;

namespace RZD.Integration
{
    public class RzdClient:IDisposable
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private static Stopwatch stopwatch = Stopwatch.StartNew();
        private static readonly SemaphoreSlim semaphore = new(1, 1);
        private readonly RzdConfig rzdConfig;

        public RzdClient(RzdConfig rzdConfig)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(rzdConfig.BaseAddress);
            _client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 YaBrowser/25.2.0.0 Safari/537.36");
            this.rzdConfig = rzdConfig;
        }

        public virtual async Task<T> SendPostRequestAsync<T>(string route, object? data = null, Dictionary<string, string>? queryParams = null) where T : class
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, GetRouteWithQueryParams(route, queryParams))
            {
                Content = data != null ? JsonContent.Create(data) : null
            };


            return await SendRequestAsync<T>(requestMessage);
        }

        public virtual async Task<T> SendGetRequestAsync<T>(string route, Dictionary<string, string>? queryParams = null) where T : class
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, GetRouteWithQueryParams(route, queryParams));

            return await SendRequestAsync<T>(requestMessage);

        }

        protected string GetBase64AuthenticationString(string username, string password)
        {
            var authenticationString = $"{username}:{password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

            return base64EncodedAuthenticationString;
        }

        private async Task<T> SendRequestAsync<T>(HttpRequestMessage requestMessage) where T : class
        {
            await WaitIfNeededAsync();

            using var response = _client.SendAsync(requestMessage).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception($"Ошибка авторизации. Route: {requestMessage.RequestUri}");
            }

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);

            return json;
        }

        private static string GetRouteWithQueryParams(string route, Dictionary<string, string>? queryParams)
        {
            if (queryParams == null)
            {
                return route;
            }

            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value?.ToString();
            }

            return $"{route}?{query}";
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private async Task WaitIfNeededAsync()
        {
            await semaphore.WaitAsync();
            try
            {
                long elapsedMs = stopwatch.ElapsedMilliseconds;
                if (elapsedMs < rzdConfig.TimeBetweenRequests)
                {
                    await Task.Delay(rzdConfig.TimeBetweenRequests - (int)elapsedMs);
                }
                stopwatch.Restart();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
