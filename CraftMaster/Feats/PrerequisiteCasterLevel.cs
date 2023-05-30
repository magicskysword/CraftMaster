using System.Text;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Root.Strings;
using Kingmaker.Localization;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;

namespace CraftMaster.Feats;

public class PrerequisiteCasterLevel : Prerequisite
{
    private int _requestCasterLevel;
    
    public Prerequisite SetCasterLevel(int level)
    {
        _requestCasterLevel = level;
        return this;
    }
    
    public override string GetUITextInternal(UnitDescriptor unit)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(GameUtils.GetString("CraftMaster.Feat.PrerequisiteCasterLevel(requestLevel,curLevel)")
            .Format(_requestCasterLevel, unit.GetCasterLevel()));
        return stringBuilder.ToString();
    }

    public override bool CheckInternal(FeatureSelectionState selectionState, UnitDescriptor unit, LevelUpState state)
    {
        var casterLevel = 0;
        foreach (Spellbook spellbook in unit.Spellbooks)
        {
            if(spellbook.CasterLevel > casterLevel)
                casterLevel = spellbook.CasterLevel;
        }
        
        return casterLevel >= _requestCasterLevel;
    }
}