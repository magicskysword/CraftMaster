using System.IO;
using Kingmaker.Blueprints.JsonSystem;
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
    
    public static T CopyNew<T>(T item) where T : DynamicBuilder
    {
        // 先序列化再反序列化
        var json = JsonConvert.SerializeObject(item);
        return JsonConvert.DeserializeObject<T>(json);
    }
}