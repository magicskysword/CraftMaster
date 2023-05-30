using System.Collections.Generic;

namespace CraftMaster.View;

public struct CheckerContext
{
    public bool IsEnable { get; set; } = true;
    public string Text { get; set; } = "";
    public List<string> Tooltips { get; set; } = new List<string>();
    public string Description { get; set; } = "";
    public string Tooltip => Tooltips.Count > 0 ? string.Join("\n", Tooltips) : string.Empty;
    
    public CheckerContext()
    {
        
    }
}