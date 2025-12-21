using MassTransit;
using WalletCore.Contrtacts.CommandContracts;
using WalletCore.DataService.Infrastructure.Interfaces;

namespace WalletCore.DataService.Consumers
{
    public class UpdateWalletBalanceConsumer
    : IConsumer<UpdateWalletBalanceCommand>
    {
        private readonly IWalletRepository _walletRepository;

        public UpdateWalletBalanceConsumer(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task Consume(
            ConsumeContext<UpdateWalletBalanceCommand> context)
        {
            var msg = context.Message;

            var wallet = await _walletRepository.GetByIdAsync(msg.WalletId);
            await _walletRepository.UpdateBalanceAsync(wallet, msg.NewBalance);
        }
    }
}
