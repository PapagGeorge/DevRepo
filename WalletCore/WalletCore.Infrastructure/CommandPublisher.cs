using MassTransit;
using WalletCore.Application.Interfaces;
using WalletCore.Contrtacts.CommandContracts;
using WalletCore.Contrtacts.DBModels;

namespace WalletCore.Infrastructure
{
    public class CommandPublisher : ICommandPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public CommandPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishCreateWalletAsync(Wallet wallet)
        {
            var command = new CreateWalletCommand(wallet);

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
