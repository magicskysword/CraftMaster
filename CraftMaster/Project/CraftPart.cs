using System;
using System.Collections.Generic;
using System.Linq;
using BlueprintCore.Utils;
using CraftMaster.Builder;
using CraftMaster.Feats;
using Kingmaker;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.UI;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace CraftMaster.Project;

public class CraftPart : UnitPart
{
    /// <summary>
    /// 当前角色进行的所有项目
    /// </summary>
    [JsonProperty] 
    private List<CraftProject> Projects { get; set; } = new List<CraftProject>();
    /// <summary>
    /// 临时存储
    /// </summary>
    [JsonProperty]
    private ItemsCollection TempStorage { get; set; } = new ItemsCollection();

    public override void OnPostLoad()
    {
        base.OnPostLoad();

        var builderPart = DynamicBuilderManager.GetBuilderPart();
        if(builderPart == null)
            return;
        
        var removeProjects = new List<CraftProject>();
        foreach (var craftProject in Projects)
        {
            if (!builderPart.HasBuilder(craftProject.BuilderGuid))
            {
                removeProjects.Add(craftProject);
                continue;
            }
            
            craftProject.Author = Owner;
        }
        
        foreach (var craftProject in removeProjects)
        {
            Projects.Remove(craftProject);
        }

        if (removeProjects.Count > 0)
        {
            Main.Logger.Log($"Remove {removeProjects.Count} projects, because builder not found.");
        }
    }

    public CraftProject CreateAndAddProject(ProjectType type,
        DynamicBuilder builder, 
        int buildPoint,
        int price,
        int checkDC,
        string[] assetGuids = null)
    {
        var project = new CraftProject
        {
            Type = type,
            BuilderGuid = builder.Guid,
            TotalPoint = buildPoint,
            CurrentPoint = 0,
            CheckDC = checkDC,
            LeftTime = TimeSpan.Zero,
            Price = price,
            Author = Owner,
        };
        project.Author = Owner;
        if(assetGuids != null) 
            project.UsedAssets.AddRange(assetGuids);

        if (Main.Settings.InstantCraft)
        {
            FinishProject(project);
        }
        else
        {
            Projects.Add(project);
        }
        
        return project;
    }
    
    public IEnumerable<CraftProject> GetAllProjects()
    {
        foreach (var craftProject in Projects)
        {
            craftProject.Author = Owner;
            yield return craftProject;
        }
    }

    public void AddWeaponBuildProject(WeaponBuilder builder, int buildPoint, int price, int checkDC)
    {
        DynamicBuilderManager.AddWeaponBuilder(builder);
        var project = CreateAndAddProject(ProjectType.WeaponBuild, builder, buildPoint, price, checkDC);
        GameUtils.ChangeMoney(-project.Price);
        RefreshProjects();
    }
    
    public void AddArmorBuildProject(ArmorBuilder armorBuilder, int buildPoint, int price, int checkDC)
    {
        DynamicBuilderManager.AddArmorBuilder(armorBuilder);
        var project = CreateAndAddProject(ProjectType.ArmorBuild, armorBuilder, buildPoint, price, checkDC);
        GameUtils.ChangeMoney(-project.Price);
        RefreshProjects();
    }
    
    public void AddWeaponEnchantmentProject(ItemEntityWeapon weapon, WeaponBuilder builder, int buildPoint, int price, int checkDC)
    {
        DynamicBuilderManager.AddWeaponBuilder(builder);
        if (weapon.Collection != null)
        {
            weapon.Collection.Remove(weapon);
        }
        TempStorage.Add(weapon);
        var project = CreateAndAddProject(ProjectType.WeaponEnchantment, builder, buildPoint, price, checkDC, 
            new [] { weapon.UniqueId });
        GameUtils.ChangeMoney(-project.Price);
        RefreshProjects();
    }
    
    public void AddArmorEnchantmentProject(ItemEntityArmor armor, ArmorBuilder builder, int buildPoint, int price, int checkDC)
    {
        DynamicBuilderManager.AddArmorBuilder(builder);
        if (armor.Collection != null)
        {
            armor.Collection.Remove(armor);
        }
        TempStorage.Add(armor);
        var project = CreateAndAddProject(ProjectType.ArmorEnchantment, builder, buildPoint, price, checkDC, 
            new [] { armor.UniqueId });
        GameUtils.ChangeMoney(-project.Price);
        RefreshProjects();
    }
    
    public void AddWandBuildProject(WandBuilder wandBuilder, int buildPoint, int price, int checkDC)
    {
        DynamicBuilderManager.AddAndGetShardWandBuilder(wandBuilder);
        
        var project = CreateAndAddProject(ProjectType.WandBuild, wandBuilder, buildPoint, price, checkDC);
        GameUtils.ChangeMoney(-project.Price);
        RefreshProjects();
    }
    
    public void CancelProject(CraftProject craftProject)
    {
        if(!Projects.Remove(craftProject))
            return;
        
        GameUtils.ChangeMoney(craftProject.Price);
        foreach (var asset in craftProject.UsedAssets)
        {
            var item = TempStorage.Items.FirstOrDefault(item => item.UniqueId == asset);
            if (item != null)
            {
                TempStorage.Remove(item);
                Owner.Inventory.Add(item);
            }
        }
    }

    public void UpdateAllProjects(TimeSpan pastTime)
    {
        var removeList = new List<CraftProject>();
        int checkCount = (int)pastTime.TotalHours;
        Main.Logger.Log($"{Owner.CharacterName} CheckCount: {checkCount}");
        var craftDC = GetCraftDC();
        
        foreach (var project in Projects)
        {
            if(checkCount <= 0)
                break;
            
            checkCount = project.TryCheckDC(checkCount, craftDC);
            
            var needPoint = project.NeedPoint;
            if (needPoint <= 0)
            {
                removeList.Add(project);
            }
        }

        foreach (var project in removeList)
        {
            Projects.Remove(project);
            FinishProject(project);
        }

        RefreshProjects();
    }

    private void FinishProject(CraftProject project)
    {
        var resultItems = project.GetBuildResults();
        if (Owner.IsUnitInParty())
        {
            GameUtils.ShowLogMessage(GameUtils.GetString("CraftMaster.Project.FinishParty(author,project)").Format(
                Owner.CharacterName,
                project.GetProjectName())
            );
            GameUtils.AddItemToInventory(resultItems);
        }
        else
        {
            GameUtils.ShowLogMessage(GameUtils.GetString("CraftMaster.Project.FinishRemote(author,project)").Format(
                Owner.CharacterName, 
                project.GetProjectName())
            );
            GameUtils.AddItemToSharedStash(resultItems);
        }

        DeleteUsedAssets(project.UsedAssets);

        project.PlayFinishSound();
    }

    private void DeleteUsedAssets(List<string> projectUsedAssets)
    {
        if(projectUsedAssets == null || projectUsedAssets.Count == 0)
            return;
        
        foreach (var usedAsset in projectUsedAssets)
        {
            var item = TempStorage.Items.FirstOrDefault(item => item.UniqueId == usedAsset);
            if (item != null)
            {
                TempStorage.Remove(item);
            }
        }
    }

    private void RefreshProjects()
    {
        Main.NeedRefreshGUI = true;
        foreach (var project in Projects)
        {
            project.LeftTime = GetNeedTime(project.NeedPoint, project.CheckDC);
        }
    }

    public int GetCraftDC()
    {
        var skillArcana = Owner.Stats.GetStat(StatType.SkillKnowledgeArcana);
        var skillReligion = Owner.Stats.GetStat(StatType.SkillLoreReligion);
        var totalSkill = skillArcana.ModifiedValue + skillReligion.ModifiedValue;
        return totalSkill;
    }

    public TimeSpan GetNeedTime(int needPoint, int checkDC)
    {
        if(needPoint == 0)
            return TimeSpan.Zero;
        
        var totalSkill = GetCraftDC();
        
        var successRate = Mathf.Clamp(0.5f + (totalSkill + 10 - checkDC) / 10f, 0f, 1f);
        
        if(successRate == 0f)
            return TimeSpan.FromDays(needPoint * 20f / 24);
        
        if(successRate < 0.05f)
            successRate = 0.05f;
        
        var expectFactor = (totalSkill + 20 - checkDC) / 5f - 2;
        if (Owner.HasFeature(CraftPossessionOfConsummateSkill.FeatGuid))
        {
            // 增加等于一半神话等级（向上取整）的点数
            expectFactor += (int) Math.Ceiling(Owner.Progression.MythicLevel / 2f);
        }
        expectFactor = Mathf.Max(expectFactor, 1f);
        var expectTime = needPoint / expectFactor / successRate;
        expectTime = Mathf.Clamp(expectTime, 0f, 24 * 2000);

        return TimeSpan.FromHours(expectTime);
    }

    public bool IsCrafting(CraftProject project)
    {
        if(Projects.Count > 0)
            return Projects[0] == project;
        
        return false;
    }

    public void SetProjectFirst(CraftProject craftProject)
    {
        var index = Projects.IndexOf(craftProject);
        if(index == -1)
            return;
        
        Projects.RemoveAt(index);
        Projects.Insert(0, craftProject);
    }
}