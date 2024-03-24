using CraftMaster.Feats;
using BlueprintCore.Blueprints.Configurators.Root;
using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CraftMaster.Builder;
using CraftMaster.Project;
using CraftMaster.Reference;
using Kingmaker;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using Kingmaker.UI;
using Kingmaker.UnitLogic;
using ModKit;
using UnityEngine;
using UnityModManagerNet;

namespace CraftMaster
{
    public static partial class Main
    {
        public static bool Enabled;
        public static UnityModManager.ModEntry ModEntry;
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static CraftMasterSetting Settings;
        public static bool NeedRefreshGUI;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                ModEntry = modEntry;
                Logger = modEntry.Logger;
                Settings = UnityModManager.ModSettings.Load<CraftMasterSetting>(modEntry);
                modEntry.OnGUI = OnGUI;
                modEntry.OnSaveGUI = OnSaveGUI;
                var harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll();
                Logger.Log("Finished patching.");
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to patch {e}");
            }

            return true;
        }

        private static void OnSaveGUI(UnityModManager.ModEntry obj)
        {
            Settings.Save(ModEntry);
        }

        public static int SelectedTab = 0;

        public static Locale CurrentLocale = Locale.zhCN;

        public static UnitEntityData SelectedUnit = null;

        private static List<NamedAction> CraftTabs = new List<NamedAction>();
        private static bool ShowSoundButton = false;

        private static void OnGUI(UnityModManager.ModEntry obj)
        {
            try
            {
                RenderModGUI();
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                ResetAll();
                throw;
            }
            finally
            {
                NeedRefreshGUI = false;
            }

            CMGUI.OnTooltip();
        }

        private static void RenderModGUI()
        {
            if (!GameUtils.IsGameStart())
            {
                CMGUI.NTitle("CraftMaster.UI.Unavailable");
            }
            else
            {
                RenderUnitSelection();

                if (SelectedUnit == null)
                    return;

                CMGUI.NTitleRaw(GameUtils.GetString("CraftMaster.UI.Money(money)").Format(Game.Instance.Player.Money));
                CMGUI.NTitleRaw(GameUtils.GetString("CraftMaster.UI.KnowledgeArcana(level)").Format(SelectedUnit.Stats.GetStat(StatType.SkillKnowledgeArcana).ModifiedValue));
                CMGUI.NTitleRaw(GameUtils.GetString("CraftMaster.UI.LoreReligion(level)").Format(SelectedUnit.Stats.GetStat(StatType.SkillLoreReligion).ModifiedValue));
                CMGUI.NTitleRaw(GameUtils.GetString("CraftMaster.UI.TotalDC(level)").Format(SelectedUnit.Stats.GetStat(StatType.SkillKnowledgeArcana).ModifiedValue + SelectedUnit.Stats.GetStat(StatType.SkillLoreReligion).ModifiedValue));
                CMGUI.NTitle("CraftMaster.UI.CraftType");

                CraftTabs.Clear();
                CraftTabs.Add(new NamedAction(GameUtils.GetString("CraftMaster.UI.CraftType_Normal"),
                    RenderCraftNormalEquipments));

                if (SelectedUnit.HasFeature(CraftMagicArmsAndArmor.FeatGuid) || Settings.IgnoreFeatureLimit)
                    CraftTabs.Add(new NamedAction(GameUtils.GetString("CraftMaster.UI.CraftType_Enchantments"),
                        RenderCraftEnchantments));

                if (SelectedUnit.HasFeature(CraftWand.FeatGuid) || Settings.IgnoreFeatureLimit)
                    CraftTabs.Add(
                        new NamedAction(GameUtils.GetString("CraftMaster.UI.CraftType_Wand"), RenderCraftWand));

                UI.TabBar(ref SelectedTab, null, CraftTabs.ToArray());

                RenderAllCraftProject();
            }

            RenderModSetting();
#if DEBUG
            ShowSoundButton = CMGUI.NToggleRaw("Sound", ShowSoundButton, 200);
            if (ShowSoundButton)
            {
                foreach (UISoundType uisoundType in (UISoundType[])Enum.GetValues(typeof(UISoundType)))
                {
                    GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
                    bool flag = GUILayout.Button("UI." + uisoundType, new[] { GUILayout.ExpandWidth(false) });
                    if (flag)
                    {
                        Game.Instance.UI.UISound.Play(uisoundType);
                    }
                    GUILayout.EndHorizontal();
                }
            }
#endif
        }

        private static void RenderUnitSelection()
        {
            var oldUnit = SelectedUnit;

            if (!Game.Instance.Player.AllCharacters.Contains(SelectedUnit) || SelectedUnit == null)
                SelectedUnit = Game.Instance.Player.MainCharacter;

            void RenderUnit(UnitEntityData unit, int curIndex, GUILayoutOption[] options)
            {
                if (CMGUI.NToggleButtonRaw(unit.CharacterName, SelectedUnit == unit, options))
                {
                    SelectedUnit = unit;
                }
            }

            if (GameUtils.IsPlayerInCapital())
            {
                CMGUI.Grid(Game.Instance.Player.AllCharacters.Where(unit => unit.CanCraft()), RenderUnit);
            }
            else
            {
                CMGUI.Grid(Game.Instance.Player.Party.Where(unit => unit.CanCraft()), RenderUnit);
            }


            if (SelectedUnit == null)
            {
                CMGUI.NTitle("CraftMaster.UI.NoUnit");
            }
            else
            {
                CMGUI.NTitleRaw(GameUtils.GetString("CraftMaster.UI.UnitName(name)")
                    .Format(SelectedUnit.CharacterName));
            }

            if (oldUnit != SelectedUnit)
            {
                ResetCraftState();
            }
        }

        private static void ResetAll()
        {
            SelectedUnit = null;
            ResetCraftState();
        }

        private static void ResetCraftState()
        {
            // 重置相关数据
            // 法杖
            WandBuilder = null;
            // 附魔
            WeaponEnchantmentView.Reset();
            // 装备
            NormalArmorView.Reset();
            NormalWeaponView.Reset();
        }

        private static int ProjectSortMode = 0;

        private static void RenderAllCraftProject()
        {
            CMGUI.Box("CraftMaster.UI.CurrentProject", () =>
            {
                var allProjects = CraftPartManager.GetAllCraftProjects();

                void RenderCraftProjectInternal(CraftProject craftProject)
                {
                    CMGUI.Horizontal(() =>
                    {
                        if (CMGUI.NButton("CraftMaster.UI.ProjectFirst", 50))
                            CraftPartManager.SetCraftProjectFirst(craftProject);

                        CMGUI.NLabelRaw(
                            GameUtils.GetString("CraftMaster.UI.ProjectName(name)")
                                .Format(craftProject.GetProjectName()),
                            GUILayout.MinWidth(250));
                        CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.ProjectAuthor(name)")
                            .Format(craftProject.Author.CharacterName));
                        CMGUI.FlexibleSpace();
                        if (craftProject.IsCrafting())
                        {
                            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.ProjectState_Crafting"));
                        }
                        else
                        {
                            CMGUI.NLabelRaw(GameUtils.GetString("CraftMaster.UI.ProjectState_Waiting"));
                        }

                        CMGUI.HorizontalSpace(10);
                        CMGUI.NLabelRaw(
                            GameUtils.GetString("CraftMaster.UI.ProjectCheckDC(checkDC)").Format(craftProject.CheckDC),
                            GUILayout.MinWidth(150));
                        CMGUI.HorizontalSpace(10);
                        CMGUI.NLabelRaw(
                            GameUtils.GetString("CraftMaster.UI.ProjectPoint(cur,need)")
                                .Format(craftProject.CurrentPoint, craftProject.TotalPoint),
                            GUILayout.MinWidth(150));
                        CMGUI.HorizontalSpace(10);
                        CMGUI.NLabelRaw(
                            GameUtils.GetString("CraftMaster.UI.ProjectLeftTime(time)")
                                .Format(craftProject.LeftTime.GetLeftTimeDescription()),
                            GUILayout.MinWidth(200));

                        if (CMGUI.NButton("CraftMaster.UI.ProjectCancel", 100))
                        {
                            CraftPartManager.CancelCraftProject(craftProject);
                        }
                    });
                }

                void RenderAllCraftProjectInternal()
                {
                    foreach (var project in allProjects)
                    {
                        CMGUI.VerticalSpace(2);
                        RenderCraftProjectInternal(project);
                    }
                }

                UI.TabBar(ref ProjectSortMode, null,
                    new(GameUtils.GetString("CraftMaster.UI.SortMode_Time"), () =>
                    {
                        allProjects = allProjects
                            .OrderBy(project => project.IsCrafting() ? 0 : 1)
                            .ThenBy(project => project.LeftTime);
                        RenderAllCraftProjectInternal();
                    }),
                    new(GameUtils.GetString("CraftMaster.UI.SortMode_Author"), () =>
                    {
                        allProjects = allProjects
                            .OrderBy(project => project.Author.CharacterName)
                            .ThenBy(project => project.IsCrafting() ? 0 : 1);
                        RenderAllCraftProjectInternal();
                    }));
            });
        }

        public static TimeSpan LastUpdateTime = TimeSpan.Zero;

        [HarmonyPatch(typeof(Player), "GameTime", MethodType.Setter)]
        static class PlayerGameTimeSettert_Patch
        {
            [HarmonyPostfix]
            private static void Postfix(Player __instance, TimeSpan value)
            {
                if (!GameUtils.IsGameStart())
                    return;

                if (value < LastUpdateTime)
                    return;

                if (value - LastUpdateTime > TimeSpan.FromHours(1))
                {
                    var passTime = value - LastUpdateTime;
#if DEBUG
                    Logger.Log($"GameTime Setter, LastUpdateTime: {LastUpdateTime} NewTime: {value} PassTime: {passTime}");
#endif
                    LastUpdateTime = value;
                    CraftPartManager.UpdateAllCraftProjects(passTime);
                }
            }
        }

        [HarmonyPatch(typeof(Player), "OnPostLoad")]
        static class PlayerPostLoad_Patch
        {
            private static void Postfix(Player __instance)
            {
                LastUpdateTime = __instance.GameTime;
#if DEBUG
                Logger.Log($"Load Game, LastUpdateTime: {LastUpdateTime}");
#endif
            }
        }

        [HarmonyPatch(typeof(UnityModManager.UI), "Update")]
        static class UnityModManagerUIUpdate_Patch
        {
            [HarmonyPostfix]
            private static void Postfix(UnityModManager.UI __instance, ref Rect ___mWindowRect,
                ref Vector2[] ___mScrollPosition, ref int ___tabId)
            {
                UI.ummRect = ___mWindowRect;
                UI.ummWidth = ___mWindowRect.width;
                UI.ummScrollPosition = ___mScrollPosition;
                UI.ummTabID = ___tabId;
            }
        }

        [HarmonyPatch(typeof(BlueprintsCache))]
        static class BlueprintsCaches_Patch
        {
            private static bool Initialized = false;

            [HarmonyPriority(Priority.First)]
            [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
            static void Init()
            {
                try
                {
                    if (Initialized)
                    {
                        Logger.Log("Already configured blueprints.");
                        return;
                    }

                    Initialized = true;

                    Logger.Log("Configuring blueprints.");

                    ReferenceManager.Init();
                    CraftMagicArmsAndArmor.Configure();
                    CraftPossessionOfConsummateSkill.Configure();
                    CraftWand.Configure();
                    CraftWandMythicFeat.Configure();
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to configure blueprints. {e}");
                }
            }
        }

        [HarmonyPatch(typeof(StartGameLoader))]
        static class StartGameLoader_Patch
        {
            private static bool Initialized = false;

            [HarmonyPatch(nameof(StartGameLoader.LoadPackTOC)), HarmonyPostfix]
            static void LoadPackTOC()
            {
                try
                {
                    if (Initialized)
                    {
                        Logger.Log("Already configured delayed blueprints.");
                        return;
                    }

                    Initialized = true;

                    RootConfigurator.ConfigureDelayedBlueprints();
                    var localizationFiles =
                        Directory.GetFiles(Path.Combine(ModEntry.Path, "Localization"), "*.json",
                            SearchOption.AllDirectories);
                    LocalizationTool.LoadLocalizationPacks(localizationFiles);
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to configure delayed blueprints. {e}");
                }
            }
        }
    }
}