using System;
using System.Collections.Generic;
using Kingmaker.UnitLogic;
using Newtonsoft.Json;

namespace CraftMaster.Builder;

public class BuilderPart : UnitPart
{
    /// <summary>
    /// 构建器缓存
    /// </summary>
    [JsonProperty]
    public Dictionary<string, DynamicBuilder> Builders = new(StringComparer.OrdinalIgnoreCase);
    
    /// <summary>
    /// 实体与构建器的映射
    /// </summary>
    [JsonProperty]
    public Dictionary<string, string> EntityMapping = new(StringComparer.OrdinalIgnoreCase);

    public void AddBuilder(DynamicBuilder builder)
    {
        Builders[builder.Guid] = builder;
    }
    
    public void AddMapping(string entityGuid, string blueprintGuid)
    {
        EntityMapping[entityGuid] = blueprintGuid;
    }

    public bool TryGetBuilder(string builderKey, out DynamicBuilder builder)
    {
        return Builders.TryGetValue(builderKey, out builder);
    }

    public bool TryGetBuilder<T>(string builderKey, out T builder) where T : DynamicBuilder
    {
        if (Builders.TryGetValue(builderKey, out var b) && b is T tBuilder)
        {
            builder = tBuilder;
            return true;
        }

        builder = null;
        return false;
    }
    
    public bool TryGetMappingBuilder(string entityGuid, out DynamicBuilder builder)
    {
        if (EntityMapping.TryGetValue(entityGuid, out var blueprintGuid))
        {
            return Builders.TryGetValue(blueprintGuid, out builder);
        }

        builder = null;
        return false;
    }
    
    public bool TryGetMappingBuilder<T>(string entityGuid, out T builder) where T : DynamicBuilder
    {
        if (EntityMapping.TryGetValue(entityGuid, out var builderGuid))
        {
            if (Builders.TryGetValue(builderGuid, out var b) && b is T tBuilder)
            {
                builder = tBuilder;
                return true;
            }
        }

        builder = null;
        return false;
    }

    public bool HasBuilder(string builderGuid)
    {
        return Builders.ContainsKey(builderGuid);
    }
}