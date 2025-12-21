namespace WalletCore.DataService.DataContracts.Models.AdjustBalance
{
    public class AdjustBalanceResponse
    {
        public Guid WalletId { get; set; }
        public decimal OldBalance { get; set; }
        public decimal NewBalance { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public decimal AppliedAmount { get; set; }
        public string WalletCurrency { get; set; }
    }
}
