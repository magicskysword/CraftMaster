using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Designers.EventConditionActionSystem.Conditions;

namespace CraftMaster.Feats
{
    /// <summary>
    /// Creates a feat that does nothing but show up.
    /// </summary>
    public class CraftMagicArmsAndArmor
    {
        public static readonly string FeatName = "CraftMagicArmsAndArmor";
        public static readonly string FeatGuid = "C3B2FD41-4CAC-4F70-ABF9-8F6AD8962478";

        private static readonly string DisplayName = "CraftMagicArmsAndArmor.Name";
        private static readonly string Description = "CraftMagicArmsAndArmor.Description";

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