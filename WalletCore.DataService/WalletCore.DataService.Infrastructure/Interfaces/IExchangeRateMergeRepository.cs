using WalletCore.Contrtacts.DBModels;

namespace WalletCore.DataService.Infrastructure.Interfaces
{
    public interface IExchangeRateMergeRepository
    {
        Task MergeRatesAsync(IEnumerable<ExchangeRate> rates, CancellationToken cancellationToken = default);
    }
}
