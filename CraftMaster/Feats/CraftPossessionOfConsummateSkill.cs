using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;

namespace CraftMaster.Feats;

public class CraftPossessionOfConsummateSkill
{
    public static readonly string FeatName = "CraftPossessionOfConsummateSkill";
    public static readonly string FeatGuid = "EAA18ADB-D661-4468-8FFE-758B5543C9CA";

    private static readonly string DisplayName = "CraftPossessionOfConsummateSkill.Name";
    private static readonly string Description = "CraftPossessionOfConsummateSkill.Description";

    public static void Configure()
    {
        FeatureConfigurator.New(FeatName, FeatGuid, FeatureGroup.MythicAbility)
            .SetDisplayName(DisplayName)
            .SetDescription(Description)
            .AddFeatureTagsComponent(FeatureTag.Magic)
            .AddFeatureTagsComponent(FeatureTag.Skills)
            .Configure();
    }
}