using System.Xml.Serialization;

namespace WalletCore.Contracts.GetDailyRates
{
    public class CurrencyRate
    {
        [XmlAttribute(AttributeName = "currency")]
        public string Currency { get; set; }

        [XmlAttribute(AttributeName = "rate")]
        public decimal Rate { get; set; }
    }
}
