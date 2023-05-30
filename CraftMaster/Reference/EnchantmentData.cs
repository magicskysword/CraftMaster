using System;
using BlueprintCore.Utils;
using CraftMaster.View;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Enums;
using Kingmaker.Items;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using ModKit.Utility;

namespace CraftMaster.Reference;

public delegate void EnchantmentDataAddChecker(ItemEntity item, UnitEntityData unit, ref CheckerContext checker);

public class EnchantmentData
{
    public string Key { get; set; }
    public string Guid { get; set; }
    public string[] ContainerEnchantments { get; set; }
    public string[] RequiredEnchantment { get; set; }
    public int Point { get; set; }
    public int CasterLevel { get; set; }
    public EnchantmentGroup Group { get; set; }
    public EnchantmentDataAddChecker AddApplyChecker { get; set; }
    
    public static void CheckAlignmentGood(ItemEntity item, UnitEntityData unit, ref CheckerContext checker)
    {
        if (unit.Alignment.ValueRaw != Alignment.LawfulGood && unit.Alignment.ValueRaw != Alignment.NeutralGood && unit.Alignment.ValueRaw != Alignment.ChaoticGood)
        {
            checker.IsEnable = false;
            checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.Alignment_NotGood").Red());
        }
        else
        {
            checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.Alignment_Good").Green());
        }
    }
    
    public static void CheckAlignmentEvil(ItemEntity item, UnitEntityData unit, ref CheckerContext checker)
    {
        if (unit.Alignment.ValueRaw != Alignment.LawfulEvil && unit.Alignment.ValueRaw != Alignment.NeutralEvil && unit.Alignment.ValueRaw != Alignment.ChaoticEvil)
        {
            checker.IsEnable = false;
            checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.Alignment_NotEvil").Red());
        }
        else
        {
            checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.Alignment_Evil").Green());
        }
    }
    
    public static void CheckAlignmentLawful(ItemEntity item, UnitEntityData unit, ref CheckerContext checker)
    {
        if (unit.Alignment.ValueRaw != Alignment.LawfulGood && unit.Alignment.ValueRaw != Alignment.LawfulNeutral && unit.Alignment.ValueRaw != Alignment.LawfulEvil)
        {
            checker.IsEnable = false;
            checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.Alignment_NotLawful").Red());
        }
        else
        {
            checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.Alignment_Lawful").Green());
        }
    }
    
    public static void CheckAlignmentChaotic(ItemEntity item, UnitEntityData unit, ref CheckerContext checker)
    {
        if (unit.Alignment.ValueRaw != Alignment.ChaoticGood && unit.Alignment.ValueRaw != Alignment.ChaoticNeutral && unit.Alignment.ValueRaw != Alignment.ChaoticEvil)
        {
            checker.IsEnable = false;
            checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.Alignment_NotChaotic").Red());
        }
        else
        {
            checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.Alignment_Chaotic").Green());
        }
    }

    public static EnchantmentDataAddChecker GetMagicChecker(string magicGuid)
    {
        void CheckMagicInternal(ItemEntity item, UnitEntityData unit, ref CheckerContext checker)
        {
            var bluePrint = BlueprintTool.Get<BlueprintAbility>(magicGuid);

            if (unit.HasAbility(bluePrint))
                checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.Magic_HasMagic(name)")
                    .Format(bluePrint.Name).Green());
            else
                checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.Magic_NoMagic(name)")
                    .Format(bluePrint.Name).Red());
        }

        return CheckMagicInternal;
    }

    public static EnchantmentDataAddChecker And(params EnchantmentDataAddChecker[] checkers)
    {
        void CheckAnd(ItemEntity item, UnitEntityData unit, ref CheckerContext checker)
        {
            var andContext = new CheckerContext();
            foreach (var addChecker in checkers)
            {
                addChecker(item, unit, ref andContext);
            }

            if (andContext.IsEnable)
            {
                checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.And").Green());
                foreach (var tooltip in andContext.Tooltips)
                {
                    checker.Tooltips.Add($"\t{tooltip}");
                }
            }
            else
            {
                checker.IsEnable = false;
                checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.And").Red());
                foreach (var tooltip in andContext.Tooltips)
                {
                    checker.Tooltips.Add($"\t{tooltip}");
                }
            }
        }

        return CheckAnd;
    }

    public static EnchantmentDataAddChecker Or(params EnchantmentDataAddChecker[] checkers)
    {
        void CheckAnd(ItemEntity item, UnitEntityData unit, ref CheckerContext checker)
        {
            var orContext = new CheckerContext();
            var isEnable = false;
            foreach (var addChecker in checkers)
            {
                orContext.IsEnable = true;
                addChecker(item, unit, ref orContext);
                if (orContext.IsEnable)
                {
                    isEnable = true;
                }
            }

            if (isEnable)
            {
                checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.Or").Green());
                foreach (var tooltip in orContext.Tooltips)
                {
                    checker.Tooltips.Add($"\t{tooltip}");
                }
            }
            else
            {
                checker.IsEnable = false;
                checker.Tooltips.Add(GameUtils.GetString("CraftMaster.EnchantmentGroup.Or").Red());
                foreach (var tooltip in orContext.Tooltips)
                {
                    checker.Tooltips.Add($"\t{tooltip}");
                }
            }
        }

        return CheckAnd;
    }
}