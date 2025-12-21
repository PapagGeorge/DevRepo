using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WalletCore.Contrtacts.AdjustBalance
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WalletStrategyOperation
    {
        [EnumMember(Value = "subtractFunds")]
        SubtractFunds,
        [EnumMember(Value = "addFunds")]
        AddFunds,
        [EnumMember(Value = "forceSubtractFunds")]
        ForceSubtractFunds
    }
}