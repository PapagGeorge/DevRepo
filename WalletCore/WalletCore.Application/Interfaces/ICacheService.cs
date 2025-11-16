using WalletCore.Domain.DBModels;

namespace WalletCore.Application.Interfaces
{
    public interface ICacheService
    {
        Task UpdateExchangeRatesAsync(IEnumerable<ExchangeRate> rates, CancellationToken ct = default);
        Task<List<ExchangeRate>> GetLatestExchangeRatesAsync(CancellationToken ct = default);
    }
}
