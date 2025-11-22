using WalletCore.Domain.Models.AdjustBalance;

namespace WalletCore.Application
{
    public static class Mappings
    {
        public static string ToMessage(this WalletStrategyOperation strategy)
        {
            return strategy switch
            {
                WalletStrategyOperation.AddFunds => "Funds were successfully added.",
                WalletStrategyOperation.SubtractFunds => "Funds were successfully subtracted.",
                WalletStrategyOperation.ForceSubtractFunds => "Funds were forcefully subtracted.",
                _ => "Unknown operation."
            };
        }
    }
}
