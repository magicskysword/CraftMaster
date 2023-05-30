using System;
using BlueprintCore.Utils;
using CraftMaster.Reference;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Weapons;

namespace CraftMaster.Builder;

public class ArmorBuilder : EquipBuilder
{
    public override bool IsValid()
    {
        return !string.IsNullOrEmpty(EquipName) && 
               !string.IsNullOrEmpty(EquipType) && 
               !string.IsNullOrEmpty(MaterialType);
    }

    public override void OnValidate()
    {
        if(string.IsNullOrEmpty(EquipName))
            EquipName = "";
        if(string.IsNullOrEmpty(MaterialType))
            MaterialType = "Normal";
        
        Refresh();
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
            return ReferenceManager.Armor.FindArmorPrototype(EquipType);
        
        return base.GetPrototypeGuid();
    }
}