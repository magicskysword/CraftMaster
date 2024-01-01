using System;
using System.Collections.Generic;
using System.Linq;
using BlueprintCore.Utils;
using CraftMaster.Project;
using CraftMaster.Reference;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Enums.Damage;
using Kingmaker.Items;
using Kingmaker.Kingdom;
using Kingmaker.Localization;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using UnityEngine;

namespace CraftMaster;

public static class GameUtils
{
    public static string GetString(string stringKey)
    {
        try
        {
            var str = LocalizationManager.CurrentPack?.GetText(stringKey);
            return str;
        }
        catch (Exception e)
        {
            Main.Logger.Error($"Failed to get string {stringKey} {e}");
            return $"Unknown StringKey [{stringKey}]";
        }
    }
    
    public static bool IsPlayerInCapital()
    {
        return (Game.Instance.Player.CapitalPartyMode) 
               || (Game.Instance.CurrentMode == 9 && KingdomTimelineManager.CanAdvanceTime());
    }

    private static DateTime _lastQueryTime;
    private static bool _cachedIsGameStart;
    public static bool IsGameStart()
    {
        if (DateTime.Now - _lastQueryTime > TimeSpan.FromSeconds(1))
        {
            _cachedIsGameStart = Game.Instance.Player?.Party.Any() ?? false;
            _lastQueryTime = DateTime.Now;
        }

        return _cachedIsGameStart;
    }
    
    public static bool IsUnitInParty(this UnitEntityData unit)
    {
        if (IsPlayerInCapital())
        {
            return true;
        }
        
        return Game.Instance.Player.Party.Contains(unit);
    }
    
    public static bool HasFeature(this UnitEntityData unit, string featureGuid)
    {
        var blueprint = featureGuid.GetBlueprint<BlueprintFeature>();
        
        return unit.Descriptor.GetFeature(blueprint) != null;
    }
    
    public static T GetBlueprint<T>(this string guid) where T : BlueprintScriptableObject
    {
        return BlueprintTool.Get<T>(guid);
    }
    
    public static bool HasAbility(this UnitEntityData unit, BlueprintAbility tagAbility)
    {
        var canCast = unit.Spellbooks.Any(s => 
            s.m_KnownSpells.Any(sps => sps.HasAbility(tagAbility)) ||
            s.m_CustomSpells.Any(sps => sps.HasAbility(tagAbility)) ||
            s.m_SpecialSpells.Any(sps => sps.HasAbility(tagAbility)));
        if (canCast)
            return true;

        return unit.Abilities.RawFacts.Any(a => a.Blueprint.AssetGuid == tagAbility.AssetGuid);
    }
    
    public static List<BlueprintAbility> GetCanSpellAbilities(this IEnumerable<AbilityData> abilities)
    {
        var list = new List<BlueprintAbility>();
        foreach (var ability in abilities)
        {
            var variants = ability.AbilityVariants;
            if (variants == null)
            {
                list.Add(ability.Blueprint);
            }
            else
            {
                list.AddRange(variants.Variants);
            }
        }
        
        return list;
    }

    public static int GetMinCasterLevelForUsable(this Spellbook spellbook, BlueprintAbility ability, int spellLevel)
    {
        var minCasterLevel = 1 + 20 / spellbook.MaxSpellLevel * (spellLevel - 1);
        if(minCasterLevel > spellbook.CasterLevel)
        {
            minCasterLevel = spellbook.CasterLevel;
        }
        if(minCasterLevel < 1)
        {
            minCasterLevel = 1;
        }
        
        return minCasterLevel;
    }

    /// <summary>
    /// 尝试添加附魔，如果已经存在则不添加
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="enchantment"></param>
    public static void TryAddEnchantment(this ItemEntity entity, BlueprintItemEnchantment enchantment)
    {
        if(entity.EnchantmentsCollection?.Enumerable.Any(e => e.Blueprint == enchantment) == true)
        {
            return;
        }
        
        entity.AddEnchantment(enchantment, new MechanicsContext(default));
    }
    
    public static bool IsEnhancement(this BlueprintItemEnchantment enchantment)
    {
        return enchantment.Components.Any(c => c is WeaponEnhancementBonus or ArmorEnhancementBonus);
    }

    public static bool HasAbility(this IEnumerable<AbilityData> abilities, BlueprintAbility tagAbility)
    {
        foreach (var ability in abilities)
        {
            if (ability.Blueprint.AssetGuid == tagAbility.AssetGuid)
                return true;
        }

        return false;
    }

    public static int GetCasterLevel(this UnitEntityData unit)
    {
        int casterLevel = 0;
        foreach (Spellbook spellBook in unit.Spellbooks)
        {
            if (casterLevel < spellBook.CasterLevel)
            {
                casterLevel = spellBook.CasterLevel;
            }
        }
        return casterLevel;
    }
    
    public static int GetCasterLevel(this UnitDescriptor unit)
    {
        int casterLevel = 0;
        foreach (Spellbook spellBook in unit.Spellbooks)
        {
            if (casterLevel < spellBook.CasterLevel)
            {
                casterLevel = spellBook.CasterLevel;
            }
        }
        return casterLevel;
    }
    
    public static void ChangeMoney(int money)
    {
        if(money == 0)
            return;
        
        var moneyEntity = OtherReference.CreatMoneyEntity(Math.Abs(money));
        if(money > 0)
        {
            Game.Instance.Player.Inventory.Add(moneyEntity);
        }
        else
        {
            Game.Instance.Player.Inventory.Remove(moneyEntity);
        }
    }

    public static void ShowLogMessage(string message)
    {
        EventBus.RaiseEvent((ILogMessageUIHandler h) => h.HandleLogMessage(message));
    }

    #region BlueprintItemW

    public static string GetWeaponName(this BlueprintItemWeapon weapon)
    {
        return weapon.Name;
    }
    
    public static string GetWeaponTypeName(this BlueprintItemWeapon weapon)
    {
        return GetString(weapon.Type.TypeName.Key);
    }
    
    public static string GetArmorName(this BlueprintItemArmor armor)
    {
        return armor.Name;
    }
    
    public static string GetArmorTypeName(this BlueprintItemArmor armor)
    {
        return GetString(armor.Type.TypeName.Key);
    }

    #endregion

    public static string GetLeftTimeDescription(this TimeSpan leftTime)
    {
        if(leftTime.TotalDays >= 1)
            return GameUtils.GetString("CraftMaster.Project.LeftTime(day)").Format(leftTime.TotalDays.ToString("F0"));
        
        if(leftTime.TotalHours >= 1)
            return GameUtils.GetString("CraftMaster.Project.LeftTime(hour)").Format(leftTime.TotalHours.ToString("F0"));

        return GameUtils.GetString("CraftMaster.Project.Instant");
    }

    public static string GetCriticalDescription(DamageCriticalModifierType criticalModifier, int criticalRollEdge)
    {
        if (criticalRollEdge == 20)
        {
            return $"{criticalRollEdge} ({criticalModifier.ToString()})";
        }
        
        return $"{criticalRollEdge} - 20 ({criticalModifier.ToString()})";
    }

    public static string GetWeaponUsageName(BlueprintItemWeapon weaponBlueprint)
    {
        var tags = new List<string>();
                
        if (weaponBlueprint.IsRanged)
        {
            tags.Add(GameUtils.GetString("CraftMaster.WeaponUsage.Ranged"));
        }
        else
        {
            tags.Add(GameUtils.GetString("CraftMaster.WeaponUsage.Melee"));
        }
        
        if (weaponBlueprint.IsLight)
        {
            tags.Add(GameUtils.GetString("CraftMaster.WeaponUsage.Light"));
        }
        else if (weaponBlueprint.IsTwoHanded)
        {
            tags.Add(GameUtils.GetString("CraftMaster.WeaponUsage.TwoHanded"));
        }
        else
        {
            tags.Add(GameUtils.GetString("CraftMaster.WeaponUsage.OneHanded"));
        }
        
        if (weaponBlueprint.IsMonk)
        {
            tags.Add(GameUtils.GetString("CraftMaster.WeaponUsage.Monk"));
        }
        
        if (weaponBlueprint.IsOneHandedWhichCanBeUsedWithTwoHands)
        {
            tags.Add(GameUtils.GetString("CraftMaster.WeaponUsage.TwoHanded"));
        }

        if (weaponBlueprint.Double)
        {
            tags.Add(GameUtils.GetString("CraftMaster.WeaponUsage.Double"));
        }
        
        return string.Join(", ", tags);
    }

    public static string GetArmorUsageName(BlueprintItemArmor armorBlueprint)
    {
        return GetString($"CraftMaster.ArmorUsage.{armorBlueprint.ProficiencyGroup.ToString()}");
    }

    public static void RemoveItemFromGame(ItemEntity itemEntity)
    {
        itemEntity.Collection?.Remove(itemEntity);
    }
    
    public static void AddItemToInventory(IEnumerable<ItemEntity> itemEntities)
    {
        foreach (var itemEntity in itemEntities)
        {
            RemoveItemFromGame(itemEntity);
            Game.Instance.Player.Inventory.Add(itemEntity);
        }
    }
    
    public static void AddItemToSharedStash(IEnumerable<ItemEntity> itemEntities)
    {
        foreach (var itemEntity in itemEntities)
        {
            RemoveItemFromGame(itemEntity);
            Game.Instance.Player.SharedStash.Add(itemEntity);
        }
    }
}
