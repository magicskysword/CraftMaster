using UnityModManagerNet;

namespace CraftMaster;

public class CraftMasterSetting : UnityModManager.ModSettings
{
    public bool InstantCraft { get; set; } = false;
    public bool IgnoreFeatureLimit { get; set; } = false;
    public bool IgnoreMythicFeatureLimit { get; set; }
    public float CraftSpeed { get; set; } = 1.0f;
}