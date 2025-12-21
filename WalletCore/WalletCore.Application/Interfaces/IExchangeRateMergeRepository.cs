using WalletCore.Contrtacts.DBModels;

namespace WalletCore.Application.Interfaces
{
    public interface IExchangeRateMergeRepository
    {
        Task MergeRatesAsync(IEnumerable<ExchangeRate> rates, CancellationToken cancellationToken = default);
    }
}
