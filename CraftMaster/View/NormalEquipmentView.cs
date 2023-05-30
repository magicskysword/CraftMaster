using System.Collections.Generic;
using CraftMaster.Builder;
using CraftMaster.Project;

namespace CraftMaster.View;

public class EquipTypeView
{
    public string NameKey { get; }
    public OrderedDictionary<string, string> EquipTypes { get; }
    
    public EquipTypeView(string nameKey, OrderedDictionary<string, string> equipTypes)
    {
        NameKey = nameKey;
        EquipTypes = equipTypes;
    }
}

public abstract class NormalEquipmentView : BaseView
{
    public EquipBuilder Builder { get; set; }
    public abstract IEnumerable<EquipTypeView> EquipTypeEntry { get; }
    public abstract IEnumerable<KeyValuePair<string, string[]>> ReferenceMaterials { get; }

    public abstract string GetTypeNameByGuid(string guid);
    public abstract void ResetBuilder();

    public abstract void AddBuildProject(CraftPart craftPart, int buildPoint, int spendMoney, int checkDC);

    public abstract string GetTooltipByGuid(string guid);

    protected override void OnReset()
    {
        Builder = null;
    }
}