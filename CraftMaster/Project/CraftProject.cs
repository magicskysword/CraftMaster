using System;
using System.Collections.Generic;
using BlueprintCore.Utils;
using CraftMaster.Builder;
using CraftMaster.Feats;
using CraftMaster.Reference;
using Kingmaker;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;
using Kingmaker.UI;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CraftMaster.Project;

public class CraftProject
{
    [JsonIgnore]
    public UnitEntityData Author { get; set; }
    
    /// <summary>
    /// 构建蓝图的GUID
    /// </summary>
    [JsonProperty]
    public string BuilderGuid { get; set; }
    
    /// <summary>
    /// 构建的类型
    /// </summary>
    [JsonProperty]
    public ProjectType Type { get; set; }
    
    /// <summary>
    /// 剩余时间
    /// </summary>
    [JsonProperty]
    public TimeSpan LeftTime { get; set; }
    
    /// <summary>
    /// 需要的点数
    /// </summary>
    [JsonProperty]
    public int TotalPoint { get; set; }
    
    /// <summary>
    /// 当前点数
    /// </summary>
    [JsonProperty]
    public int CurrentPoint { get; set; }
    
    /// <summary>
    /// 成本
    /// </summary>
    [JsonProperty]
    public int Price { get; set; }
    
    /// <summary>
    /// 使用的原材料
    /// </summary>
    [JsonProperty]
    public List<string> UsedAssets { get; set; } = new List<string>();

    /// <summary>
    /// 检定DC
    /// </summary>
    [JsonProperty] 
    public int CheckDC { get; set; } = 5;
    
    [JsonIgnore]
    public int NeedPoint => TotalPoint - CurrentPoint;




    public string GetProjectName()
    {
        switch (Type)
        {
            case ProjectType.WandBuild:
            {
                DynamicBuilderManager.GetBuilderPart().TryGetBuilder(BuilderGuid, out UsableBuilder builder);
                var ability = BlueprintTool.Get<BlueprintAbility>(builder.TagAbility);
                return GameUtils.GetString("CraftMaster.Project.Craft(type,name)").Format(
                    GameUtils.GetString("CraftMaster.Project.Wand"),
                    GameUtils.GetString("CraftMaster.Builder.WandName(spellName)").Format(ability.Name));
            }
            case ProjectType.WeaponBuild:
            {
                DynamicBuilderManager.GetBuilderPart().TryGetBuilder(BuilderGuid, out WeaponBuilder builder);
                var itemWeapon = BlueprintTool.Get<BlueprintItemWeapon>(builder.GetPrototypeGuid());
                return GameUtils.GetString("CraftMaster.Project.Craft(type,name)").Format(
                    itemWeapon.GetWeaponTypeName(),
                    builder.EquipName);
            }
            case ProjectType.WeaponEnchantment:
            {
                DynamicBuilderManager.GetBuilderPart().TryGetBuilder(BuilderGuid, out WeaponBuilder builder);
                var itemWeapon = BlueprintTool.Get<BlueprintItemWeapon>(builder.GetPrototypeGuid());
                return GameUtils.GetString("CraftMaster.Project.Enchantment(type,name)").Format(
                    itemWeapon.GetWeaponTypeName(),
                    builder.EquipName);
            }
            case ProjectType.ArmorBuild:
            {
                DynamicBuilderManager.GetBuilderPart().TryGetBuilder(BuilderGuid, out ArmorBuilder builder);
                var itemArmor = BlueprintTool.Get<BlueprintItemArmor>(builder.GetPrototypeGuid());
                return GameUtils.GetString("CraftMaster.Project.Craft(type,name)").Format(
                    itemArmor.GetArmorTypeName(),
                    builder.EquipName);
            }
        }
        
        return GameUtils.GetString("CraftMaster.Project.Unknown");
    }

    public ItemEntity GetBuildResult()
    {
        switch (Type)
        {
            case ProjectType.WeaponBuild:
            case ProjectType.WeaponEnchantment:
                DynamicBuilderManager.GetBuilderPart().TryGetBuilder(BuilderGuid, out WeaponBuilder builder);
                return DynamicBuilderManager.CreateEquip(builder,
                    materialsReference:ReferenceManager.Weapon,
                    enchantmentReference:ReferenceManager.Weapon,
                    enhancementReference:ReferenceManager.Weapon);
            case ProjectType.ArmorBuild:
                DynamicBuilderManager.GetBuilderPart().TryGetBuilder(BuilderGuid, out ArmorBuilder armorBuilder);
                return DynamicBuilderManager.CreateEquip(armorBuilder,
                    materialsReference:ReferenceManager.Armor,
                    enchantmentReference:ReferenceManager.Armor,
                    enhancementReference:ReferenceManager.Armor);
            case ProjectType.WandBuild:
                DynamicBuilderManager.GetBuilderPart().TryGetBuilder(BuilderGuid, out WandBuilder wandBuilder);
                return DynamicBuilderManager.CreateWand(wandBuilder);
        }
        
        Main.Logger.Error("Unknown project type: " + Type);
        return null;
    }

    public void PlayFinishSound()
    {
        switch (Type)
        {
            case ProjectType.WeaponEnchantment:
            case ProjectType.WeaponBuild:
            case ProjectType.ArmorBuild:
                Game.Instance.UI.UISound.Play(UISoundType.ChargenLoadPremadeBuildClick);
                break;
            case ProjectType.WandBuild:
                Game.Instance.UI.UISound.Play(UISoundType.GlobalMapRandomEncounter);
                break;
        }
    }

    public int TryCheckDC(int checkCount, int dc)
    {
        for (; checkCount > 0 && CurrentPoint < TotalPoint; checkCount--)
        {
            var roll = Random.Range(1, 20);
            var result = roll + dc;
            int addPoint;
            if (result < CheckDC && roll != 20)
            {
                continue;
            }
            if (roll == 20)
            {
                addPoint = 1;
            }
            else
            {
                var extraPoint = (result - CheckDC) / 5;
                addPoint = 1 + extraPoint;
            }

            if (Author.HasFeature(CraftPossessionOfConsummateSkill.FeatGuid))
            {
                // 增加等于一半神话等级（向上取整）的点数
                addPoint += (int) Math.Ceiling(Author.Progression.MythicLevel / 2f);
            }
            
            CurrentPoint += addPoint;
            CurrentPoint = Mathf.Min(CurrentPoint, TotalPoint);
            GameUtils.ShowLogMessage(
                GameUtils.GetString("CraftMaster.UI.ProjectCheckDCLog(roleName,itemName,checkDC,dc,roll,point,currentPoint,totalPoint)")
                    .Format(
                        Author.CharacterName,
                        GetProjectName(),
                        CheckDC,
                        dc,
                        roll,
                        addPoint,
                        CurrentPoint,
                        TotalPoint)
                );
        }

        return checkCount;
    }
}