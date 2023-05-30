namespace CraftMaster;

public partial class Main
{

    private static bool _toggleModSetting = false;
    private static void RenderModSetting()
    {
        _toggleModSetting = CMGUI.NToggle("CraftMaster.UI.ModCheating", _toggleModSetting);
        if (_toggleModSetting)
        {
            CMGUI.Box(() =>
            {
                Settings.InstantCraft = CMGUI.NToggle("CraftMaster.UI.InstantCraft", Settings.InstantCraft);
                Settings.IgnoreFeatureLimit = CMGUI.NToggle("CraftMaster.UI.IgnoreFeatureLimit", Settings.IgnoreFeatureLimit);
                Settings.IgnoreMythicFeatureLimit = CMGUI.NToggle("CraftMaster.UI.IgnoreMythicFeatureLimit", Settings.IgnoreMythicFeatureLimit);
                Settings.CraftSpeed = CMGUI.NSliderFloat("CraftMaster.UI.CraftSpeed", Settings.CraftSpeed, 0.1f, 10f);
            });
        }
    }
}