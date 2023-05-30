using HarmonyLib;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Items;
using Kingmaker.UnitLogic.Abilities;

namespace CraftMaster.Builder;

[HarmonyPatch(typeof(ItemEntity))]
public class ItemEntityPatch
{
    [HarmonyPatch("Name", MethodType.Getter)]
    [HarmonyPrefix]
    public static bool NamePrefixPatch(ItemEntity __instance, ref string __result)
    {
        if (!GameUtils.IsGameStart())
            return true;
        
        if (DynamicBuilderManager.GetBuilderPart().TryGetMappingBuilder<EquipBuilder>(__instance.UniqueId, out var equipBuilder) == true)
        {
            __result = equipBuilder.EquipName;
            return false;
        }
        
        if (DynamicBuilderManager.GetBuilderPart().TryGetMappingBuilder<WandBuilder>(__instance.UniqueId, out var wandBuilder) == true)
        {
            var blueprint = __instance.Blueprint as BlueprintItemEquipmentUsable;
            var spellName = blueprint?.Ability?.Name;
            __result = GameUtils.GetString("CraftMaster.Builder.WandName(spellName)").Format(spellName);
            return false;
        }
        
        return true;
    }
    
    [HarmonyPatch("Blueprint", MethodType.Getter)]
    [HarmonyPrefix]
    public static bool AbilityPrefixPatch(ItemEntity __instance, ref BlueprintItem __result)
    {
        if (!GameUtils.IsGameStart())
            return true;

        if (DynamicBuilderManager.GetBuilderPart().TryGetMappingBuilder<WandBuilder>(__instance.UniqueId, out var builder))
        {
            __result = DynamicBuilderManager.GetWandBlueprint(builder);
            return false;
        }
        
        return true;
    }
}