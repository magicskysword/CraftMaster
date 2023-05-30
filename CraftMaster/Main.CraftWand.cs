using System;
using System.Collections.Generic;
using System.Linq;
using CraftMaster.Builder;
using CraftMaster.Feats;
using CraftMaster.Project;
using Kingmaker;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using ModKit;
using UnityEngine;

namespace CraftMaster;

public partial class Main
{
    private static WandBuilder WandBuilder = new();
    private static Spellbook Spellbook = null;
    private static int SelectedSpellLevel = 0;
    private static int SelectedCasterLevel = 0;

    private static List<int> CanSelectSpellLevel = new();
    private static List<BlueprintAbility> CanSelectSpell = new();
    
    private static BlueprintAbility SelectedSpell = null;
    private static RefreshTimer WandRefreshTimer = new();

    private static void RenderCraftWand()
    {
        void OnRenderSpellBooks()
        {
            var oldSpellBook = Spellbook;
            
            var allSpellBook = SelectedUnit.Spellbooks.ToList();
            if (allSpellBook.Count == 0)
            {
                Spellbook = null;
                return;
            }

            Spellbook = CMGUI.ToggleGroup(allSpellBook, Spellbook, 
                spellBook => $"{spellBook.Blueprint.Name} (Cl {spellBook.CasterLevel})", 
                numPerRow: 1);
            if (oldSpellBook != Spellbook || WandRefreshTimer.CheckNeedRefresh())
                WandRefreshTimer.IsDirty = true;
        }
        
        void OnRenderSpells()
        {
            if(Spellbook == null)
                return;
            
            if (WandRefreshTimer.IsDirty)
            {
                CanSelectSpellLevel.Clear();
                var haveMythicCraftWand = SelectedUnit.HasFeature(CraftWandMythicFeat.FeatGuid);
                if (haveMythicCraftWand || Settings.IgnoreMythicFeatureLimit)
                {
                    for (int i = 0; i <= Spellbook.GetMaxSpellLevel(); i++)
                    {
                        CanSelectSpellLevel.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i <= Spellbook.GetMaxSpellLevel() && i <= 4; i++)
                    {
                        CanSelectSpellLevel.Add(i);
                    }
                }
            }
            
            CMGUI.NTitle("CraftMaster.UI.SpellLevelList");
            
            var oldSpellLevel = SelectedSpellLevel;
            SelectedSpellLevel = CMGUI.ToggleGroupNumber(CanSelectSpellLevel, SelectedSpellLevel,
                spellLevel => GameUtils.GetString("CraftMaster.UI.WandSpellLevel(level)").Format(spellLevel),
                numPerRow: 1);

            if (oldSpellLevel != SelectedSpellLevel || WandRefreshTimer.IsDirty)
            {
                CanSelectSpell.Clear();
                // 获取所有可选法术
                var customSpells = Spellbook.GetCustomSpells(SelectedSpellLevel);
                CanSelectSpell.AddRange(customSpells.GetCanSpellAbilities());
                var knownSpells = Spellbook.GetKnownSpells(SelectedSpellLevel);
                CanSelectSpell.AddRange(knownSpells.GetCanSpellAbilities());
                var specialSpells = Spellbook.GetSpecialSpells(SelectedSpellLevel);
                CanSelectSpell.AddRange(specialSpells.GetCanSpellAbilities());
                // 去重
                CanSelectSpell = CanSelectSpell.Distinct().ToList();
            }
            
            CMGUI.NTitle("CraftMaster.UI.CurSpellLevelSpells");

            var oldSelectedSpell = SelectedSpell;
            if (CanSelectSpell.Count == 0)
            {
                SelectedSpell = null;
                CMGUI.NTitle("CraftMaster.UI.NoSpell");
            }
            else
            {
                SelectedSpell = CMGUI.ToggleGroup(CanSelectSpell, SelectedSpell, spell => spell.Name.StripHTML(),
                    spell =>
                    {
                        var spellName = spell.Name.StripHTML();
                        var spellLevel = SelectedSpellLevel;
                        var durationKey = spell.LocalizedDuration.Key;
                        var duration = string.IsNullOrEmpty(durationKey) ? GameUtils.GetString("CraftMaster.UI.None") : GameUtils.GetString(spell.LocalizedDuration.Key);
                        var minCasterLevel = Spellbook.GetMinCasterLevelForUsable(spell, SelectedSpellLevel);
                        var description = spell.Description.StripHTML();
                        return GameUtils.GetString("CraftMaster.UI.SpellInfo(name,spellLevel,duration,minCastLevel,description)")
                            .Format(spellName, spellLevel, duration, minCasterLevel, description);
                    }, 1);
            }

            if (SelectedSpell == null)
            {
                WandBuilder = null;
            }
            else if(oldSelectedSpell != SelectedSpell)
            {
                WandBuilder = DynamicBuilderManager.GetWandNewBuilder(SelectedSpell, SelectedSpellLevel);
            }
            
            WandRefreshTimer.IsDirty = false;
        }
        
        void OnRenderSpellInfo()
        {
            if(WandBuilder == null || Spellbook == null || SelectedSpell == null)
                return;
            
            var spellName = SelectedSpell.Name.StripHTML();
            var spellLevel = SelectedSpellLevel;
            var durationKey = SelectedSpell.LocalizedDuration.Key;
            var duration = string.IsNullOrEmpty(durationKey) ? GameUtils.GetString("CraftMaster.UI.None") : GameUtils.GetString(SelectedSpell.LocalizedDuration.Key);
            var minCasterLevel = Spellbook.GetMinCasterLevelForUsable(SelectedSpell, SelectedSpellLevel);
            var description = SelectedSpell.Description.StripHTML();
            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.SpellName(name)").Format(spellName));
            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.DurationTime(timeDesc)").Format(duration));
            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.SpellLevel(level)").Format(spellLevel));
            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.SpellMinCasterLevel(level)").Format(minCasterLevel));
            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.SpellDescription(description)").Format(description));
        }

        void OnRenderBuildState()
        {
            if(WandBuilder == null || Spellbook == null || SelectedSpell == null)
                return;
            
            var spellName = SelectedSpell.Name.StripHTML();
            var name = GameUtils.GetString("CraftMaster.Builder.WandName(spellName)").Format(spellName);
            CMGUI.NTitleRaw(name);
            var minCasterLevel = Spellbook.GetMinCasterLevelForUsable(SelectedSpell, SelectedSpellLevel);
            SelectedCasterLevel = CMGUI.NSliderInt("CraftMaster.UI.CasterLevel", SelectedCasterLevel,
                minCasterLevel, Spellbook.CasterLevel);
            WandBuilder.CasterLevel = SelectedCasterLevel;
            WandBuilder.SpellLevel = SelectedSpellLevel;
            WandBuilder.GenerateDC(SelectedUnit);
            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.WandDC(dc)").Format(WandBuilder.DC));
            CMGUI.VerticalSpace(10);
            var cost = WandBuilder.GetRawCost();
            var price = cost / 2;
            var hasMoney = Game.Instance.Player.Money >= price;
            
            if (hasMoney)
                CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.CraftCost(money)").Format(price));
            else
                CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.InsufficientCraftCost(money)").Format(price));
            var buildPoint = WandBuilder.GetBuildPoint();
            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.BuildPoint(point)").Format(buildPoint));
            var dc = WandBuilder.GetCheckDC();
            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.CheckDC(time)").Format(dc));
            var needTime = SelectedUnit.GetOrCreateCraftPart().GetNeedTime(buildPoint, dc).GetLeftTimeDescription();
            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.NeedTime(time)").Format(needTime));
            CMGUI.VerticalSpace(10);
            
            GUI.enabled = hasMoney;
            if (CMGUI.NButton("CraftMaster.UI.Craft", 200))
            {
                var craftPart = SelectedUnit.GetOrCreateCraftPart();
                craftPart.AddWandBuildProject(WandBuilder, buildPoint , price, WandBuilder.GetCheckDC());
                WandBuilder = null;
            }
            GUI.enabled = true;
        }

        CMGUI.Horizontal(() =>
        {
            CMGUI.Vertical(() =>
            {
                CMGUI.NTitle("CraftMaster.UI.SpellBook");
                CMGUI.Box(OnRenderSpellBooks);
            }, GUILayout.MinWidth(200));
            
            CMGUI.HorizontalSpace(40);
            
            CMGUI.Vertical(() =>
            {
                CMGUI.NTitle("CraftMaster.UI.Spells");
                CMGUI.Box(OnRenderSpells);
            }, GUILayout.MinWidth(200));
            
            CMGUI.HorizontalSpace(40);
            
            CMGUI.Vertical(() =>
            {
                CMGUI.NTitle("CraftMaster.UI.SelectedSpell");
                CMGUI.Box(OnRenderSpellInfo);
            }, GUILayout.MinWidth(400));
            
            CMGUI.HorizontalSpace(40);
            
            CMGUI.Vertical(() =>
            {
                CMGUI.NTitle("CraftMaster.UI.WandSetting");
                CMGUI.Box(OnRenderBuildState);
            }, GUILayout.MinWidth(300));

            CMGUI.FlexibleSpace();
        });
    }
}