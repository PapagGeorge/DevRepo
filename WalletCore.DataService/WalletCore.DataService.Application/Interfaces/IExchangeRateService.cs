using WalletCore.Contracts.DBModels;

namespace WalletCore.DataService.Application.Interfaces
{
    public interface IExchangeRateService
    {
        Task MergeRatesAsync(IEnumerable<ExchangeRate> rates, CancellationToken cancellationToken = default);
    }
}
