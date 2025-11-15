using System.Xml.Serialization;

namespace WalletCore.Domain.Models.GetDailyRates
{
    public class CubeTime
    {
        [XmlAttribute(AttributeName = "time")]
        public string Time { get; set; }

        [XmlElement(ElementName = "Cube")]
        public List<CurrencyRate> Rates { get; set; }
    }
}
