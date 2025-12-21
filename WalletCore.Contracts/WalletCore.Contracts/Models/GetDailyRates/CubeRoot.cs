using System.Collections.Generic;
using System.Xml.Serialization;

namespace WalletCore.Contrtacts.GetDailyRates
{
    public class CubeRoot
    {
        [XmlElement(ElementName = "Cube")]
        public List<CubeTime> TimeCubes { get; set; }
    }
}
