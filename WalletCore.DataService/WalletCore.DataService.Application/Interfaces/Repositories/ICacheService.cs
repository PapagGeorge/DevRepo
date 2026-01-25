using WalletCore.Contrtacts.DBModels;

namespace WalletCore.DataService.Application.Interfaces.Repositories
{
    public interface ICacheService
    {
        Task UpdateExchangeRatesAsync(IEnumerable<ExchangeRate> rates, CancellationToken ct = default);
        Task<List<ExchangeRate>> GetLatestExchangeRatesAsync(CancellationToken ct = default);
    }
}
