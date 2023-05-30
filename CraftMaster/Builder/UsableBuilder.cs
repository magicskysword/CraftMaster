using Newtonsoft.Json;

namespace CraftMaster.Builder;

public abstract class UsableBuilder : DynamicBuilder
{
    [JsonProperty]
    public string TagAbility { get; set; }
    [JsonProperty]
    public int SpellLevel { get; set; }
    [JsonProperty]
    public int CasterLevel { get; set; }
    [JsonProperty]
    public int DC { get; set; }
    [JsonProperty]
    public string PropertyGuid { get; set; }
}