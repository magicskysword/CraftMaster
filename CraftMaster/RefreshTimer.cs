using System;

namespace CraftMaster;

public class RefreshTimer
{
    public DateTime CurrentTime { get; set; } = DateTime.MinValue;
    public float RefreshTime { get; set; } = 0.5f;
    public bool IsDirty { get; set; } = false;
    
    public bool CheckNeedRefresh()
    {
        var timeSpan = DateTime.Now - CurrentTime;
        if (timeSpan.TotalSeconds > RefreshTime)
        {
            CurrentTime = DateTime.Now;
            return true;
        }
        
        return false;
    }
}