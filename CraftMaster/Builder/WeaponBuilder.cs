using System;
using System.Collections.Generic;
using System.Linq;
using BlueprintCore.Utils;
using CraftMaster.Reference;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;
using Newtonsoft.Json;

namespace CraftMaster.Builder;

public class WeaponBuilder : EquipBuilder
{
    public WeaponBuilder() : base()
    {
        
    }
    
    public WeaponBuilder(ItemEntityWeapon weapon) : base(weapon)
    {
        HasPrototype = true;
        MaterialType = "";
        CustomPrototypeGuid = weapon.Blueprint.AssetGuid.ToString();
        EquipName = weapon.Blueprint.Name;
        WeaponRawDescriptionKey = weapon.Blueprint.m_DescriptionText.Key;
        WeaponAdditionDescription = "";
        ReadRawEnchantment();
        Refresh();
    }

    /// <summary>
    /// 武器基础描述多语言Key
    /// </summary>
    [JsonProperty]
    public string WeaponRawDescriptionKey { get; set; } = string.Empty;

    /// <summary>
    /// 武器额外描述
    /// </summary>
    [JsonProperty]
    public string WeaponAdditionDescription { get; set; } = string.Empty;

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
    /// 读取原始附魔
    /// </summary>
    private void ReadRawEnchantment()
    {
        var enchantments = ItemEntity.EnchantmentsCollection?.Enumerable;
        if(enchantments == null)
            return;
        foreach (var enchantment in enchantments)
        {
            var findEnchantmentData = ReferenceManager.Weapon.GetEnchantmentByGuid(enchantment.Blueprint.AssetGuid);
            if(findEnchantmentData == null)
            {
                if(!enchantment.Blueprint.HiddenInUI && !string.IsNullOrEmpty(enchantment.Blueprint.Name))
                    OtherEnchantment.Add(enchantment);
                continue;
            }
            
            AddEnchantmentRaw(findEnchantmentData);
        }
    }
    
    /// <summary>
    /// 计算一个附魔组里的附魔加值
    /// </summary>
    /// <param name="enchantmentGroup"></param>
    /// <returns></returns>
    public int CountEnchantmentGroupPoint(Dictionary<string, List<string>> enchantmentGroup)
    {
        var addEnhancement = 0;
        foreach (var pair in enchantmentGroup)
        {
            if(pair.Key == "Enhancement")
                continue;

            foreach (var enchantment in pair.Value)
            {
                addEnhancement += ReferenceManager.Weapon.GetEnchantment(pair.Key ,enchantment)?.Point ?? 0;
            }
        }

        return addEnhancement;
    }

    /// <summary>
    /// 尝试获取附魔组中的附魔
    /// </summary>
    /// <param name="enhancementGroup"></param>
    /// <param name="outEnchantment"></param>
    /// <returns></returns>
    public bool TryGetEnchantmentInGroup(string enhancementGroup, out IEnumerable<EnchantmentData> outEnchantment)
    {
        if(EnchantmentGroups.TryGetValue(enhancementGroup, out var enchantments) && enchantments.Count > 0)
        {
            outEnchantment = enchantments.Select(key => ReferenceManager.Weapon.GetEnchantment(enhancementGroup, key));
            return true;
        }

        outEnchantment = null;
        return false;
    }

    public override void OnValidate()
    {
        if(string.IsNullOrEmpty(EquipName))
            EquipName = "";
        if(string.IsNullOrEmpty(WeaponAdditionDescription))
            WeaponAdditionDescription = "";
        if(string.IsNullOrEmpty(WeaponRawDescriptionKey))
            WeaponRawDescriptionKey = "";
        if(string.IsNullOrEmpty(MaterialType))
            MaterialType = "Normal";
        
        Refresh();
    }

    public override bool IsValid()
    {
        return !string.IsNullOrEmpty(EquipName) && 
               !string.IsNullOrEmpty(EquipType) && 
               !string.IsNullOrEmpty(MaterialType);
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

    public WeaponBuilder Copy()
    {
        var json = JsonConvert.SerializeObject(this);
        var newBuilder = JsonConvert.DeserializeObject<WeaponBuilder>(json);
        newBuilder.Guid = System.Guid.NewGuid().ToString("N");
        newBuilder.Refresh();
        return newBuilder;
    }

    public virtual int GetTotalCost()
    {
        return GetRawCost() + GetEnchantmentCost();
    }

    public override string GetPrototypeGuid()
    {
        if(!HasPrototype)
            return ReferenceManager.Weapon.FindWeaponPrototype(EquipType);
        
        return base.GetPrototypeGuid();
    }
}