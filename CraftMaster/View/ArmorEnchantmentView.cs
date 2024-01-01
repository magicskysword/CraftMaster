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

public class ArmorEnchantmentView : EnchantmentView
{
    public ArmorBuilder ArmorBuilder => Builder as ArmorBuilder;
    public override Func<ItemEntity, bool> Filter => ArmorFilter;
    public override IEnumerable<EnchantmentGroup> AllEnchantmentGroups =>
        ReferenceManager.Armor.GetAllEnchantmentGroups();
    
    public override CheckerContext CheckAddEnchantment(UnitEntityData unit,
        EnchantmentGroup group, 
        EnchantmentData enchantmentData)
    {
        var builder = ArmorBuilder;
        var curPoint = builder.TotalEnchantmentPoint;
        var blueprint = BlueprintTool.Get<BlueprintArmorEnchantment>(enchantmentData.Guid);
        var checker = new CheckerContext();
        checker.Text = blueprint.Name;
        checker.Description = blueprint.Description;
        
        DynamicBuilderManager.CanAddEnchantment(curPoint,unit, ArmorBuilder, group, enchantmentData, ref checker);
        if (enchantmentData.AddApplyChecker != null)
        {
            enchantmentData.AddApplyChecker(Item, unit, ref checker);
        }

        return checker;
    }

    public override EquipBuilder GetNewBuilder(ItemEntity newItem)
    {
        return DynamicBuilderManager.GetArmorNewBuilder(newItem as ItemEntityArmor);
    }

    public override EnchantmentGroup GetEnchantmentGroup(string groupKey)
    {
        return ReferenceManager.Armor.GetEnchantmentGroup(groupKey);
    }

    public override EnchantmentData GetEnchantment(string groupKey, string enchantmentKey)
    {
        return ReferenceManager.Armor.GetEnchantment(groupKey, enchantmentKey);
    }

    public override void CreateProject(CraftPart craftPart, int buildPoint, int spendMoney, int checkDC)
    {
        craftPart.AddArmorEnchantmentProject((ItemEntityArmor)Item, ArmorBuilder, buildPoint, spendMoney, checkDC);
    }
    
    private static bool ArmorFilter(ItemEntity item)
    {
        if (!(item is ItemEntityArmor armor))
        {
            return false;
        }
            
        if (armor.IsPartOfAnotherItem)
            return false;
        
        if (armor.Blueprint.IsShield)
            return false;

        if (armor.Blueprint.IsNotable)
            return false;
            
        return true;
    }
}