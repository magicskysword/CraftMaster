using System.Collections.Generic;
using BlueprintCore.Utils;
using CraftMaster.Builder;
using CraftMaster.Project;
using CraftMaster.Reference;
using Kingmaker.Blueprints.Items.Armors;

namespace CraftMaster.View;

public class ArmorEquipmentView : NormalEquipmentView
{
    public ArmorBuilder ArmorBuilder => (ArmorBuilder)Builder;
    
    public EquipTypeView LightArmorTypeView { get; } = new("CraftMaster.UI.LightArmor",
        ReferenceManager.Armor.LightArmorTypes);
    
    public EquipTypeView MiddleArmorTypeView { get; } = new("CraftMaster.UI.MiddleArmor",
        ReferenceManager.Armor.MiddleArmorTypes);
    
    public EquipTypeView HeavyArmorTypeView { get; } = new("CraftMaster.UI.HeavyArmor",
        ReferenceManager.Armor.HeavyArmorTypes);
    
    public EquipTypeView BardingArmorTypesView { get; } = new("CraftMaster.UI.BardingArmor",
        ReferenceManager.Armor.BardingArmorTypes);
    
    public override IEnumerable<EquipTypeView> EquipTypeEntry
    {
        get
        {
            yield return LightArmorTypeView;
            yield return MiddleArmorTypeView;
            yield return HeavyArmorTypeView;
            yield return BardingArmorTypesView;
        }
    }

    public override IEnumerable<KeyValuePair<string, string[]>> ReferenceMaterials => ReferenceManager.Armor.Materials;

    public override string GetTypeNameByGuid(string guid)
    {
        var weaponBlueprint = BlueprintTool.Get<BlueprintItemArmor>(guid);
        return GameUtils.GetString(weaponBlueprint.Type.TypeName.Key);
    }

    public override void ResetBuilder()
    {
        Builder = new ArmorBuilder();
    }

    public override void AddBuildProject(CraftPart craftPart, int buildPoint, int spendMoney, int checkDC)
    {
        craftPart.AddArmorBuildProject(ArmorBuilder, buildPoint, spendMoney, checkDC);
    }

    public override string GetTooltipByGuid(string guid)
    {
        var armorBlueprint = BlueprintTool.Get<BlueprintItemArmor>(guid);
        return GameUtils.GetString("CraftMaster.UI.ArmorTooltip(name,type,ac,weight,price)").Format(
            GameUtils.GetString(armorBlueprint.Type.TypeName.Key),
            GameUtils.GetArmorUsageName(armorBlueprint),
            armorBlueprint.ArmorBonus,
            armorBlueprint.Weight,
            armorBlueprint.SellPrice);
    }
}