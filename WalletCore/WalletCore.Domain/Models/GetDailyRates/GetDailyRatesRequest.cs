using System.Xml.Serialization;

namespace WalletCore.Domain.Models.GetDailyRates
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
    public class GesmesEnvelope
    {
        [XmlElement(ElementName = "subject", Namespace = "")]
        public string Subject { get; set; }

        [XmlElement(ElementName = "Sender", Namespace = "")]
        public Sender Sender { get; set; }

        [XmlElement(ElementName = "Cube", Namespace = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref")]
        public CubeRoot Cube { get; set; }
    }
}
