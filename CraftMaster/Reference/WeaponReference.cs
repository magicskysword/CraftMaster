using System;
using System.Collections.Generic;
using CraftMaster.View;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Enums;
using Kingmaker.Items;
using UnityEngine;

namespace CraftMaster.Reference;

public class WeaponReference : IEnchantmentReference, IEnhancementReference, IMaterialsReference
{
    public void Init()
    {
        InitWeaponEnchantmentData();
    }

    public OrderedDictionary<string, EnchantmentGroup> EnchantmentGroups { get; } = new();

    /// <summary>
    /// 增强加值附魔 +1 ~ +6
    /// </summary>
    public string[] Enhancements { get; } = new []
    {
        "d42fc23b92c640846ac137dc26e000d4",
        "eb2faccc4c9487d43b3575d7e77ff3f5",
        "80bb8a737579e35498177e1e3c75899b",
        "783d7d496da6ac44f9511011fc5f1979",
        "bdba267e951851449af552aa9f9e3992",
        "0326d02d2e24d254a9ef626cc7a3850f",
    };

    /// <summary>
    /// 武器材质
    /// </summary>
    public OrderedDictionary<string, string[]> Materials { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        // 普通
        { "Normal", Array.Empty<string>() },
        // 精制品
        { "Masterwork", new []
        {
            "6b38844e2bffbac48b63036b66e735be"
        } },
        // 寒铁
        { "ColdIron", new []
        {
            "e5990dc76d2a613409916071c898eee8",
            "6b38844e2bffbac48b63036b66e735be"
        } },
        // 秘银
        { "Mithral", new []
        {
            "0ae8fc9f2e255584faf4d14835224875",
            "6b38844e2bffbac48b63036b66e735be"
        } },
        // 精金
        { "Adamantine", new []
        {
            "ab39e7d59dd12f4429ffef5dca88dc7b",
            "6b38844e2bffbac48b63036b66e735be"
        } },
    };

    /// <summary>
    /// 轻型武器
    /// </summary>
    public OrderedDictionary<string, string> SimpleWeaponTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        #region 轻型
        
        // 匕首
        {"Dagger", "aa514dbf4c3d61f4e9c0738bd4d373cb"},
        // 轻型硬头锤
        {"LightMace", "355659b342d28e641bf29fe6d8084d19"},
        // 拳刃
        {"PunchingDagger", "43ff56218554d8547840e7659816db5e"},
        // 单镰
        {"Sickle", "bfe24b51e4d943a42b0976aaee7e1b7c"},

        #endregion

        #region 单手

        // 木棒
        {"Club", "07863393521453a4da9e98626531eb5f"},
        // 重型硬头锤
        {"HeavyMace", "766689ccb57d9eb4f8fc88e2e85a919f"},
        // 短矛
        {"ShortSpear", "926d02c8af0352b46874791d4de9764f"},

        #endregion

        #region 双手
        
        // 长矛
        {"LongSpear", "f28f6031c2908d84d945865a80f67177"},
        // 木棍
        {"Quarterstaff", "ada85dae8d12eda4bbe6747bb8b5883c"},
        // 矛
        {"Spear", "4abc27631e2894f4b8b70270e31694f1"},

        #endregion

        #region 远程

        // 飞镖
        {"Dart", "a20308f698383b744bb459847333d64b"},
        // 重弩
        {"HeavyCrossbow", "19a5092244dcf99478dcd73c974828b1"},
        // 标枪
        {"Javelin", "c73e20a04b9cae74ca0d5affffeebd34"},
        // 轻弩
        {"LightCrossbow", "511c97c1ea111444aa186b1a58496664"},
        // 投石索
        {"SlingStaff", "dda1a4f8cbf8ad34ca7845ca17313e86"},

        #endregion
    };

    /// <summary>
    /// 军用武器
    /// </summary>
    public OrderedDictionary<string, string> MartialWeaponTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        #region 轻型

        // 手斧
        {"HandAxe", "238ac092fad27144c9514f82917fbec9"},
        // 十手
        {"Jutte", "c73e20a04b9cae74ca0d5affffeebd34"},
        // 反曲刀
        {"Kukri", "3125ac6c819db9f4697312710699b637"},
        // 轻锤
        {"LightHammer", "79e044277b90a05448a71ae3bcaf581a"},
        // 轻稿
        {"LightPick", "f20e85bd5bb8dc74785afc129284bcda"},
        // 短剑
        {"Shortsword", "f717b39c351b8b44388c471d4d272f4e"},
        // 星刃
        {"StarKnife", "d19662d357d752447a801951b7bec798"},
        // 飞斧
        {"ThrowingAxe", "b9ed902d07b622b4f8ec223808f754c1"},

        #endregion

        #region 单手

        // 战斧
        {"Battleaxe", "6080738f7e97b5646980a0efad2da676"},
        // 长剑
        {"Longsword", "6fd0a849531617844b195f452661b2cd"},
        // 细剑
        {"Rapier", "1546a05eb151d424eb9132832d5511bb"},
        // 弯刀
        {"Scimitar", "5363519e36752d84698e03a86fb33afb"},
        // 三叉戟
        {"Trident", "231f325de2b32dd4585707f8d0c87af3"},
        // 战锤
        {"Warhammer", "3f35d5c01e11d564daa59938dec3db4b"},
        // 轻型链枷
        {"Flail", "8bdfa4f81bbc7b540919d770095720be"},

        #endregion

        #region 双手

        // 新月战斧
        {"Bardiche", "5bfdaaa6416cc604cba121d003db11ef"},
        // 裂地锤
        {"EarthBreaker", "fc47ddc975f1f804bbef320e1c574cd7"},
        // 弯刃大刀
        {"Falchion", "0f0e6834458d01049a408cd304053b1b"},
        // 大砍刀
        {"Glaive", "f83415c0e7ea1994d8a7f3dec8f5a861"},
        // 巨斧
        {"Greataxe", "6efea466862f014469cec6c3f2b85cb7"},
        // 巨木棒
        {"Greatclub", "c926ffbdccc4d124c8e8dedfe2e6f499"},
        // 巨剑
        {"Greatsword", "2fff2921851568a4d80ed52f76cccdb6"},
        // 重型链枷
        {"HeavyFlail", "7f7c8e1e4fdd99e438b30ed9622e9e3f"},
        // 巨镰
        {"Scythe", "1052a1f7128861942aa0c2ee6078531e"},
        
        #endregion

        #region 远程

        // 复合长弓
        {"CompositeLongbow", "d23f6f5d3cbf715488b1f73e130dca99"},
        // 复合短弓
        {"CompositeShortbow", "2ae1cfe7ea8e60d459948193b6a0f7fe"},
        // 长弓
        {"Longbow", "201f6150321e09048bd59e9b7f558cb0"},
        // 短弓
        {"Shortbow", "6d40d84e239bdf345b349ff52e3c00a9"},
        
        #endregion
    };

    /// <summary>
    /// 异种武器
    /// </summary>
    public OrderedDictionary<string, string> ExoticWeaponTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        #region 轻型

        // 单镰
        {"Kama", "46e685e26290d2c468d96439198e6896"},
        // 双截棍
        {"Nunchaku", "c0ce1d36d1d3ae246a7587bb17296f07"},
        // 笔架叉
        {"Sai", "d60d0d4c78570ef408a0402f9d4313a6"},
        
        #endregion

        #region 单手

        // 重剑
        {"BastardSword", "07531989333442348b7d0102b24af236"},
        // 决斗剑
        {"DuelingSword", "4697667a33a6774489e5265a955675a5"},
        // 钉斧
        {"Tongi", "b47455bac4f039747ad5e4ddc4e981a4"},
        
        #endregion

        #region 双手
        
        // 半身人投石杖
        {"SlingStaff", "dda1a4f8cbf8ad34ca7845ca17313e86"},
        // 矮人重斧
        {"DwarvenWaraxe", "30711e40771796340ac67172abfd3279"},
        // 反曲剑
        {"Falcata", "ee28827188e12a64ba75222b37ac8092"},
        // 矮人矛斧
        {"DwarvenUrgrosh", "c20f347c84b6605479c9a7b28ccb236b"},
        // 精灵曲刃
        {"ElvenCurveBlade", "f58e421cb8b4ed64ba195123df754055"},
        // 侏儒钩锤
        {"GnomeHookedHammer", "8998da2cfe0884f47943bd28823c3a51"},
        // 兽人双头斧
        {"DoubleAxe", "1d9acaa3c344c1244bcea18095652955"},
        // 斩矛
        {"Fauchard", "c91f331cdf7e331468490323a2e1613d"},
        // 双头剑
        {"DoubleSword", "b665770f14e49bc49999d7c3c11c1d61"},
        // 
        {"Estoc", "4c863a47d69b63647b3b16bbb01d5ba8"},
        
        #endregion
    };

    private void InitWeaponEnchantmentData()
    {
        #region 增强加值

        var enhancementGroup = new EnchantmentGroup() {
            Key = "Enhancement",
            NameStringKey = "CraftMaster.EnchantmentGroup.Enhancement",
            AllowMultiple = false
        };
        
        enhancementGroup.AddEnchantment(new() {
            Key = "Enhancement1",
            Guid = "d42fc23b92c640846ac137dc26e000d4",
            Point = 1,
            CasterLevel = 3, });
        enhancementGroup.AddEnchantment(new() {
            Key = "Enhancement2",
            Guid = "eb2faccc4c9487d43b3575d7e77ff3f5",
            Point = 2,
            CasterLevel = 6, });
        enhancementGroup.AddEnchantment(new() {
            Key = "Enhancement3",
            Guid = "80bb8a737579e35498177e1e3c75899b",
            Point = 3,
            CasterLevel = 9, });
        enhancementGroup.AddEnchantment(new() {
            Key = "Enhancement4",
            Guid = "783d7d496da6ac44f9511011fc5f1979",
            Point = 4,
            CasterLevel = 12, });
        enhancementGroup.AddEnchantment(new() {
            Key = "Enhancement5",
            Guid = "bdba267e951851449af552aa9f9e3992",
            Point = 5,
            CasterLevel = 15, });
        
        this.AddEnchantmentGroup(enhancementGroup);

        #endregion

        #region +1附魔 破敌

        var baneGroup = new EnchantmentGroup() {
            Key = "Bane",
            NameStringKey = "CraftMaster.EnchantmentGroup.Bane",
            AllowMultiple = true
        };
        
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneFey",
            Guid = "b6948040cdb601242884744a543050d4",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BanePlant",
            Guid = "0b761b6ed6375114d8d01525d44be5a9",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneAnimal",
            Guid = "78cf9fabe95d3934688ea898c154d904",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneVermin",
            Guid = "c3428441c00354c4fabe27629c6c64dd",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneUndead",
            Guid = "eebb4d3f20b8caa43af1fed8f2773328",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneDragon",
            Guid = "e5cb46a0a658b0a41854447bea32d2ee",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneLiving",
            Guid = "e1d6f5e3cd3855b43a0cb42f6c747e1c",
            Point = 1,
            CasterLevel = 8,
        });
        // baneGroup.AddEnchantment(new()
        // {
        //     Key = "BaneNonHuman",
        //     Guid = "eb2b2e9f741e4cc18edc47cbb1387e02",
        //     Point = 1,
        //     CasterLevel = 8,
        // });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneOrcGoblin",
            Guid = "0391d8eae25f39a48bcc6c2fc8bf4e12",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneConstruct",
            Guid = "73d30862f33cc754bb5a5f3240162ae6",
            Point = 1,
            CasterLevel = 8,
        });
        // baneGroup.AddEnchantment(new()
        // {
        //     Key = "BaneVermin1d8",
        //     Guid = "e535fc007d0d7e74da45021d4607e607",
        //     Point = 1,
        //     CasterLevel = 8,
        // });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneAberration",
            Guid = "ee71cc8848219c24b8418a628cc3e2fa",
            Point = 1,
            CasterLevel = 8,
        });
        // baneGroup.AddEnchantment(new()
        // {
        //     Key = "BaneEverything",
        //     Guid = "1a93ab9c46e48f3488178733be29342a",
        //     Point = 1,
        //     CasterLevel = 8,
        // });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneLycanthrope",
            Guid = "188efcfcd9938d44e9561c87794d17a8",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneOutsiderEvil",
            Guid = "20ba9055c6ae1e44ca270c03feacc53b",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneMagicalBeast",
            Guid = "97d477424832c5144a9413c64d818659",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneOutsiderGood",
            Guid = "a876de94b916b7249a77d090cb9be4f3",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneOutsiderLawful",
            Guid = "3a6f564c8ea2d1941a45b19fa16e59f5",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneOutsiderNeutral",
            Guid = "4e30e79c500e5af4b86a205cc20436f2",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneOutsiderChaotic",
            Guid = "234177d5807909f44b8c91ed3c9bf7ac",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneHumanoidGiant",
            Guid = "dcecb5f2ffacfd44ead0ed4f8846445d",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneLongshankEnchant",
            Guid = "92a1f5db1a03c5b468828c25dd375806",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneHumanoidReptilian",
            Guid = "c4b9cce255d1d6641a6105a255934e2e",
            Point = 1,
            CasterLevel = 8,
        });
        baneGroup.AddEnchantment(new()
        {
            Key = "BaneMonstrousHumanoid",
            Guid = "c5f84a79ad154c84e8d2e9fe0dd49350",
            Point = 1,
            CasterLevel = 8,
        });
        // baneGroup.AddEnchantment(new()
        // {
        //     Key = "BaneOfTheLivingEnchantment",
        //     Guid = "a6c6bab27c8572246afd722f6be0ee2a",
        //     Point = 1,
        //     CasterLevel = 8,
        // });
        
        this.AddEnchantmentGroup(baneGroup);

        #endregion

        #region 阵营附魔

        var alignmentGroup = new EnchantmentGroup() {
            Key = "Alignment",
            NameStringKey = "CraftMaster.EnchantmentGroup.Alignment",
            AllowMultiple = false
        };
        
        alignmentGroup.AddEnchantment(new() {
            Key = "GoodAligned",
            Guid = "326da486cd9077242a0e25df7eb7cd78",
            Point = 1,
            CasterLevel = 5,
            AddApplyChecker = EnchantmentData.CheckAlignmentGood,
        });
        alignmentGroup.AddEnchantment(new() {
            Key = "EvilAligned",
            Guid = "785f58ae4af37d041a6924634b0f238f",
            Point = 1,
            CasterLevel = 5,
            AddApplyChecker = EnchantmentData.CheckAlignmentEvil,
        });
        alignmentGroup.AddEnchantment(new() {
            Key = "LawfulAligned",
            Guid = "76c7f6e9f0618a64fa21905687e36133",
            Point = 1,
            CasterLevel = 5,
            AddApplyChecker = EnchantmentData.CheckAlignmentLawful,
        });
        alignmentGroup.AddEnchantment(new() {
            Key = "ChaoticAligned",
            Guid = "5781c3a3255f5be4a9f94c6faf0ac0c3",
            Point = 1,
            CasterLevel = 5,
            AddApplyChecker = EnchantmentData.CheckAlignmentChaotic
        });
        alignmentGroup.AddEnchantment(new() {
            Key = "Holy",
            Guid = "28a9964d81fedae44bae3ca45710c140",
            Point = 2,
            CasterLevel = 7,
            AddApplyChecker = EnchantmentData.CheckAlignmentGood,
        });
        alignmentGroup.AddEnchantment(new() {
            Key = "Unholy",
            Guid = "d05753b8df780fc4bb55b318f06af453",
            Point = 2,
            CasterLevel = 7,
            AddApplyChecker = EnchantmentData.CheckAlignmentEvil,
        });
        alignmentGroup.AddEnchantment(new() {
            Key = "Axiomatic",
            Guid = "0ca43051edefcad4b9b2240aa36dc8d4",
            Point = 2,
            CasterLevel = 7,
            AddApplyChecker = EnchantmentData.CheckAlignmentLawful,
        });
        alignmentGroup.AddEnchantment(new() {
            Key = "Anarchic",
            Guid = "57315bc1e1f62a741be0efde688087e9",
            Point = 2,
            CasterLevel = 7,
            AddApplyChecker = EnchantmentData.CheckAlignmentChaotic
        });

        this.AddEnchantmentGroup(alignmentGroup);

        #endregion

        #region 酸附魔

        var acidGroup = new EnchantmentGroup() {
            Key = "Acid",
            NameStringKey = "CraftMaster.EnchantmentGroup.Acid",
            AllowMultiple = true
        };
        acidGroup.AddEnchantment(new() {
            Key = "Corrosive",
            Guid = "633b38ff1d11de64a91d490c683ab1c8",
            Point = 1,
            CasterLevel = 10,
            AddApplyChecker = EnchantmentData.GetMagicChecker(OtherReference.AcidArrowAbility)
        });
        acidGroup.AddEnchantment(new() {
            Key = "CorrosiveBurst",
            Guid = "0cf34703e67e37b40905845ca14b1380",
            Point = 2,
            CasterLevel = 12,
            AddApplyChecker = EnchantmentData.GetMagicChecker(OtherReference.AcidArrowAbility),
            RequiredEnchantment = new []{"Corrosive"},
        });
        
        this.AddEnchantmentGroup(acidGroup);

        #endregion
        
        #region 焰附魔

        var flameGroup = new EnchantmentGroup() {
            Key = "Flame",
            NameStringKey = "CraftMaster.EnchantmentGroup.Flame",
            AllowMultiple = true
        };
        flameGroup.AddEnchantment(new() {
            Key = "Flaming",
            Guid = "30f90becaaac51f41bf56641966c4121",
            Point = 1,
            CasterLevel = 10,
            AddApplyChecker = EnchantmentData.Or(
                EnchantmentData.GetMagicChecker(OtherReference.FlameStrike),
                EnchantmentData.GetMagicChecker(OtherReference.FireBall))
        });
        flameGroup.AddEnchantment(new() {
            Key = "FlamingBurst",
            Guid = "3f032a3cd54e57649a0cdad0434bf221",
            Point = 2,
            CasterLevel = 12,
            AddApplyChecker = EnchantmentData.Or(
                EnchantmentData.GetMagicChecker(OtherReference.FlameStrike),
                EnchantmentData.GetMagicChecker(OtherReference.FireBall)),
            RequiredEnchantment = new []{"Flaming"},
        });
        flameGroup.AddEnchantment(new() {
            Key = "Igniting",
            Guid = "cd344d5e4cdd8254e97943b2dd358ce5",
            Point = 2,
            CasterLevel = 12,
            AddApplyChecker = EnchantmentData.Or(
                EnchantmentData.GetMagicChecker(OtherReference.FlameStrike),
                EnchantmentData.GetMagicChecker(OtherReference.FireBall)),
            RequiredEnchantment = new []{"Flaming"},
        });
        this.AddEnchantmentGroup(flameGroup);

        #endregion
        
        #region 冰附魔

        var iceGroup = new EnchantmentGroup() {
            Key = "Ice",
            NameStringKey = "CraftMaster.EnchantmentGroup.Ice",
            AllowMultiple = true
        };
        iceGroup.AddEnchantment(new() {
            Key = "Frost",
            Guid = "421e54078b7719d40915ce0672511d0b",
            Point = 1,
            CasterLevel = 10,
            AddApplyChecker = EnchantmentData.GetMagicChecker(OtherReference.IceStorm)
        });
        iceGroup.AddEnchantment(new() {
            Key = "IcyBurst",
            Guid = "564a6924b246d254c920a7c44bf2a58b",
            Point = 2,
            CasterLevel = 12,
            AddApplyChecker = EnchantmentData.GetMagicChecker(OtherReference.IceStorm),
            RequiredEnchantment = new []{"Frost"},
        });
        this.AddEnchantmentGroup(iceGroup);

        #endregion
        
        #region 电附魔

        var lightningGroup = new EnchantmentGroup() {
            Key = "Lightning",
            NameStringKey = "CraftMaster.EnchantmentGroup.Lightning",
            AllowMultiple = true
        };
        lightningGroup.AddEnchantment(new() {
            Key = "Shock",
            Guid = "7bda5277d36ad114f9f9fd21d0dab658",
            Point = 1,
            CasterLevel = 10,
            AddApplyChecker = EnchantmentData.Or(
                EnchantmentData.GetMagicChecker(OtherReference.CallLightning),
                EnchantmentData.GetMagicChecker(OtherReference.LightningBolt)),
        });
        lightningGroup.AddEnchantment(new() {
            Key = "ShockingBurst",
            Guid = "914d7ee77fb09d846924ca08bccee0ff",
            Point = 2,
            CasterLevel = 12,
            AddApplyChecker = EnchantmentData.Or(
                EnchantmentData.GetMagicChecker(OtherReference.CallLightning),
                EnchantmentData.GetMagicChecker(OtherReference.LightningBolt)),
            RequiredEnchantment = new []{"Shock"},
        });
        this.AddEnchantmentGroup(lightningGroup);

        #endregion

        #region 特殊

        var specialGroup = new EnchantmentGroup() {
            Key = "Special",
            NameStringKey = "CraftMaster.EnchantmentGroup.Special",
            AllowMultiple = true
        };
        specialGroup.AddEnchantment(new() {
            Key = "Heartseeker",
            Guid = "e252b26686ab66241afdf33f2adaead6",
            Point = 1,
            CasterLevel = 7,
        });
        specialGroup.AddEnchantment(new() {
            Key = "Agile",
            Guid = "a36ad92c51789b44fa8a1c5c116a1328",
            Point = 1,
            CasterLevel = 7,
            AddApplyChecker = EnchantmentData.GetMagicChecker(OtherReference.CatsGrace)
        });
        specialGroup.AddEnchantment(new() {
            Key = "GhostTouch",
            Guid = "47857e1a5a3ec1a46adf6491b1423b4f",
            Point = 1,
            CasterLevel = 9,
        });
        specialGroup.AddEnchantment(new() {
            Key = "Keen",
            Guid = "102a9c8c9b7a75e4fb5844e79deaf4c0",
            Point = 1,
            CasterLevel = 10,
        });
        specialGroup.AddEnchantment(new() {
            Key = "Speed",
            Guid = "f1c0c50108025d546b2554674ea1c006",
            Point = 3,
            CasterLevel = 7,
            AddApplyChecker = EnchantmentData.GetMagicChecker(OtherReference.Haste)
        });
        specialGroup.AddEnchantment(new() {
            Key = "BrilliantEnergy",
            Guid = "66e9e299c9002ea4bb65b6f300e43770",
            Point = 4,
            CasterLevel = 16,
        });

        this.AddEnchantmentGroup(specialGroup);

        #endregion
    }

    /// <summary>
    /// 根据武器类型ID寻找原型GUID
    /// </summary>
    /// <param name="weaponType"></param>
    /// <returns></returns>
    public string FindWeaponPrototype(string weaponType)
    {
        string prototype;
        if (SimpleWeaponTypes.TryGetValue(weaponType, out prototype))
        {
            return prototype;
        }

        if (MartialWeaponTypes.TryGetValue(weaponType, out prototype))
        {
            return prototype;
        }
        
        if (ExoticWeaponTypes.TryGetValue(weaponType, out prototype))
        {
            return prototype;
        }

        Main.Logger.Error($"Weapon type : {weaponType} not found!");
        return null;
    }
}