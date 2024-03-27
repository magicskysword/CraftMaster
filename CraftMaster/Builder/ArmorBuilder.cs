using System;
using System.Linq;
using BlueprintCore.Utils;
using CraftMaster.Reference;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Items;
using Newtonsoft.Json;

namespace CraftMaster.Builder;

public class ArmorBuilder : EquipBuilder
{
    [JsonIgnore]
    public override IEnchantmentReference EnchantmentReference => ReferenceManager.Armor;
    
    public ArmorBuilder()
    {
        
    }
    
    public ArmorBuilder(ItemEntityArmor weapon) : base(weapon)
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
            var blueprint = BlueprintTool.Get<BlueprintItemArmor>(CustomPrototypeGuid);
            foreach (var enchantment in blueprint.Enchantments.OfType<BlueprintArmorEnchantment>())
            {
                foreach (var bonus in enchantment.Components.OfType<ArmorEnhancementBonus>())
                {
                    if(bonus.EnhancementValue > value)
                        value = bonus.EnhancementValue;
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
        
        // 从原形和当前增强加值中取最大值
        ActuallyEnhancement = Math.Max(value, PrototypeEnhancement);
        
        // 原形所带的增强附魔，对应的加值
        var baseEnhancement = ActuallyEnhancement;
        
        // 原形所带的其他附魔，对应的加值
        var rawEnchantment = CountEnchantmentGroupPoint(RawEnchantmentGroups);
        RawEnchantmentPoint = rawEnchantment + PrototypeEnhancement;
    
        // 当前新增的附魔，对应的加值
        var addEnchantment = CountEnchantmentGroupPoint(EnchantmentGroups);
        
        // 总附魔加值
        TotalEnchantmentPoint = baseEnhancement + rawEnchantment + addEnchantment;
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
            // 精制品
            case "Masterwork":
                cost *= 4;
                break;
            // 寒铁
            case "ColdIron":
                cost *= 8;
                break;
            // 秘银
            case "Mithral":
                cost *= 4;
                cost += (int)(prototype.Weight * 100);
                break;
            // 精金
            case "Adamantine":
                cost *= 4;
                cost += 3000;
                break;
        }

        return cost;
    }
    
    public ArmorBuilder CopyNew()
    {
        var newBuilder = CopyNew(this);
        newBuilder.Guid = System.Guid.NewGuid().ToString("N");
        newBuilder.Refresh();
        return newBuilder;
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
    
    public override string GetPrototypeGuid()
    {
        if(!HasPrototype)
            return ReferenceManager.Armor.FindPrototype(EquipType);
        
        return base.GetPrototypeGuid();
    }
}