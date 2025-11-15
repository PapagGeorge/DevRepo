namespace WalletCore.Application.Interfaces
{
    public interface IGenericHttpClient
    {
        Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken ct = default);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken ct = default);
        Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken ct = default);
        Task<TResponse?> PatchAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(string endpoint, CancellationToken ct = default);
        Task<TResponse> GetXmlAsync<TResponse>(string endpoint, CancellationToken ct = default);
    }
}
