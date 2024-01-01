using System.Collections.Generic;

namespace CraftMaster.Reference;

public interface IEnchantmentReference
{
    public OrderedDictionary<string, EnchantmentGroup> EnchantmentGroups { get; }
    string FindPrototype(string equipType);
}