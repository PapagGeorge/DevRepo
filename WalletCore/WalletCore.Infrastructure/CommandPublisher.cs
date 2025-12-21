using MassTransit;
using WalletCore.Application.Interfaces;
using WalletCore.Contrtacts.CommandContracts;

namespace WalletCore.Infrastructure
{
    public class CommandPublisher : ICommandPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public CommandPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishCreateWalletAsync(Guid walletId, string currency)
        {
            var command = new CreateWalletCommand(walletId, currency);

            try
            {
                await _publishEndpoint.Publish(command);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task PublishUpdateWalletBalanceAsync(Guid walletId, decimal newBalance)
        {
            var command = new UpdateWalletBalanceCommand(walletId, newBalance);

            try
            {
                await _publishEndpoint.Publish(command);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task PublishMergeExchangeRatesAsync(IReadOnlyCollection<ExchangeRateDto> rates)
        {
            var command = new MergeExchangeRatesCommand(rates);

            try
            {
                await _publishEndpoint.Publish(command);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
