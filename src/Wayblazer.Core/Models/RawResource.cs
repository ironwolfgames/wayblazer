namespace Wayblazer.Core.Models;

/// <summary>
/// Represents a raw material that can be gathered from the world.
/// </summary>
public class RawResource
{
    public string Name { get; }
    public ResourceKind Kind { get; }
    public Dictionary<ResourcePropertyType, ResourceProperty> Properties { get; }
    public HarvestMethod RequiredHarvestMethod { get; }

    public RawResource(string name, ResourceKind kind, Dictionary<ResourcePropertyType, ResourceProperty> properties,
        HarvestMethod requiredHarvestMethod = HarvestMethod.KineticMining)
    {
        Name = name;
        Kind = kind;
        Properties = properties;
        RequiredHarvestMethod = requiredHarvestMethod;
    }
}
