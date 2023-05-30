namespace CraftMaster.Reference;

public class ReferenceManager
{
    public static OtherReference Other = new OtherReference();
    public static WeaponReference Weapon = new WeaponReference();
    public static ArmorReference Armor = new ArmorReference();
    public static WandReference Wand = new WandReference();

    public static void Init()
    {
        Weapon.Init();
        Armor.Init();
    }
} 