using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlueprintCore.Blueprints.Configurators.Items.Equipment;
using BlueprintCore.Blueprints.Configurators.Items.Weapons;
using BlueprintCore.Blueprints.CustomConfigurators;
using BlueprintCore.Utils;
using CraftMaster.Reference;
using CraftMaster.View;
using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using ModKit.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace CraftMaster.Builder;

public static class DynamicBuilderManager
{
    public static BuilderPart GetBuilderPart()
    {
        return Game.Instance?.Player?.Ensure<BuilderPart>();
    }

    /// <summary>
    /// 添加新的武器
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="materialsReference"></param>
    /// <param name="enchantmentReference"></param>
    /// <param name="enhancementReference"></param>
    public static ItemEntity CreateEquip(EquipBuilder builder,
        IMaterialsReference materialsReference = null,
        IEnchantmentReference enchantmentReference = null, 
        IEnhancementReference enhancementReference = null)
    {
        // 校验数据
        builder.OnValidate();
        builder.Refresh();
        
        // 移除原始附魔组，移动到新的附魔组
        foreach (var pair in builder.RawEnchantmentGroups)
        {
            foreach (var data in pair.Value)
            {
                builder.AddEnchantment(enchantmentReference.GetEnchantment(pair.Key, data));
            }
        }
        builder.RawEnchantmentGroups.Clear();
        
        var prototypeGuid = builder.GetPrototypeGuid();
        
        ItemEntity entity;

        switch (builder)
        {
            case WeaponBuilder:
            {
                var blueprint = BlueprintTool.Get<BlueprintItemWeapon>(prototypeGuid);
                entity = new ItemEntityWeapon(blueprint);
                break;
            }
            case ArmorBuilder:
            {
                var blueprint = BlueprintTool.Get<BlueprintItemArmor>(prototypeGuid);
                entity = new ItemEntityArmor(blueprint);
                break;
            }
            default:
            {
                Main.Logger.Error($"unknown builder type: {builder.GetType()}");
                return null;
            }
        }

        // 移除原型增强附魔
        var enchants = entity.Enchantments.ToList();
        foreach (var enchant in enchants)
        {
            if (enchant.Blueprint.IsEnhancement())
            {
                entity.RemoveEnchantment(enchant);
            }
        }
        
        // 添加实际增强附魔
        if (enhancementReference != null)
        {
            if(builder.ActuallyEnhancement > 0)
            {
                var enhancement = enhancementReference.GetEnhancement(builder.ActuallyEnhancement);
                entity.AddEnchantment(enhancement, new MechanicsContext(default));
            }
        }
        
        // 添加材质
        var materialGuids = materialsReference.FindMaterial(builder.MaterialType);
        foreach (var materialGuid in materialGuids)
        {
            entity.AddEnchantment(materialGuid.GetBlueprint<BlueprintItemEnchantment>(), new MechanicsContext(default));
        }
        
        // 添加附魔
        foreach (var pair in builder.EnchantmentGroups)
        {
            var groupKey = pair.Key;
            if(groupKey == "Enhancement")
                continue;
            foreach (var enchantmentKey in pair.Value)
            {
                var enchantmentData = enchantmentReference.GetEnchantment(groupKey, enchantmentKey);
                if (enchantmentData == null)
                {
                    Main.Logger.Error($"enchantment not found: {groupKey} {enchantmentKey}");
                    continue;
                }
                
                var enchantmentGuid = enchantmentData.Guid;
                entity.TryAddEnchantment(enchantmentGuid.GetBlueprint<BlueprintItemEnchantment>());
                if(enchantmentData.ContainerEnchantments != null && enchantmentData.ContainerEnchantments.Length > 0)
                {
                    foreach (var containerEnchantment in enchantmentData.ContainerEnchantments)
                    {
                        entity.TryAddEnchantment(containerEnchantment.GetBlueprint<BlueprintItemEnchantment>());
                    }
                }
            }
        }
        
        var builderPart = GetBuilderPart();
        builderPart.AddMapping(entity.UniqueId, builder.Guid);
        entity.IsIdentified = false;
        return entity;
    }

    /// <summary>
    /// 添加新的法杖
    /// </summary>
    /// <param name="wandBuilder"></param>
    /// <returns></returns>
    public static ItemEntity CreateWand(WandBuilder wandBuilder)
    {
        wandBuilder.OnValidate();
        
        var blueprint = BlueprintTool.Get<BlueprintItemEquipmentUsable>(wandBuilder.PropertyGuid);
        var entity = new ItemEntityUsable(blueprint);
        entity.Charges = 50;

        var builderPart = GetBuilderPart();
        builderPart.AddMapping(entity.UniqueId, wandBuilder.Guid);

        return entity;
    }

    public static BlueprintItemEquipmentUsable GetWandBlueprint(WandBuilder wandBuilder)
    {
        if (BlueprintTool.TryGet<BlueprintItemEquipmentUsable>(wandBuilder.Guid, out var blueprint))
        {
            return blueprint;
        }

        var baseBlueprint = BlueprintTool.Get<BlueprintItemEquipmentUsable>(wandBuilder.PropertyGuid);
        var blueprintConfigurator = ItemEquipmentUsableConfigurator.New(wandBuilder.Guid, wandBuilder.Guid);
        blueprintConfigurator.CopyFrom(baseBlueprint);
        blueprint = blueprintConfigurator.Configure();
        var abilityBlueprint = BlueprintTool.Get<BlueprintAbility>(wandBuilder.TagAbility);
        blueprint.m_Ability = abilityBlueprint.ToReference<BlueprintAbilityReference>();
        blueprint.DC = wandBuilder.DC;
        blueprint.CR = wandBuilder.CasterLevel;
        blueprint.SpellLevel = wandBuilder.SpellLevel;
        blueprint.CasterLevel = wandBuilder.CasterLevel;
        blueprint.Charges = 50;

        return blueprint;
    }

    public static void AddWeaponBuilder(WeaponBuilder builder)
    {
        if (builder.IsBuilt)
        {
            Main.Logger.Error($"duplicate build weapon: {builder.Guid} data:{JsonConvert.SerializeObject(builder)}");
            return;
        }
        
        builder.IsBuilt = true;
        GetBuilderPart().AddBuilder(builder);
    }
    
    public static void AddArmorBuilder(ArmorBuilder armorBuilder)
    {
        if (armorBuilder.IsBuilt)
        {
            Main.Logger.Error($"duplicate build armor: {armorBuilder.Guid} data:{JsonConvert.SerializeObject(armorBuilder)}");
            return;
        }
        
        armorBuilder.IsBuilt = true;
        GetBuilderPart().AddBuilder(armorBuilder);
    }
    
    public static WandBuilder AddAndGetShardWandBuilder(WandBuilder wandBuilder)
    {
        var tagBuilder = GetBuilderPart().Builders.Values.OfType<WandBuilder>().FirstOrDefault(builder =>
        {
            return builder.TagAbility == wandBuilder.TagAbility
                   && builder.SpellLevel == wandBuilder.SpellLevel
                   && builder.CasterLevel == wandBuilder.CasterLevel
                   && builder.DC == wandBuilder.DC;
        });

        if (tagBuilder == null)
        {
            GetBuilderPart().AddBuilder(wandBuilder);
            tagBuilder = wandBuilder;
        }

        return tagBuilder;
    }

    public static WeaponBuilder GetWeaponNewBuilder(ItemEntityWeapon weapon)
    {
        if(GetBuilderPart().TryGetMappingBuilder<WeaponBuilder>(weapon.UniqueId, out var builder))
        {
            return builder.CopyNew();
        }
        
        return new WeaponBuilder(weapon);
    }
    
    public static ArmorBuilder GetArmorNewBuilder(ItemEntityArmor armor)
    {
        if(GetBuilderPart().TryGetMappingBuilder<ArmorBuilder>(armor.UniqueId, out var builder))
        {
            return builder.CopyNew();
        }
        
        return new ArmorBuilder(armor);
    }
    
    public static WandBuilder GetWandNewBuilder(BlueprintAbility spell, int spellLevel)
    {
        return new WandBuilder()
        {
            TagAbility = spell.AssetGuid.ToString(),
            SpellLevel = spellLevel,
        };
    }

    public static void CanAddEnchantment(int curPoint, UnitEntityData unit,EquipBuilder builder, EnchantmentGroup group,
        EnchantmentData enchantmentData, ref CheckerContext checker)
    {
        if (group.Key == "Enhancement")
        {
            if (enchantmentData.Point <= builder.PrototypeEnhancement)
            {
                checker.IsEnable = false;
            }
            
            if (enchantmentData.Point == builder.ActuallyEnhancement)
            {
                checker.IsEnable = false;
            }
        }
        
        if(builder.ContainsEnchantment(enchantmentData.Key))
        {
            checker.IsEnable = false;
        }

        if (enchantmentData.RequiredEnchantment.HasContent())
        {
            var requireTooltips = new List<string>();
            var meetRequire = true;
            requireTooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.EnchantmentRequire"));
            foreach (var require in enchantmentData.RequiredEnchantment)
            {
                var requireEnchantment = ReferenceManager.Weapon.GetEnchantmentByKey(require);
                if(requireEnchantment == null)
                {
                    Main.Logger.Error($"enchantment not found: {require}");
                    continue;
                }
                
                var blueprint = requireEnchantment.Guid.GetBlueprint<BlueprintItemEnchantment>();
                if (!builder.ContainsEnchantment(require))
                {
                    meetRequire = false;
                    requireTooltips.Add($"\t{GameUtils.GetString("CraftMaster.EnchantmentGroup.EnchantmentRequire(name)")
                        .Format(blueprint.Name).Red()}");
                }
                else
                {
                    requireTooltips.Add($"\t{GameUtils.GetString("CraftMaster.EnchantmentGroup.EnchantmentRequire(name)")
                        .Format(blueprint.Name).Green()}");
                }
            }

            if (!meetRequire)
            {
                checker.IsEnable = false;
                requireTooltips[0] = requireTooltips[0].Red();
            }
            else
            {
                requireTooltips[0] = requireTooltips[0].Green();
            }
            checker.Tooltips.AddRange(requireTooltips);
        }
        
        if (builder.ActuallyEnhancement == 0 && group.Key != "Enhancement")
        {
            checker.IsEnable = false;
            checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.EnchantmentPointNotEnough").Red());
        }

        if(curPoint + enchantmentData.Point > 10)
        {
            checker.IsEnable = false;
            checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.EnchantmentPointOutLimit").Red());
        }
        
        if(unit.GetCasterLevel() < enchantmentData.CasterLevel)
        {
            checker.IsEnable = false;
            checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.CasterLevelNotEnough(require,current)")
                .Format(enchantmentData.CasterLevel, unit.GetCasterLevel()).Red());
        }
    }

    public static void AddEnchantment(EquipBuilder builder,EnchantmentGroup group, EnchantmentData enchantmentData)
    {
        if (!group.AllowMultiple)
        {
            builder.RemoveEnchantmentGroup(group.Key);
        }
        
        builder.AddEnchantment(enchantmentData);
    }

    public static void RemoveEnchantment(EquipBuilder builder, string enchantmentKey)
    {
        builder.RemoveEnchantment(enchantmentKey);
    }
}