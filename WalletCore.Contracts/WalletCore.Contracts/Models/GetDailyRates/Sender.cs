using System.Xml.Serialization;

namespace WalletCore.Contracts.GetDailyRates
{
    public class Sender
    {
        [XmlElement(ElementName = "name", Namespace = "")]
        public string Name { get; set; }
    }
}
