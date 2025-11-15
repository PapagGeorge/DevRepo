using System.Xml.Serialization;

namespace WalletCore.Domain.Models.GetDailyRates
{
    public class CurrencyRate
    {
        [XmlAttribute(AttributeName = "currency")]
        public string Currency { get; set; }

        [XmlAttribute(AttributeName = "rate")]
        public decimal Rate { get; set; }
    }
}
