using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using RZD.Test.Configs;

namespace RZD.Test.API
{
    public class RzdClient
    {
        private readonly HttpClient _client;

        public RzdClient(HttpClient client, RzdConfig config)
        {
            _client = client;
            _client.BaseAddress = new Uri(config.BaseAddress);
        }

        public virtual async Task<T> SendPostRequestAsync<T>(string route, object? data = null) where T : class
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, route)
            {
                Content = data != null ? JsonContent.Create(data) : null
            };


            return await SendRequestAsync<T>(requestMessage);
        }

        public virtual async Task<T> SendGetRequestAsync<T>(string route, Dictionary<string, string>? queryParams = null) where T : class
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, GetUriWithQueryParams(route, queryParams));

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
            using var response = _client.SendAsync(requestMessage).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception($"Ошибка авторизации. Route: {requestMessage.RequestUri}");
            }

            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();

            var json = JsonConvert.DeserializeObject<T>(jsonString);

            if (json == null)
            {
                throw new Exception($"Ошибка десериализации модели {jsonString} по пути {requestMessage.RequestUri!.ToString()} в тип {typeof(T).FullName}");
            }

            return json;
        }

        private Uri GetUriWithQueryParams(string route, Dictionary<string, string>? queryParams)
        {
            if (queryParams == null)
                return new Uri(route);

            var uriBuilder = new UriBuilder(route)
            {
                Query = string.Join("&", queryParams.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"))
            };

            return uriBuilder.Uri;
        }
    }
}
