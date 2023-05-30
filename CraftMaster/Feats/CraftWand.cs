using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Designers.EventConditionActionSystem.Conditions;

namespace CraftMaster.Feats
{
    public class CraftWand
    {
        private static readonly string FeatName = "CraftWand";
        public static readonly string FeatGuid = "369A3C62-438F-41EC-8D92-123FAEB080FD";

        private static readonly string DisplayName = "CraftWand.Name";
        private static readonly string Description = "CraftWand.Description";

        public static void Configure()
        {
            var prerequisiteCasterLevel = new PrerequisiteCasterLevel().SetCasterLevel(5);

            FeatureConfigurator.New(FeatName, FeatGuid, FeatureGroup.Feat, FeatureGroup.CombatFeat)
                .SetDisplayName(DisplayName)
                .SetDescription(Description)
                .AddComponent(prerequisiteCasterLevel)
                .AddFeatureTagsComponent(FeatureTag.Magic)
                .AddFeatureTagsComponent(FeatureTag.Skills)
                .Configure();
        }
    }
}