using System;
using System.Collections.Generic;
using System.Linq;
using CraftMaster.Reference;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Items;
using Kingmaker.Utility;
using Newtonsoft.Json;

namespace CraftMaster.Builder;

public abstract class EquipBuilder : DynamicBuilder
{
    protected EquipBuilder()
    {
        
    }

    protected EquipBuilder(ItemEntity equip)
    {
        RawItemEntity = equip;
    }

    /// <summary>
    /// 武器名称
    /// </summary>
    [JsonProperty]
    public string EquipName { get; set; } = string.Empty;
    
    /// <summary>
    /// 附魔
    /// </summary>
    [JsonProperty]
    public Dictionary<string, List<string>> EnchantmentGroups { get; set; } = new();
    
    /// <summary>
    /// 是否有原型
    /// </summary>
    [JsonProperty]
    public bool HasPrototype { get; set; }
    
    /// <summary>
    /// 原型Guid
    /// </summary>
    [JsonProperty]
    public string CustomPrototypeGuid { get; set; } = string.Empty;
    
    /// <summary>
    /// 装备类型
    /// </summary>
    [JsonProperty]
    public string EquipType { get; set; } = string.Empty;
    
    /// <summary>
    /// 材质类型
    /// </summary>
    [JsonProperty]
    public string MaterialType { get; set; } = string.Empty;
    
    /// <summary>
    /// 原附魔 - 无法被移除
    /// 用于计算原型附魔加值
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, List<string>> RawEnchantmentGroups { get; set; } = new();

    /// <summary>
    /// 其余附魔
    /// </summary>
    [JsonIgnore]
    public List<ItemEnchantment> OtherEnchantment { get; protected set; } = new();
    
    /// <summary>
    /// 武器基础描述多语言Key
    /// </summary>
    [JsonProperty]
    public string RawDescriptionKey { get; set; } = string.Empty;

    /// <summary>
    /// 武器额外描述
    /// </summary>
    [JsonProperty]
    public string AdditionDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// 原型增强加值
    /// </summary>
    [JsonIgnore]
    public int PrototypeEnhancement { get; protected set; }

    /// <summary>
    /// 实际基础加值
    /// </summary>
    [JsonProperty]
    public int ActuallyEnhancement { get; protected set; }

    /// <summary>
    /// 构造器指向的原始物品数据
    /// </summary>
    [JsonIgnore]
    public ItemEntity RawItemEntity { get; set; }
    
    /// <summary>
    /// 原型附魔加值
    /// </summary>
    [JsonIgnore]
    public int RawEnchantmentPoint { get; protected set; }
    
    /// <summary>
    /// 总附魔加值
    /// </summary>
    [JsonIgnore]
    public int TotalEnchantmentPoint { get; protected set; }

    /// <summary>
    /// 是否已经构建
    /// </summary>
    [JsonIgnore]
    public bool IsBuilt { get; set; }
    
    /// <summary>
    /// 强化数据 索引
    /// </summary>
    [JsonIgnore]
    public abstract IEnchantmentReference EnchantmentReference { get; }

    protected virtual void OnRefresh()
    {
    }
    
    public abstract int GetEnchantmentCost();
    public abstract int GetRawEnchantmentCost();

    public void Refresh()
    {
        OnRefresh();
    }
    
    public virtual void RemoveEnchantmentGroup(string groupKey)
    {
        if (EnchantmentGroups.ContainsKey(groupKey))
        {
            EnchantmentGroups.Remove(groupKey);
        }

        Refresh();
    }

    public virtual void AddEnchantment(EnchantmentData enchantmentData, bool refresh = true)
    {
        if (enchantmentData == null)
        {
            Main.Logger.Warning("EnchantmentData is null");
            return;
        }
        
        if (!EnchantmentGroups.ContainsKey(enchantmentData.Group.Key))
        {
            EnchantmentGroups.Add(enchantmentData.Group.Key, new List<string>());
        }

        EnchantmentGroups[enchantmentData.Group.Key].Add(enchantmentData.Key);

        if(refresh)
            Refresh();
    }
    
    public virtual void AddEnchantmentRaw(EnchantmentData enchantmentData, bool refresh = true)
    {
        if (!RawEnchantmentGroups.ContainsKey(enchantmentData.Group.Key))
        {
            RawEnchantmentGroups.Add(enchantmentData.Group.Key, new List<string>());
        }

        RawEnchantmentGroups[enchantmentData.Group.Key].Add(enchantmentData.Key);

        if(refresh)
            Refresh();
    }

    /// <summary>
    /// 附魔组是否包含附魔
    /// </summary>
    /// <param name="enchantmentDataKey"></param>
    /// <returns></returns>
    public virtual bool ContainsEnchantment(string enchantmentDataKey)
    {
        return RawEnchantmentGroups.Values.Any(list => list.Contains(enchantmentDataKey))
               || EnchantmentGroups.Values.Any(list => list.Contains(enchantmentDataKey));
    }

    public virtual void RemoveEnchantment(string enchantmentKey)
    {
        foreach (var pair in EnchantmentGroups)
        {
            var group = pair.Value;
            if (group.Remove(enchantmentKey))
            {
                if (group.Count == 0)
                    EnchantmentGroups.Remove(pair.Key);
                break;
            }
        }

        Refresh();
    }

    public override void OnValidate()
    {
        if(string.IsNullOrEmpty(EquipName))
            EquipName = "";
        if(string.IsNullOrEmpty(AdditionDescription))
            AdditionDescription = "";
        if(string.IsNullOrEmpty(RawDescriptionKey))
            RawDescriptionKey = "";
        if(string.IsNullOrEmpty(MaterialType))
            MaterialType = "Normal";
    }
    
    public override bool IsValid()
    {
        return !string.IsNullOrEmpty(EquipName) && 
               !string.IsNullOrEmpty(EquipType) && 
               !string.IsNullOrEmpty(MaterialType);
    }
    
    /// <summary>
    /// 读取原始附魔
    /// </summary>
    public void ReadRawEnchantment()
    {
        var enchantments = RawItemEntity.EnchantmentsCollection?.Enumerable;
        if(enchantments == null)
            return;
        foreach (var enchantment in enchantments)
        {
            var findEnchantmentData = EnchantmentReference.GetEnchantmentByGuid(enchantment.Blueprint.AssetGuid);
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
                addEnhancement += EnchantmentReference.GetEnchantment(pair.Key ,enchantment)?.Point ?? 0;
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
            outEnchantment = enchantments.Select(key => EnchantmentReference.GetEnchantment(enhancementGroup, key));
            return true;
        }

        outEnchantment = null;
        return false;
    }

    public virtual string GetPrototypeGuid()
    {
        if(!HasPrototype)
            return EnchantmentReference.FindPrototype(EquipType);
        
        if(string.IsNullOrEmpty(EquipType))
            return CustomPrototypeGuid;

        Main.Logger.Error("GetPrototypeGuid Error");
        return null;
    }
    
    public override int GetCheckDC()
    {
        return 10;
    }

    public override int GetBuildPoint()
    {
        return 10;
    }
}