using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlueprintCore.Utils;
using CraftMaster.Reference;
using Kingmaker;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;
using Newtonsoft.Json;

namespace CraftMaster.Builder;

public class WeaponBuilder : EquipBuilder
{
    [JsonIgnore]
    public override IEnchantmentReference EnchantmentReference => ReferenceManager.Weapon;
    
    public WeaponBuilder() : base()
    {
        
    }
    
    public WeaponBuilder(ItemEntityWeapon weapon) : base(weapon)
    {
        HasPrototype = true;
        MaterialType = "";
        CustomPrototypeGuid = weapon.Blueprint.AssetGuid.ToString();
        EquipName = weapon.Blueprint.Name;
        RawDescriptionKey = weapon.Blueprint.m_DescriptionText.Key;
        AdditionDescription = "";
        ReadRawEnchantment();
        Refresh();
    }

    protected override void OnRefresh()
    {
        // 原型增强加值
        var value = 0;
        if(HasPrototype)
        {
            var blueprint = BlueprintTool.Get<BlueprintItemWeapon>(CustomPrototypeGuid);
            foreach (var enchantment in blueprint.Enchantments.OfType<BlueprintWeaponEnchantment>())
            {
                foreach (var bonus in enchantment.Components.OfType<WeaponEnhancementBonus>())
                {
                    if(bonus.EnhancementBonus > value)
                        value = bonus.EnhancementBonus;
                }
            }
        }
        PrototypeEnhancement = value;
        
        // 当前增强加值
        value = 0;
        if(TryGetEnchantmentInGroup("Enhancement", out var enchantments))
        {
            value = enchantments.Max(enchantment => enchantment.Point);
        }
        ActuallyEnhancement = Math.Max(value, PrototypeEnhancement);
        
        // 原始附魔加值
        var rawEnchantment = CountEnchantmentGroupPoint(RawEnchantmentGroups);
        RawEnchantmentPoint = rawEnchantment + PrototypeEnhancement;
    
        // 新增附魔加值
        var baseEnhancement = ActuallyEnhancement;
        var addEnchantment = CountEnchantmentGroupPoint(EnchantmentGroups);
        
        // 总附魔加值
        TotalEnchantmentPoint = baseEnhancement + rawEnchantment + addEnchantment;
    }

    /// <summary>
    /// 获取原始制造点数
    /// </summary>
    /// <returns></returns>
    public override int GetRawCost()
    {
        var prototypeGuid = GetPrototypeGuid();
        if(string.IsNullOrEmpty(prototypeGuid))
            return 0;
        
        var prototype = BlueprintTool.Get<BlueprintItem>(prototypeGuid);
        var cost = 100;
        switch (MaterialType)
        {
            case "Masterwork":
                cost *= 4;
                break;
            case "ColdIron":
                cost *= 8;
                break;
            case "Mithral":
                cost *= 4;
                cost += (int)(prototype.Weight * 100);
                break;
            case "Adamantine":
                cost *= 4;
                cost += 3000;
                break;
        }

        return cost;
    }

    public override int GetCheckDC()
    {
        if(TotalEnchantmentPoint == 0)
            return base.GetCheckDC();
        
        return 20 + TotalEnchantmentPoint * 5;
    }

    public override int GetBuildPoint()
    {
        return base.GetBuildPoint() + (int)(Math.Pow(TotalEnchantmentPoint, 2) - Math.Pow(RawEnchantmentPoint, 2)) * 10;
    }

    public override int GetEnchantmentCost()
    {
        var enchantment = TotalEnchantmentPoint;
        var cost = Math.Pow(enchantment, 2) * 2000;
        
        return (int)cost;
    }
    
    public override int GetRawEnchantmentCost()
    {
        var enchantment = RawEnchantmentPoint;
        var cost = Math.Pow(enchantment, 2) * 2000;
        
        return (int)cost;
    }

    public WeaponBuilder CopyNew()
    {
        var newBuilder = CopyNew(this);
        newBuilder.Guid = System.Guid.NewGuid().ToString("N");
        newBuilder.Refresh();
        return newBuilder;
    }

    public virtual int GetTotalCost()
    {
        return GetRawCost() + GetEnchantmentCost();
    }
}