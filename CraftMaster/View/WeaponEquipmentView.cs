using System.Collections.Generic;
using BlueprintCore.Utils;
using CraftMaster.Builder;
using CraftMaster.Project;
using CraftMaster.Reference;
using Kingmaker.Blueprints.Items.Weapons;

namespace CraftMaster.View;

public class WeaponEquipmentView : NormalEquipmentView
{
    public WeaponBuilder WeaponBuilder => (WeaponBuilder)Builder;

    public EquipTypeView SimpleWeaponTypeView { get; } = new("CraftMaster.UI.SimpleWeapon", 
        ReferenceManager.Weapon.SimpleWeaponTypes);
    
    public EquipTypeView MartialWeaponTypeView { get; } = new("CraftMaster.UI.MartialWeapon",
        ReferenceManager.Weapon.MartialWeaponTypes);
    
    public EquipTypeView ExoticWeaponTypeView { get; } = new("CraftMaster.UI.ExoticWeapon",
        ReferenceManager.Weapon.ExoticWeaponTypes);

    public override IEnumerable<EquipTypeView> EquipTypeEntry
    {
        get
        {
            yield return SimpleWeaponTypeView;
            yield return MartialWeaponTypeView;
            yield return ExoticWeaponTypeView;
        }
    }

    public override IEnumerable<KeyValuePair<string, string[]>> ReferenceMaterials => ReferenceManager.Weapon.Materials;

    public override string GetTypeNameByGuid(string guid)
    {
        var weaponBlueprint = BlueprintTool.Get<BlueprintItemWeapon>(guid);
        return GameUtils.GetString(weaponBlueprint.Type.TypeName.Key);
    }

    public override void ResetBuilder()
    {
        Builder = new WeaponBuilder();
    }

    public override void AddBuildProject(CraftPart craftPart, int buildPoint, int spendMoney, int checkDC)
    {
        craftPart.AddWeaponBuildProject(WeaponBuilder, buildPoint, spendMoney, checkDC);
    }

    public override string GetTooltipByGuid(string guid)
    {
        var weaponBlueprint = BlueprintTool.Get<BlueprintItemWeapon>(guid);
        return GameUtils.GetString("CraftMaster.UI.WeaponTooltip(name,type,damage,critical,range,weight,price)").Format(
                GameUtils.GetString(weaponBlueprint.Type.TypeName.Key),
                GameUtils.GetWeaponUsageName(weaponBlueprint),
                weaponBlueprint.BaseDamage.ToString(),
                GameUtils.GetCriticalDescription(weaponBlueprint.CriticalModifier, weaponBlueprint.CriticalRollEdge),
                weaponBlueprint.Type.AttackRange.ToString(),
                weaponBlueprint.Weight,
                weaponBlueprint.SellPrice
                );
    }
}