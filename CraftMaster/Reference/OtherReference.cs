using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items;
using Kingmaker.Items;

namespace CraftMaster.Reference;

public class OtherReference
{
    public static readonly string GoldCoinItem = "f2bc0997c24e573448c6c91d2be88afa";
    public static readonly string AcidArrowAbility = "9a46dfd390f943647ab4395fc997936d";
    public static readonly string FlameStrike = "f9910c76efc34af41b6e43d5d8752f0f";
    public static readonly string FireBall = "2d81362af43aeac4387a3d4fced489c3";
    public static readonly string IceStorm = "fcb028205a71ee64d98175ff39a0abf9";
    public static readonly string CallLightning = "2a9ef0e0b5822a24d88b16673a267456";
    public static readonly string LightningBolt = "d2cff9243a7ee804cb6d5be47af30c73";
    /// <summary>
    /// 猫之轻灵
    /// </summary>
    public static readonly string CatsGrace = "de7a025d48ad5da4991e7d3c682cf69d";
    /// <summary>
    /// 加速术
    /// </summary>
    public static readonly string Haste = "486eaff58293f6441a5c2759c4872f98";

    public static ItemEntity CreatMoneyEntity(int money)
    {
        var blueprint = ResourcesLibrary.TryGetBlueprint<BlueprintItem>(GoldCoinItem)!;
        var item = new ItemEntitySimple(blueprint);
        item.SetCount(money);
        return item;
    }
}