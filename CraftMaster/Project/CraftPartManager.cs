using System;
using System.Collections.Generic;
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI;

namespace CraftMaster.Project;

public static class CraftPartManager
{
    public static CraftPart GetOrCreateCraftPart(this UnitEntityData unit)
    {
        var craft = unit.Parts.Get<CraftPart>();
        if (craft == null)
        {
            craft = unit.Parts.Add<CraftPart>();
        }

        return craft;
    }
    
    public static bool TryGetCraftPart(this UnitEntityData unit, out CraftPart craft)
    {
        craft = unit.Parts.Get<CraftPart>();
        return craft != null;
    }

    public static bool CanCraft(this UnitEntityData unit)
    {
        return unit.IsPlayerFaction && !unit.IsPet && !unit.Descriptor.State.IsDead &&
               !unit.Descriptor.State.IsFinallyDead;
    }
    
    public static bool IsCrafting(this CraftProject project)
    {
        var craftPart = project.Author.GetOrCreateCraftPart();
        return craftPart.IsCrafting(project);
    }
    
    public static IEnumerable<CraftProject> GetAllCraftProjects()
    {
        foreach (var entityData in Game.Instance.Player.AllCharacters)
        {
            if(!entityData.CanCraft())
                continue;
            
            if (entityData.TryGetCraftPart(out var craftPart))
            {
                foreach (var project in craftPart.GetAllProjects())
                {
                    yield return project;
                }
            }
        }
    }
    
    public static void UpdateAllCraftProjects(TimeSpan pastTime)
    {
        foreach (var entityData in Game.Instance.Player.AllCharacters)
        {
            if(!entityData.CanCraft())
                continue;

            if (entityData.TryGetCraftPart(out var craftPart))
            {
                craftPart.UpdateAllProjects(pastTime);
            }
        }
    }

    public static void CancelCraftProject(CraftProject craftProject)
    {
        if (craftProject.Author.TryGetCraftPart(out var craftPart))
        {
            craftPart.CancelProject(craftProject);
        }
    }

    public static void SetCraftProjectFirst(CraftProject craftProject)
    {
        if (craftProject.Author.TryGetCraftPart(out var craftPart))
        {
            craftPart.SetProjectFirst(craftProject);
        }
    }
}