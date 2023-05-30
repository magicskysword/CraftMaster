using Kingmaker.EntitySystem.Entities;
using Newtonsoft.Json;

namespace CraftMaster.Builder;

public abstract class DynamicBuilder
{
    /// <summary>
    /// 唯一ID
    /// </summary>
    [JsonProperty] 
    public string Guid { get; set; }

    public DynamicBuilder()
    {
        Guid = System.Guid.NewGuid().ToString("N");
    }
    
    
    public abstract bool IsValid();
    public abstract void OnValidate();
    public abstract int GetRawCost();
    public abstract int GetBuildPoint();
    public abstract int GetCheckDC();
}