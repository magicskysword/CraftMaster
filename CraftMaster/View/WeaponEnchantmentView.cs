using System;
using System.Collections.Generic;
using BlueprintCore.Utils;
using CraftMaster.Builder;
using CraftMaster.Project;
using CraftMaster.Reference;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;

namespace CraftMaster.View;

public class WeaponEnchantmentView : EnchantmentView
{
    public WeaponBuilder WeaponBuilder => Builder as WeaponBuilder;
    public override Func<ItemEntity, bool> Filter => WeaponFilter;
    public override IEnumerable<EnchantmentGroup> AllEnchantmentGroups => ReferenceManager.Weapon.GetAllEnchantmentGroups();

    public override CheckerContext CheckAddEnchantment(UnitEntityData unit,EnchantmentGroup group, EnchantmentData enchantmentData)
    {
        var builder = WeaponBuilder;
        var curPoint = builder.TotalEnchantmentPoint;
        var blueprint = BlueprintTool.Get<BlueprintWeaponEnchantment>(enchantmentData.Guid);
        var checker = new CheckerContext();
        checker.Text = blueprint.Name;
        checker.Description = blueprint.Description;
        
        DynamicBuilderManager.CanAddEnchantment(curPoint,unit, WeaponBuilder, group, enchantmentData, ref checker);
        if (enchantmentData.AddApplyChecker != null)
        {
            enchantmentData.AddApplyChecker(Item, unit, ref checker);
        }

        return checker;
    }

    public override EquipBuilder GetNewBuilder(ItemEntity newItem)
    {
        return DynamicBuilderManager.GetWeaponNewBuilder(newItem as ItemEntityWeapon);
    }

    public override EnchantmentGroup GetEnchantmentGroup(string groupKey)
    {
        return ReferenceManager.Weapon.GetEnchantmentGroup(groupKey);
    }

    public override EnchantmentData GetEnchantment(string groupKey, string enchantmentKey)
    {
        return ReferenceManager.Weapon.GetEnchantment(groupKey, enchantmentKey);
    }

    public override void CreateProject(CraftPart craftPart, int buildPoint, int spendMoney, int checkDC)
    {
        craftPart.AddWeaponEnchantmentProject((ItemEntityWeapon)Item, WeaponBuilder, buildPoint, spendMoney, checkDC);
    }

    private static bool WeaponFilter(ItemEntity item)
    {
        if (!(item is ItemEntityWeapon weapon))
        {
            return false;
        }
            
        if (weapon.IsPartOfAnotherItem)
            return false;

        if (weapon.IsShield)
            return false;
            
        if (weapon.IsMonkUnarmedStrike)
            return false;
            
        if (weapon.IsPermanentEmptyHand)
            return false;
            
        if (weapon.Blueprint.IsNatural)
            return false;
            
        if (weapon.Blueprint.IsUnarmed)
            return false;

        if (weapon.Blueprint.IsNotable)
            return false;
            
        return true;
    }
}