using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueprintCore.Utils;
using CraftMaster.Builder;
using CraftMaster.Project;
using CraftMaster.Reference;
using CraftMaster.View;
using Kingmaker;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Items;
using ModKit;
using ModKit.Utility;
using UnityEngine;

namespace CraftMaster;

public partial class Main
{
    private static int SelectedEnchantmentTab = 0;
    private static List<NamedAction> EnchantmentTabs = new();
    private static WeaponEnchantmentView WeaponEnchantmentView = new();
    private static void RenderCraftEnchantments()
    {
        EnchantmentTabs.Clear();
        EnchantmentTabs.Add(new NamedAction(GameUtils.GetString("CraftMaster.UI.EnchantmentType_Weapon"), 
            () => RenderEnchantment(WeaponEnchantmentView)));
            
        UI.TabBar(ref SelectedEnchantmentTab, null, EnchantmentTabs.ToArray());
    }

    private static int SelectedItemFromTab = 0;
    private static DateTime LastItemFromTabCheck = DateTime.Now;
    private static List<ItemEntity> LoadItems = new();
    private static List<string> ItemFromTabs = new()
    {
        "CraftMaster.UI.ItemFromTab_All",
        "CraftMaster.UI.ItemFromTab_Active",
        "CraftMaster.UI.ItemFromTab_Inventory",
        "CraftMaster.UI.ItemFromTab_Equipment",
        "CraftMaster.UI.ItemFromTab_Chest",
    };
    
    private static void CheckItemFromTabs(Func<ItemEntity, bool> filter)
    {
        bool needRefresh = false;

        CMGUI.NTitle("CraftMaster.UI.ItemFromTab");
        var newValue = CMGUI.ToggleGroupNumber(Enumerable.Range(0, ItemFromTabs.Count), SelectedItemFromTab, 
            index => GameUtils.GetString(ItemFromTabs[index]));
        if(newValue != SelectedItemFromTab)
        {
            SelectedItemFromTab = newValue;
            needRefresh = true;
        }

        // 
        if(!needRefresh && DateTime.Now.Subtract(LastItemFromTabCheck).TotalSeconds < 10)
            return;
        
        LastItemFromTabCheck = DateTime.Now;

        LoadItems.Clear();

        bool Check(ItemEntity entity)
        {
            if (entity.IsNonRemovable)
                return false;

            if (entity.IsPartOfAnotherItem)
                return false;
            
            return filter(entity);
        }

        if (SelectedItemFromTab == 0 || SelectedItemFromTab == 1 || SelectedItemFromTab == 2)
        {
            // Inventory
            foreach (var item in Game.Instance.Player.Inventory.Items)
            {
                if (!Check(item))
                    continue;
                
                LoadItems.Add(item);
            }
        }
        
        if (SelectedItemFromTab == 3)
        {
            // Equipment
            foreach (var item in Game.Instance.Player.Inventory.Items)
            {
                if(item.Owner == null)
                    continue;
                
                if (!Check(item))
                    continue;
                
                LoadItems.Add(item);
            }
        }
        
        if (SelectedItemFromTab == 0 || SelectedItemFromTab == 1 || SelectedItemFromTab == 4)
        {
            // Chest
            foreach (var item in Game.Instance.Player.SharedStash.Items)
            {
                if (!Check(item))
                    continue;
                
                LoadItems.Add(item);
            }
        }
    }
    
    private static StringBuilder _tooltipBuilder = new();
    private static void RenderEnchantment(EnchantmentView view)
    {
        CheckItemFromTabs(view.Filter);
        
        void OnRenderEquipmentSelect()
        {
            var oldItem = view.Item;
            view.Item = CMGUI.ToggleGroup(LoadItems, view.Item, item => item.Name, numPerRow: 1);

            if (view.Item == null)
            {
                view.Item = null;
                view.Builder = null;
            }
            else if(view.Item != oldItem)
            {
                view.Builder = view.GetNewBuilder(view.Item); 
            }
        }

        void OnRenderAllWeaponEnchantment()
        {
            if(view.Builder == null)
                return;
            
            foreach (var group in view.AllEnchantmentGroups)
            {
                var groupKey = group.Key;
                var isExpand = CMGUI.NToggleRaw($"{GameUtils.GetString(group.NameStringKey)}", view.IsExpanded(groupKey));
                view.SetExpand(groupKey, isExpand);
                
                if (isExpand)
                {
                    foreach (var dataPair in group.Enchantments)
                    {
                        var data = dataPair.Value;
                        var checker = view.CheckAddEnchantment(SelectedUnit, group, data);
                        
                        _tooltipBuilder.Clear();
                        _tooltipBuilder.Append(GameUtils.GetString("CraftMaster.UI.EnchantmentName(name)").Format(checker.Text));
                        _tooltipBuilder.Append("\n");
                        _tooltipBuilder.Append(GameUtils.GetString("CraftMaster.UI.EnchantmentPointUsage(point)").Format(data.Point));
                        _tooltipBuilder.Append("\n");
                        _tooltipBuilder.Append(GameUtils.GetString("CraftMaster.UI.EnchantmentCasterLevel(level)").Format(data.CasterLevel));
                        _tooltipBuilder.Append("\n");
                        
                        if (!string.IsNullOrEmpty(checker.Description))
                        {
                            _tooltipBuilder.Append(GameUtils
                                .GetString("CraftMaster.UI.EnchantmentCasterLevel(description)")
                                .Format(checker.Description.StripHTML()));
                        }
                        
                        if (!string.IsNullOrEmpty(checker.Tooltip))
                        {
                            _tooltipBuilder.Append("\n");
                            _tooltipBuilder.Append("\n");
                            _tooltipBuilder.Append(checker.Tooltip);
                        }
                        
                        GUI.enabled = checker.IsEnable;
                        
                        if (CMGUI.NButtonWithTooltipRaw(checker.Text, _tooltipBuilder.ToString()))
                        {
                            DynamicBuilderManager.AddEnchantment(view.Builder, group, data);
                        }

                        GUI.enabled = true;
                    }
                }

                CMGUI.VerticalSpace(5);
            }
        }
        
        void OnRenderEquipmentEnchantment()
        {
            if(view.Builder == null)
                return;
            
            var enchantments = view.Builder.ActuallyEnhancement;
            var weaponPoint = view.Builder.TotalEnchantmentPoint;
            CMGUI.NTitleRaw(GameUtils.GetString("CraftMaster.UI.EnchantmentPoint(point)").Format(weaponPoint));
            CMGUI.VerticalSpace(2);
            CMGUI.NTitleRaw(GameUtils.GetString("CraftMaster.UI.Enhancement(point)").Format(enchantments));

            // 已有附魔绘制
            CMGUI.NTitleRaw(GameUtils.GetString("CraftMaster.UI.HasEnchantment"));
            CMGUI.Box(() =>
            {
                foreach (var pair in view.Builder.RawEnchantmentGroups)
                {
                    var group = view.GetEnchantmentGroup(pair.Key);
                    if(group == null)
                        continue;
                
                    CMGUI.NLabelRaw($"{GameUtils.GetString(group.NameStringKey)}".Size(14).Bold());
                
                    foreach (var enchantmentKey in pair.Value)
                    {
                        var data = view.GetEnchantment(pair.Key ,enchantmentKey);
                        if(data == null)
                            continue;
                    
                        var blueprint = BlueprintTool.Get<BlueprintWeaponEnchantment>(data.Guid);
                        CMGUI.NLabelRaw(blueprint.Name);
                    }
                    CMGUI.VerticalSpace(5);
                }

                if (view.Builder.OtherEnchantment.Count > 0)
                {
                    CMGUI.VerticalSpace(5);
                    CMGUI.NLabelRaw($"{GameUtils.GetString("CraftMaster.EnchantmentGroup.Other")}".Size(14).Bold());
                    foreach (var enchantment in view.Builder.OtherEnchantment)
                    {
                        CMGUI.NLabelRaw(enchantment.Blueprint.Name);
                    }
                }
            });
            
            // 新增附魔绘制
            CMGUI.NTitleRaw(GameUtils.GetString("CraftMaster.UI.AddEnchantment").Size(14).Bold());
            CMGUI.Box(() =>
            {
                string removeKey = null;
                foreach (var pair in view.Builder.EnchantmentGroups)
                {
                    var group = view.GetEnchantmentGroup(pair.Key);
                    if(group == null)
                        continue;
                
                    CMGUI.NLabelRaw($"{GameUtils.GetString(group.NameStringKey)}");
                
                    foreach (var enchantmentKey in pair.Value)
                    {
                        var data = view.GetEnchantment(pair.Key ,enchantmentKey);
                        if(data == null)
                            continue;
                    
                        var blueprint = BlueprintTool.Get<BlueprintWeaponEnchantment>(data.Guid);

                        _tooltipBuilder.Clear();
                        _tooltipBuilder.Append(GameUtils.GetString("CraftMaster.UI.EnchantmentName(name)").Format(blueprint.Name));
                        _tooltipBuilder.Append("\n");
                        _tooltipBuilder.Append(GameUtils.GetString("CraftMaster.UI.EnchantmentPointUsage(point)").Format(data.Point));
                        _tooltipBuilder.Append("\n");
                        _tooltipBuilder.Append(GameUtils.GetString("CraftMaster.UI.EnchantmentCasterLevel(level)").Format(data.CasterLevel));
                        _tooltipBuilder.Append("\n");
                    
                        if (!string.IsNullOrEmpty(blueprint.Description))
                        {
                            _tooltipBuilder.Append(GameUtils
                                .GetString("CraftMaster.UI.EnchantmentCasterLevel(description)")
                                .Format(blueprint.Description.StripHTML()));
                        }

                        if (CMGUI.NButtonWithTooltipRaw(blueprint.Name, _tooltipBuilder.ToString()))
                        {
                            removeKey = enchantmentKey;
                        }
                    }
                    CMGUI.VerticalSpace(5);
                }
                
                if(!string.IsNullOrEmpty(removeKey))
                    DynamicBuilderManager.RemoveEnchantment(view.Builder, removeKey);
            });
        }

        void OnRenderBuildState()
        {
            if(view.Builder == null)
                return;
            
            view.Builder.EquipName = CMGUI.NInput("CraftMaster.UI.EquipInputName" ,view.Builder.EquipName, 200);
            CMGUI.VerticalSpace(5);
            CMGUI.VerticalSpace(5);
            var cost = view.Builder.GetEnchantmentCost() - view.Builder.GetRawEnchantmentCost();
            var price = cost / 2;
            var hasMoney = Game.Instance.Player.Money >= price;
            
            if (hasMoney)
                CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.CraftCost(money)").Format(price));
            else
                CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.InsufficientCraftCost(money)").Format(price));
            var buildPoint = view.Builder.GetBuildPoint();
            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.BuildPoint(point)").Format(buildPoint));
            var checkDC = view.Builder.GetCheckDC();
            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.CheckDC(time)").Format(view.Builder.GetCheckDC()));
            var needTime = SelectedUnit.GetOrCreateCraftPart().GetNeedTime(buildPoint, checkDC).GetLeftTimeDescription();
            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.NeedTime(time)").Format(needTime));
            CMGUI.VerticalSpace(10);
            
            GUI.enabled = hasMoney && !string.IsNullOrEmpty(view.Builder.EquipName);
            if (CMGUI.NButton("CraftMaster.UI.Craft", 200))
            {
                var craftPart = SelectedUnit.GetOrCreateCraftPart();
                view.CreateProject(craftPart, buildPoint, price, checkDC);
            }
            GUI.enabled = true;
        }
        
        CMGUI.Horizontal(() =>
        {
            CMGUI.Vertical(() =>
            {
                CMGUI.NTitle("CraftMaster.UI.Enchantment_SelectItem");
                CMGUI.Box(OnRenderEquipmentSelect);
            }, GUILayout.MinWidth(200));
            
            CMGUI.HorizontalSpace(40);
            
            CMGUI.Vertical(() =>
            {
                CMGUI.NTitle("CraftMaster.UI.Enchantment_All");
                CMGUI.Box(OnRenderAllWeaponEnchantment);
            }, GUILayout.MinWidth(200));
            
            CMGUI.HorizontalSpace(40);
            
            CMGUI.Vertical(() =>
            {
                CMGUI.NTitle("CraftMaster.UI.Enchantment_Selected");
                CMGUI.Box(OnRenderEquipmentEnchantment);
            }, GUILayout.MinWidth(200));
            
            CMGUI.HorizontalSpace(40);
            
            CMGUI.Vertical(() =>
            {
                CMGUI.NTitle("CraftMaster.UI.Enchantment_BuildState");
                CMGUI.Box(OnRenderBuildState);
            }, GUILayout.MinWidth(200));
            
            CMGUI.FlexibleSpace();
        });
    }

    
}