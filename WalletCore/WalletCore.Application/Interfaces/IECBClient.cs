namespace WalletCore.Application.Interfaces
{
    public interface IECBClient
    {
        Task<TResponse> GetXmlAsync<TResponse>(string endpoint, CancellationToken ct = default);
    }
}
