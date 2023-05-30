using System.Collections.Generic;
using BlueprintCore.Utils;
using CraftMaster.Builder;
using CraftMaster.Project;
using CraftMaster.Reference;
using CraftMaster.View;
using Kingmaker;
using Kingmaker.Blueprints.Items.Weapons;
using ModKit;
using UnityEngine;

namespace CraftMaster;

public partial class Main
{
    private static int SelectedCraftNormalEquipmentTab = 0;
    private static List<NamedAction> CraftNormalEquipmentTabs = new();
    private static WeaponEquipmentView NormalWeaponView = new();
    private static ArmorEquipmentView NormalArmorView = new();
    private static void RenderCraftNormalEquipments()
    {
        CraftNormalEquipmentTabs.Clear();
        CraftNormalEquipmentTabs.Add(new NamedAction(GameUtils.GetString("CraftMaster.UI.CraftType_Weapon"), 
            () => RenderCraftNormalEquipment(NormalWeaponView)));
        CraftNormalEquipmentTabs.Add(new NamedAction(GameUtils.GetString("CraftMaster.UI.CraftType_Armor"), 
            () => RenderCraftNormalEquipment(NormalArmorView)));
        
            
        UI.TabBar(ref SelectedCraftNormalEquipmentTab, null, CraftNormalEquipmentTabs.ToArray());
    }
    
    private static void RenderCraftNormalEquipment(NormalEquipmentView view)
    {
        if (view.Builder == null)
        {
            view.ResetBuilder();
        }
        
        view.Builder!.EquipName = CMGUI.NInput("CraftMaster.UI.WeaponName", view.Builder.EquipName);

        void RenderWeaponType(KeyValuePair<string, string> pair, int curIndex, GUILayoutOption[] options)
        {
            var weaponTypeName = view.GetTypeNameByGuid(pair.Value);
            if (CMGUI.NToggleButtonRaw(weaponTypeName, view.Builder.EquipType == pair.Key, options))
            {
                view.Builder.EquipType = pair.Key;
                view.Builder.EquipName = weaponTypeName;
            }
            var tooltip = view.GetTooltipByGuid(pair.Value);
            CMGUI.SetTooltip(tooltip);
        }

        CMGUI.Box("CraftMaster.UI.EquipType", () =>
        {
            var equipTypes = view.EquipTypeEntry;

            foreach (var equipTypeView in equipTypes)
            {
                CMGUI.NTitle(equipTypeView.NameKey);
                CMGUI.Grid(equipTypeView.EquipTypes, RenderWeaponType);
            }
        });

        void RenderMaterial(KeyValuePair<string, string[]> pair,int curIndex, GUILayoutOption[] options)
        {
            if (CMGUI.NToggleButton($"CraftMaster.UI.Material_{pair.Key}", view.Builder.MaterialType == pair.Key,
                    options))
            {
                view.Builder.MaterialType = pair.Key;
            }
        }

        CMGUI.NTitle("CraftMaster.UI.Material");
        CMGUI.Grid(view.ReferenceMaterials, RenderMaterial);
        
        var isValid = view.Builder.IsValid();
        var cost = 0;
        if (isValid)
        {
            cost = view.Builder.GetRawCost();
        }
        var spendMoney = cost / 2;
        
        if (Game.Instance.Player.Money < spendMoney)
        {
            CMGUI.NTitleRaw(GameUtils.GetString("CraftMaster.UI.InsufficientCraftCost(money)").Format(spendMoney));
        }
        else
        {
            CMGUI.NTitleRaw(GameUtils.GetString("CraftMaster.UI.CraftCost(money)").Format(spendMoney));
        }
        var buildPoint = view.Builder.GetBuildPoint();
        CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.BuildPoint(point)").Format(buildPoint));
        var checkDC = view.Builder.GetCheckDC();
        CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.CheckDC(time)").Format(checkDC));
        var needTime = SelectedUnit.GetOrCreateCraftPart().GetNeedTime(buildPoint, checkDC).GetLeftTimeDescription();
        CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.NeedTime(time)").Format(needTime));
        
        CMGUI.VerticalSpace(10);

        GUI.enabled = isValid && Game.Instance.Player.Money >= spendMoney;
        if (CMGUI.NButton("CraftMaster.UI.Craft", 200))
        {
            var craftPart = SelectedUnit.GetOrCreateCraftPart();
            view.AddBuildProject(craftPart, buildPoint, spendMoney, checkDC);
            view.ResetBuilder();
        }

        GUI.enabled = true;
    }
}