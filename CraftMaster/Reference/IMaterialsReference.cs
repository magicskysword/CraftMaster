namespace CraftMaster.Reference;

public interface IMaterialsReference
{
    public OrderedDictionary<string, string[]> Materials { get; }
}