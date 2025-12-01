using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using WalletCore.Application.Interfaces;

namespace WalletCore.Application.HttpClientInfrastructure
{
    public class GenericHttpClient : IGenericHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GenericHttpClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public GenericHttpClient(HttpClient httpClient, ILogger<GenericHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken ct = default) =>
            SendAsync<TResponse>(new HttpRequestMessage(HttpMethod.Get, endpoint), null, ct);

        public Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken ct = default) =>
            SendWithBodyAsync<TRequest, TResponse>(HttpMethod.Post, endpoint, request, ct);

        public Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken ct = default) =>
            SendWithBodyAsync<TRequest, TResponse>(HttpMethod.Put, endpoint, request, ct);

        public Task<TResponse?> PatchAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken ct = default) =>
            SendWithBodyAsync<TRequest, TResponse>(HttpMethod.Patch, endpoint, request, ct);

        public async Task<bool> DeleteAsync(string endpoint, CancellationToken ct = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            var response = await _httpClient.SendAsync(request, ct);
            await LogResponseAsync(response, request);
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<TResponse> GetXmlAsync<TResponse>(string endpoint, CancellationToken ct = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            await LogRequestAsync(request, null);

            var response = await _httpClient.SendAsync(request, ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            await LogResponseAsync(response, request, content);
            response.EnsureSuccessStatusCode();

            var serializer = new XmlSerializer(typeof(TResponse));
            using var reader = new StringReader(content);
            return (TResponse)serializer.Deserialize(reader)!;
        }

        private async Task<TResponse?> SendWithBodyAsync<TRequest, TResponse>(HttpMethod method, string endpoint, TRequest requestObj, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize(requestObj, _jsonOptions);
            var httpRequest = new HttpRequestMessage(method, endpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            return await SendAsync<TResponse>(httpRequest, json, ct);
        }

        private async Task<TResponse?> SendAsync<TResponse>(HttpRequestMessage request, string? body, CancellationToken ct)
        {
            try
            {
                await LogRequestAsync(request, body);
                var response = await _httpClient.SendAsync(request, ct);
                return await HandleResponseWithLogging<TResponse>(response, request, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} request to {Url}", request.Method, GetFullRequestUrl(request));
                throw;
            }
        }

        private async Task<TResponse?> HandleResponseWithLogging<TResponse>(HttpResponseMessage response, HttpRequestMessage request, CancellationToken ct)
        {
            var responseContent = await response.Content.ReadAsStringAsync(ct);
            await LogResponseAsync(response, request, responseContent);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}");

            if (string.IsNullOrWhiteSpace(responseContent)) return default;

            return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
        }

        private async Task LogRequestAsync(HttpRequestMessage request, string? body)
        {
            var headers = MaskSensitiveHeaders(request.Headers);
            _logger.LogInformation("HTTP Request {Method} {Url}\nHeaders: {Headers}\nBody: {Body}",
                request.Method, GetFullRequestUrl(request), headers, body ?? "<empty>");
        }

        private async Task LogResponseAsync(HttpResponseMessage response, HttpRequestMessage request, string? body = null)
        {
            var headers = MaskSensitiveHeaders(response.Headers);
            _logger.LogInformation("HTTP Response {StatusCode} for {Method} {Url}\nHeaders: {Headers}\nBody: {Body}",
                response.StatusCode, request.Method, GetFullRequestUrl(request), headers, body ?? "<empty>");
        }

        private string GetFullRequestUrl(HttpRequestMessage request)
        {
            if (request.RequestUri.IsAbsoluteUri)
                return request.RequestUri.ToString();
            var baseAddress = _httpClient.BaseAddress?.ToString().TrimEnd('/') ?? "";
            var relativePath = request.RequestUri?.ToString().TrimStart('/') ?? "";
            return $"{baseAddress}/{relativePath}";
        }

        private static string MaskSensitiveHeaders(HttpHeaders headers)
        {
            return string.Join(", ", headers.Select(h =>
            {
                var value = h.Key.ToLower().Contains("token") || h.Key.ToLower().Contains("apikey") ? "****" : string.Join(";", h.Value);
                return $"{h.Key}: {value}";
            }));
        }
    }
}
