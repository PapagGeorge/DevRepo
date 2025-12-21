using MassTransit;
using Microsoft.Extensions.Logging;
using WalletCore.DataService.DataContracts;
using WalletCore.Contrtacts.CommandContracts;
using WalletCore.DataService.Infrastructure.Interfaces;

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
                message.WalletId);

            var wallet = new Wallet
            {
                Id = message.WalletId,
                Currency = message.Currency,
                Balance = 0m
            };

            await _walletRepository.AddAsync(wallet);

            _logger.LogInformation(
                "Wallet {WalletId} created successfully",
                message.WalletId);
        }
    }
}
