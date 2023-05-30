using System;
using System.Collections.Generic;
using CraftMaster.Builder;
using CraftMaster.Project;
using CraftMaster.Reference;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;

namespace CraftMaster.View;

public abstract class EnchantmentView : BaseView
{
    public abstract Func<ItemEntity, bool> Filter { get; }
    
    public abstract IEnumerable<EnchantmentGroup> AllEnchantmentGroups { get; }
    public ItemEntity Item { get; set; }
    public EquipBuilder Builder { get; set; }
    public HashSet<string> ExpandGroup { get; set; } = new();

    public abstract CheckerContext CheckAddEnchantment(UnitEntityData unit, EnchantmentGroup group, EnchantmentData enchantmentData);

    public abstract EquipBuilder GetNewBuilder(ItemEntity newItem);

    public bool IsExpanded(string groupKey)
    {
        return ExpandGroup.Contains(groupKey);
    }

    public void SetExpand(string groupKey, bool isExpand)
    {
        if(isExpand)
            ExpandGroup.Add(groupKey);
        else
            ExpandGroup.Remove(groupKey);
    }

    public abstract EnchantmentGroup GetEnchantmentGroup(string groupKey);
    public abstract EnchantmentData GetEnchantment(string groupKey, string enchantmentKey);

    public abstract void CreateProject(CraftPart craftPart, int buildPoint, int spendMoney, int checkDC);

    protected override void OnReset()
    {
        Item = null;
        Builder = null;
        ExpandGroup.Clear();
    }
}