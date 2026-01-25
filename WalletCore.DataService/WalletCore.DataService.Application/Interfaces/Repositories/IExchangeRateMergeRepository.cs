using WalletCore.Contracts.DBModels;

namespace WalletCore.DataService.Application.Interfaces.Repositories
{
    public interface IExchangeRateMergeRepository
    {
        Task MergeRatesAsync(IEnumerable<ExchangeRate> rates, CancellationToken cancellationToken = default);
    }
}
