using WalletCore.Contrtacts.CommandContracts;
using WalletCore.Contrtacts.DBModels;

namespace WalletCore.Application.Interfaces
{
    public interface ICommandPublisher
    {
        Task PublishCreateWalletAsync(Wallet wallet);
        Task PublishMergeExchangeRatesAsync(IReadOnlyCollection<ExchangeRateDto> rates);
    }
}
