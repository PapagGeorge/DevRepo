using System.Xml.Serialization;

namespace WalletCore.Contrtacts.GetDailyRates
{
    public class Sender
    {
        [XmlElement(ElementName = "name", Namespace = "")]
        public string Name { get; set; }
    }
}
