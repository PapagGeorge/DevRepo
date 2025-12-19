using WalletCore.DataService.DataContracts;

namespace WalletCore.DataService.Infrastructure.Interfaces
{
    public interface IExchangeRateMergeRepository
    {
        Task MergeRatesAsync(IEnumerable<ExchangeRate> rates, CancellationToken cancellationToken = default);
    }
}
