using WalletCore.Contrtacts.DBModels;

namespace WalletCore.DataService.Services.Infrastructure.Interfaces
{
    public interface ICacheService
    {
        Task UpdateExchangeRatesAsync(IEnumerable<ExchangeRate> rates, CancellationToken ct = default);
        Task<List<ExchangeRate>> GetLatestExchangeRatesAsync(CancellationToken ct = default);
    }
}
