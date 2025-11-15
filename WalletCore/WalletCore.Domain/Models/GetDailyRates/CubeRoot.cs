using System.Xml.Serialization;

namespace WalletCore.Domain.Models.GetDailyRates
{
    public class CubeRoot
    {
        [XmlElement(ElementName = "Cube")]
        public List<CubeTime> TimeCubes { get; set; }
    }
}
