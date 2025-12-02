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
                LogRequest(HttpMethod.Get, endpoint);

                var response = await HttpClient.GetAsync(endpoint, ct);
                var content = await response.Content.ReadAsStringAsync(ct);

                LogResponse(response.StatusCode, endpoint, content);
                response.EnsureSuccessStatusCode();

                if (string.IsNullOrWhiteSpace(content))
                    return default;

                return JsonSerializer.Deserialize<TResponse>(content, JsonOptions);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in GET JSON request to {Endpoint}", endpoint);
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
                LogRequest(HttpMethod.Post, endpoint, json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await HttpClient.PostAsync(endpoint, content, ct);
                var responseContent = await response.Content.ReadAsStringAsync(ct);

                LogResponse(response.StatusCode, endpoint, responseContent);
                response.EnsureSuccessStatusCode();

                if (string.IsNullOrWhiteSpace(responseContent))
                    return default;

                return JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in POST request to {Endpoint}", endpoint);
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
                LogRequest(HttpMethod.Put, endpoint, json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await HttpClient.PutAsync(endpoint, content, ct);
                var responseContent = await response.Content.ReadAsStringAsync(ct);

                LogResponse(response.StatusCode, endpoint, responseContent);
                response.EnsureSuccessStatusCode();

                if (string.IsNullOrWhiteSpace(responseContent))
                    return default;

                return JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in PUT request to {Endpoint}", endpoint);
                throw;
            }
        }

        protected async Task<TResponse> GetXmlAsync<TResponse>(
            string endpoint,
            CancellationToken ct = default)
        {
            try
            {
                LogRequest(HttpMethod.Get, endpoint);

                var response = await HttpClient.GetAsync(endpoint, ct);
                var content = await response.Content.ReadAsStringAsync(ct);

                LogResponse(response.StatusCode, endpoint, content);
                response.EnsureSuccessStatusCode();

                var serializer = new XmlSerializer(typeof(TResponse));
                using var reader = new StringReader(content);
                return (TResponse)serializer.Deserialize(reader)!;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in XML GET request to {Endpoint}", endpoint);
                throw;
            }
        }

        protected async Task<bool> DeleteAsync(string endpoint, CancellationToken ct = default)
        {
            try
            {
                LogRequest(HttpMethod.Delete, endpoint);

                var response = await HttpClient.DeleteAsync(endpoint, ct);

                LogResponse(response.StatusCode, endpoint);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in DELETE request to {Endpoint}", endpoint);
                throw;
            }
        }

        private void LogRequest(HttpMethod method, string endpoint, string? body = null)
        {
            var url = GetFullUrl(endpoint);
            if (body != null)
                Logger.LogInformation("HTTP {Method} {Url} - Body: {Body}", method, url, body);
            else
                Logger.LogInformation("HTTP {Method} {Url}", method, url);
        }

        private void LogResponse(System.Net.HttpStatusCode statusCode, string endpoint, string? body = null)
        {
            var url = GetFullUrl(endpoint);
            Logger.LogInformation("HTTP Response {StatusCode} from {Url} - Body: {Body}",
                statusCode, url, body ?? "<empty>");
        }

        private string GetFullUrl(string endpoint)
        {
            if (Uri.TryCreate(endpoint, UriKind.Absolute, out _))
                return endpoint;

            var baseAddress = HttpClient.BaseAddress?.ToString().TrimEnd('/') ?? "";
            var relativePath = endpoint.TrimStart('/');
            return $"{baseAddress}/{relativePath}";
        }
    }
}
