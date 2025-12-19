using MassTransit;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.CommandContracts;

namespace WalletCore.Infrastructure
{
    public class CommandPublisher : ICommandPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public CommandPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task PublishCreateWalletAsync(Guid walletId, string currency)
        {
            var command = new CreateWalletCommand(walletId, currency);
            return _publishEndpoint.Publish(command);
        }

        public Task PublishUpdateWalletBalanceAsync(Guid walletId, decimal newBalance)
        {
            var command = new UpdateWalletBalanceCommand(walletId, newBalance);
            return _publishEndpoint.Publish(command);
        }

        public Task PublishMergeExchangeRatesAsync(IReadOnlyCollection<ExchangeRateDto> rates)
        {
            var command = new MergeExchangeRatesCommand(rates);
            return _publishEndpoint.Publish(command);
        }
    }
}
