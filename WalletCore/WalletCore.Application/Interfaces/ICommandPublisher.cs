using WalletCore.Contrtacts.CommandContracts;

namespace WalletCore.Application.Interfaces
{
    public interface ICommandPublisher
    {
        Task PublishMergeExchangeRatesAsync(IReadOnlyCollection<ExchangeRateDto> rates);
    }
}
