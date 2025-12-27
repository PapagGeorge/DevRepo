using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace WalletCore.Infrastructure.HttpClientInfrastructure
{
    public abstract class BaseHttpClient
    {
        protected readonly HttpClient HttpClient;
        protected readonly ILogger Logger;
        protected readonly JsonSerializerOptions JsonOptions;

        protected BaseHttpClient(HttpClient httpClient, ILogger logger)
        {
            HttpClient = httpClient;
            Logger = logger;
            JsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected async Task<TResponse?> GetJsonAsync<TResponse>(
            string endpoint,
            CancellationToken ct = default)
        {
            try
            {
                var response = await HttpClient.GetAsync(endpoint, ct);
                var content = await response.Content.ReadAsStringAsync(ct);

                response.EnsureSuccessStatusCode();

                if (string.IsNullOrWhiteSpace(content))
                    return default;

                return JsonSerializer.Deserialize<TResponse>(content, JsonOptions);
            }
            catch (JsonException ex)
            {
                Logger.LogError(ex, "Failed to deserialize JSON from {Endpoint}", endpoint);
                throw;
            }
            catch (HttpRequestException ex)
            {
                Logger.LogError(ex, "HTTP GET request failed for {Endpoint}", endpoint);
                throw;
            }
        }

        protected async Task<TResponse?> PostJsonAsync<TRequest, TResponse>(
            string endpoint,
            TRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, JsonOptions);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await HttpClient.PostAsync(endpoint, content, ct);
                var responseContent = await response.Content.ReadAsStringAsync(ct);

                response.EnsureSuccessStatusCode();

                if (string.IsNullOrWhiteSpace(responseContent))
                    return default;

                return JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
            }
            catch (JsonException ex)
            {
                Logger.LogError(ex, "Failed to deserialize JSON response from {Endpoint}", endpoint);
                throw;
            }
            catch (HttpRequestException ex)
            {
                Logger.LogError(ex, "HTTP POST request failed for {Endpoint}", endpoint);
                throw;
            }
        }

        protected async Task<TResponse?> PutJsonAsync<TRequest, TResponse>(
            string endpoint,
            TRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, JsonOptions);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await HttpClient.PutAsync(endpoint, content, ct);
                var responseContent = await response.Content.ReadAsStringAsync(ct);

                response.EnsureSuccessStatusCode();

                if (string.IsNullOrWhiteSpace(responseContent))
                    return default;

                return JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
            }
            catch (JsonException ex)
            {
                Logger.LogError(ex, "Failed to deserialize JSON response from {Endpoint}", endpoint);
                throw;
            }
            catch (HttpRequestException ex)
            {
                Logger.LogError(ex, "HTTP PUT request failed for {Endpoint}", endpoint);
                throw;
            }
        }

        protected async Task<TResponse> GetXmlAsync<TResponse>(
            string endpoint,
            CancellationToken ct = default)
        {
            try
            {
                var response = await HttpClient.GetAsync(endpoint, ct);
                var content = await response.Content.ReadAsStringAsync(ct);

                response.EnsureSuccessStatusCode();

                var serializer = new XmlSerializer(typeof(TResponse));
                using var reader = new StringReader(content);
                return (TResponse)serializer.Deserialize(reader)!;
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError(ex, "Failed to deserialize XML response from {Endpoint}", endpoint);
                throw;
            }
            catch (HttpRequestException ex)
            {
                Logger.LogError(ex, "HTTP GET request failed for {Endpoint}", endpoint);
                throw;
            }
        }

        protected async Task<bool> DeleteAsync(string endpoint, CancellationToken ct = default)
        {
            try
            {
                var response = await HttpClient.DeleteAsync(endpoint, ct);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                Logger.LogError(ex, "HTTP DELETE request failed for {Endpoint}", endpoint);
                throw;
            }
        }
    }
}
