using System;
using System.Collections.Generic;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Ecnchantments;
using UnityEngine;

namespace CraftMaster.Reference;

public static class EnchantmentExtension
{
    public static void AddEnchantmentGroup(this IEnchantmentReference reference, EnchantmentGroup group)
    {
        reference.EnchantmentGroups.Add(group.Key, group);
    }

    public static EnchantmentData GetEnchantment(this IEnchantmentReference reference, string groupKey, string dataKey)
    {
        if (reference.EnchantmentGroups.TryGetValue(groupKey, out var group))
        {
            if (group.Enchantments.TryGetValue(dataKey, out var data))
            {
                return data;
            }
        }

        Main.Logger.Warning($"Enchantment not found: {groupKey} - {dataKey}");
        return null;
    }
    
    public static IEnumerable<EnchantmentGroup> GetAllEnchantmentGroups(this IEnchantmentReference reference)
    {
        return reference.EnchantmentGroups.Values;
    }

    public static EnchantmentData GetEnchantmentByKey(this IEnchantmentReference reference, string dataKey)
    {
        return reference.EnchantmentGroups.Values.SelectMany(group => group.Enchantments)
            .FirstOrDefault(data => data.Key.Equals(dataKey, StringComparison.OrdinalIgnoreCase)).Value;
    }

    public static EnchantmentData GetEnchantmentByGuid(this IEnchantmentReference reference,
        BlueprintGuid blueprintAssetGuid)
    {
        return reference.EnchantmentGroups.Values.SelectMany(group => group.Enchantments)
            .FirstOrDefault(data =>
                data.Value.Guid.Equals(blueprintAssetGuid.ToString(), StringComparison.OrdinalIgnoreCase)).Value;
    }

    public static EnchantmentGroup GetEnchantmentGroup(this IEnchantmentReference reference, string groupKey)
    {
        if (reference.EnchantmentGroups.TryGetValue(groupKey, out var group))
        {
            return group;
        }

        return null;
    }
    
    public static BlueprintItemEnchantment GetEnhancement(this IEnhancementReference reference, int enhancement)
    {
        var index = enhancement - 1;
        index = Mathf.Clamp(index, 0, reference.Enhancements.Length - 1);
        return reference.Enhancements[index].GetBlueprint<BlueprintItemEnchantment>();
    }
    
    public static string[] FindMaterial(this IMaterialsReference reference, string materialKey)
    {
        string[] materials;
        if (reference.Materials.TryGetValue(materialKey, out materials))
        {
            return materials;
        }

        Main.Logger.Error($"Weapon material : {materialKey} not found!");
        return null;
    }
}