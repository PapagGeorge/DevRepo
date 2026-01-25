using System;

namespace WalletCore.Contracts.DBModels
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
    }
}
