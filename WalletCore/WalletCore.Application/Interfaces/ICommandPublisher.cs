using WalletCore.Contrtacts.CommandContracts;
using WalletCore.Domain.DBModels;

namespace WalletCore.Application.Interfaces
{
    public interface ICommandPublisher
    {
        Task PublishCreateWalletAsync(Guid walletId, string currency);
        Task PublishUpdateWalletBalanceAsync(Guid walletId, decimal newBalance);
        Task PublishMergeExchangeRatesAsync(IReadOnlyCollection<ExchangeRateDto> rates);
    }
}
