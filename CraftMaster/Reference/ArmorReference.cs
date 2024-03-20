using System;

namespace CraftMaster.Reference;

public class ArmorReference : IEnchantmentReference, IEnhancementReference, IMaterialsReference
{
    public void Init()
    {
        InitArmorEnchantmentData();
    }

    public OrderedDictionary<string, EnchantmentGroup> EnchantmentGroups { get; } = new();

    public string[] Enhancements { get; } = new[]
    {
        "a9ea95c5e02f9b7468447bc1010fe152",
        "758b77a97640fd747abf149f5bf538d0",
        "9448d3026111d6d49b31fc85e7f3745a",
        "eaeb89df5be2b784c96181552414ae5a",
        "6628f9d77fd07b54c911cd8930c0d531",
        "de15272d1f4eb7244aa3af47dbb754ef"
    };

    /// <summary>
    /// 防具材质
    /// </summary>
    public OrderedDictionary<string, string[]> Materials { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        // 普通
        { "Normal", Array.Empty<string>() },
        // 秘银
        {
            "Mithral", new[]
            {
                "7b95a819181574a4799d93939aa99aff",
            }
        },
        // 精金
        {
            "Adamantine", new[]
            {
                "933456ff83c454146a8bf434e39b1f93",
            }
        },
    };

    private void InitArmorEnchantmentData()
    {
        var enhancementGroup = new EnchantmentGroup()
        {
            Key = "Enhancement",
            NameStringKey = "CraftMaster.EnchantmentGroup.Enhancement",
            AllowMultiple = false
        };
        enhancementGroup.AddEnchantment(new()
        {
            Key = "Enhancement1",
            Guid = "a9ea95c5e02f9b7468447bc1010fe152",
            Point = 1,
            CasterLevel = 3,
        });
        enhancementGroup.AddEnchantment(new()
        {
            Key = "Enhancement2",
            Guid = "758b77a97640fd747abf149f5bf538d0",
            Point = 2,
            CasterLevel = 6,
        });
        enhancementGroup.AddEnchantment(new()
        {
            Key = "Enhancement3",
            Guid = "9448d3026111d6d49b31fc85e7f3745a",
            Point = 3,
            CasterLevel = 9,
        });
        enhancementGroup.AddEnchantment(new()
        {
            Key = "Enhancement4",
            Guid = "eaeb89df5be2b784c96181552414ae5a",
            Point = 4,
            CasterLevel = 12,
        });
        enhancementGroup.AddEnchantment(new()
        {
            Key = "Enhancement5",
            Guid = "6628f9d77fd07b54c911cd8930c0d531",
            Point = 5,
            CasterLevel = 15,
        });
        
        this.AddEnchantmentGroup(enhancementGroup);
    }

    /// <summary>
    /// 轻型盔甲
    /// </summary>
    public OrderedDictionary<string, string> LightArmorTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Haramaki", "9e27c61d6eac09e4ba98980e6df3645c" },
        { "Padded", "598540b85673d984a8d45effcadda93f" },
        { "Leather", "9f76e9a3353e914479c5ddb4b4a82fb4" },
        { "ChainShirt", "c65f6fc979d5556489b20e478189cbdd" },
    };
    
    /// <summary>
    /// 中型盔甲
    /// </summary>
    public OrderedDictionary<string, string> MiddleArmorTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Hide", "385be51e5706a55418384f70d8341371" },
        { "Scalemail", "d7963e1fcf260c148877afd3252dbc91" },
        { "Breastplate", "9809987cc12d94545a64ff20e6fdb216" },
        { "Chainmail", "02e9f83be5d1c5d4e927b5c44ed34840" },
    };
    
    /// <summary>
    /// 重型盔甲
    /// </summary>
    public OrderedDictionary<string, string> HeavyArmorTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Banded", "1638fa11f5af1814191cf6c05cdcf2b6" },
        { "Halfplate", "ed6bbd7ecd050c04690fe11d4c3b3f7d" },
        { "Fullplate", "559b0b6f194656c428c403a000ceee78" },
    };
    
    /// <summary>
    /// 坐骑盔甲
    /// </summary>
    public OrderedDictionary<string, string> BardingArmorTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "HideBarding", "4a0838d1c48a9cc459b287e6a04f17f9" },
        { "FullBarding", "102cda1cf4b6d9e4289b538bde6dba0d" },
        { "ScaleBarding", "292b8263268e5464e8d015de144ceee0" },
        { "LeatherBarding", "02f8d3ca984dbba43808ef34fdd7b4cf" },
        { "ChainmailBarding", "4a95066d4dc2d79489a63a9bb84ad4a8" },
        { "ChainshirtBarding", "6a619d6bf950f6c478e36d4a1d01cc0d" },
    };
    
    /// <summary>
    /// 根据防具类型ID寻找原型GUID
    /// </summary>
    /// <param name="equipType"></param>
    /// <returns></returns>
    public string FindPrototype(string equipType)
    {
        string prototype;
        if (LightArmorTypes.TryGetValue(equipType, out prototype))
        {
            return prototype;
        }

        if (MiddleArmorTypes.TryGetValue(equipType, out prototype))
        {
            return prototype;
        }
        
        if (HeavyArmorTypes.TryGetValue(equipType, out prototype))
        {
            return prototype;
        }
        
        if (BardingArmorTypes.TryGetValue(equipType, out prototype))
        {
            return prototype;
        }

        Main.Logger.Error($"Armor type : {equipType} not found!");
        return null;
    }
}