using MassTransit;
using Microsoft.Extensions.Logging;
using WalletCore.Contrtacts.CommandContracts;
using WalletCore.DataService.Infrastructure.Interfaces;
using WalletCore.Contrtacts.DBModels;

namespace WalletCore.DataService.Infrastructure.Consumers
{
    public class CreateWalletConsumer : IConsumer<CreateWalletCommand>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<CreateWalletConsumer> _logger;

        public CreateWalletConsumer(
            IWalletRepository walletRepository,
            ILogger<CreateWalletConsumer> logger)
        {
            _walletRepository = walletRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateWalletCommand> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Consuming CreateWalletCommand for WalletId {WalletId}",
                message.wallet.Id);

            await _walletRepository.AddAsync(message.wallet);

            _logger.LogInformation(
                "Wallet with Id: {WalletId} created successfully",
                message.wallet.Id);
        }
    }
}
