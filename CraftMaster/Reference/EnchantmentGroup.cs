using System;
using System.Collections.Generic;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;

namespace CraftMaster.Reference;

public class EnchantmentGroup
{
    private Dictionary<string, EnchantmentData> _enchantments = new();
    
    public string Key { get; set; }
    public string NameStringKey { get; set; }
    public bool AllowMultiple { get; set; }
    public IReadOnlyDictionary<string, EnchantmentData> Enchantments => _enchantments;

    public void AddEnchantment(EnchantmentData enchantmentData)
    {
        if (enchantmentData.Key == null)
        {
            var blueprint = BlueprintTool.Get<SimpleBlueprint>(enchantmentData.Guid);
            enchantmentData.Key = blueprint.name;
        }
        enchantmentData.Group = this;
        
        _enchantments[enchantmentData.Key] = enchantmentData;
    }
}