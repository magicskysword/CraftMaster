using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items;
using Kingmaker.Items;

namespace CraftMaster.Reference;

public class OtherReference
{
    public static string GoldCoinItem = "f2bc0997c24e573448c6c91d2be88afa";
    public static string AcidArrowAbility = "9a46dfd390f943647ab4395fc997936d";
    public static string FlameStrike = "f9910c76efc34af41b6e43d5d8752f0f";
    public static string FireBall = "2d81362af43aeac4387a3d4fced489c3";
    public static string IceStorm = "fcb028205a71ee64d98175ff39a0abf9";
    public static string CallLightning = "2a9ef0e0b5822a24d88b16673a267456";
    public static string LightningBolt = "d2cff9243a7ee804cb6d5be47af30c73";
    /// <summary>
    /// 猫之轻灵
    /// </summary>
    public static string CatsGrace = "de7a025d48ad5da4991e7d3c682cf69d";
    /// <summary>
    /// 加速术
    /// </summary>
    public static string Haste = "486eaff58293f6441a5c2759c4872f98";

    public static ItemEntity CreatMoneyEntity(int money)
    {
        var blueprint = ResourcesLibrary.TryGetBlueprint<BlueprintItem>(GoldCoinItem);
        var item = new ItemEntitySimple(blueprint);
        item.SetCount(money);
        return item;
    }
}