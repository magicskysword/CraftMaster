using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;

namespace CraftMaster.Feats;

public class CraftWandMythicFeat
{
    private static readonly string FeatName = "CraftWandMythicFeat";
    public static readonly string FeatGuid = "5EC9623E-0169-48AB-990A-193AFACEA16C";

    private static readonly string DisplayName = "CraftWandMythicFeat.Name";
    private static readonly string Description = "CraftWandMythicFeat.Description";

    public static void Configure()
    {
        var requireFeat = new PrerequisiteFeature();
        requireFeat.Group = Prerequisite.GroupType.Any;
        requireFeat.m_Feature = BlueprintTool.Get<BlueprintFeature>(CraftWand.FeatGuid)
            .ToReference<BlueprintFeatureReference>();
        
        FeatureConfigurator.New(FeatName, FeatGuid, FeatureGroup.MythicFeat)
            .SetDisplayName(DisplayName)
            .SetDescription(Description)
            .AddComponent(requireFeat)
            .AddFeatureTagsComponent(FeatureTag.Magic)
            .AddFeatureTagsComponent(FeatureTag.Skills)
            .Configure();
    }
}