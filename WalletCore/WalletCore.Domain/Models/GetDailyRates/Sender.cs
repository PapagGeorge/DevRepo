using System.Xml.Serialization;

namespace WalletCore.Domain.Models.GetDailyRates
{
    public class Sender
    {
        [XmlElement(ElementName = "name", Namespace = "")]
        public string Name { get; set; }
    }
}
