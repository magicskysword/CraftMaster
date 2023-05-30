using CraftMaster.Feats;
using CraftMaster.Reference;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Utility;
using Newtonsoft.Json;

namespace CraftMaster.Builder;

public class WandBuilder : UsableBuilder
{
    public override bool IsValid()
    {
        return true;
    }

    public override void OnValidate()
    {
        if (DC <= 0)
        {
            GenerateDC();
        }
        
        if (CasterLevel <= 0)
        {
            CasterLevel = 1;
        }
        
        if (SpellLevel <= 0)
        {
            SpellLevel = 1;
        }

        if (string.IsNullOrEmpty(PropertyGuid))
        {
            PropertyGuid = ReferenceManager.Wand.BasePrototypeGuid.Random();
        }
    }

    public override int GetRawCost()
    {
        var baseCost = 750;
        baseCost *= SpellLevel;
        baseCost *= CasterLevel;
        
        return baseCost;
    }

    public override int GetBuildPoint()
    {
        return 10 + CasterLevel * 5 + SpellLevel * 10;
    }

    public override int GetCheckDC()
    {
        return 20 + CasterLevel * 2 + SpellLevel * 2;
    }

    public void GenerateDC(UnitEntityData unitEntityData = null)
    {
        DC = 10 + CasterLevel;
        if (unitEntityData != null && (unitEntityData.HasFeature(CraftWandMythicFeat.FeatGuid) || Main.Settings.IgnoreMythicFeatureLimit))
        {
            DC += unitEntityData.Progression.MythicLevel;
        }
    }
}