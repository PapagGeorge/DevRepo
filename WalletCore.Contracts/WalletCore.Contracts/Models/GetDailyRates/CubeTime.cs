using System.Collections.Generic;
using System.Xml.Serialization;

namespace WalletCore.Contrtacts.GetDailyRates
{
    public class CubeTime
    {
        [XmlAttribute(AttributeName = "time")]
        public string Time { get; set; }

        [XmlElement(ElementName = "Cube")]
        public List<CurrencyRate> Rates { get; set; }
    }
}
