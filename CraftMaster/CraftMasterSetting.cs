using System.Xml.Serialization;
using UnityModManagerNet;

namespace CraftMaster;

public class CraftMasterSetting : UnityModManager.ModSettings
{
    [XmlElement]
    public bool InstantCraft { get; set; } = false;
    [XmlElement]
    public bool IgnoreFeatureLimit { get; set; } = false;
    [XmlElement]
    public bool IgnoreMythicFeatureLimit { get; set; }
    [XmlElement]
    public float CraftSpeed { get; set; } = 1.0f;
}