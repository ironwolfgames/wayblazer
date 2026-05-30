using Wayblazer.Core.Models;

namespace Wayblazer.Core.Systems;

/// <summary>
/// Deterministic resource generation from seed and complexity level.
/// Ensures same seed always produces identical resource properties.
/// </summary>
public class ResourceGenerationSystem
{
    /// <summary>
    /// Generates a set of resources for a planet given a seed and complexity.
    /// Higher complexity produces more extreme property values.
    /// </summary>
    public List<RawResource> GenerateResources(int seed, int complexity = 1)
    {
        var resources = new List<RawResource>();
        var rng = new Random(seed);

        int resourceCount = 3 + complexity; // More resources at higher complexity

        for (int i = 0; i < resourceCount; i++)
        {
            var resource = GenerateSingleResource(rng, complexity, i);
            resources.Add(resource);
        }

        return resources;
    }

    /// <summary>
    /// Generates a single resource with a specific sub-seed for determinism.
    /// </summary>
    public RawResource GenerateResource(int seed, int index, int complexity = 1)
    {
        var rng = new Random(seed);
        // Advance to the correct index
        for (int i = 0; i < index; i++)
        {
            AdvanceRng(rng);
        }
        return GenerateSingleResource(rng, complexity, index);
    }

    /// <summary>
    /// Generates planetary constants from a seed and complexity.
    /// </summary>
    public PlanetaryConstants GeneratePlanetaryConstants(int seed, int complexity = 1)
    {
        var rng = new Random(seed + 10000); // Offset to avoid correlation with resources

        float gravityRange = 1.0f + complexity * 2.0f;
        float gravity = 1.0f + (float)rng.NextDouble() * gravityRange;

        float pressure = 1.0f + (float)rng.NextDouble() * (3.0f + complexity * 2.0f);

        float corrosion = (float)rng.NextDouble() * (0.3f + complexity * 0.3f);

        float tectonic = (float)rng.NextDouble() * (0.5f + complexity * 0.5f);

        float tempLow = -40f + (float)rng.NextDouble() * 30f;
        float tempHigh = tempLow + 30f + (float)rng.NextDouble() * (30f + complexity * 20f);

        return new PlanetaryConstants(gravity, pressure, corrosion, tectonic, tempLow, tempHigh);
    }

    private RawResource GenerateSingleResource(Random rng, int complexity, int index)
    {
        ReadOnlySpan<ResourceKind> generatedKinds = [ResourceKind.Ore, ResourceKind.Wood, ResourceKind.Gas];
        var kind = generatedKinds[index % generatedKinds.Length];
        var name = GenerateResourceName(kind, index, rng);
        var harvestMethod = GetHarvestMethodForKind(kind);

        var properties = new Dictionary<ResourcePropertyType, ResourceProperty>();
        float minValue = 1.0f;
        float maxValue = 5.0f + complexity * 3.0f; // Higher complexity = higher possible stats

        foreach (ResourcePropertyType propType in Enum.GetValues<ResourcePropertyType>())
        {
            float value = minValue + (float)rng.NextDouble() * (maxValue - minValue);
            properties[propType] = new ResourceProperty(propType, value);
        }

        return new RawResource(name, kind, properties, harvestMethod);
    }

    private static string GenerateResourceName(ResourceKind kind, int index, Random rng)
    {
        var oreNames = new[] { "Ferrite", "Titanite", "Chromite", "Cobaltite", "Magnetite" };
        var woodNames = new[] { "Softwood", "Hardwood", "Ironbark", "Crystalwood", "Petrified" };
        var gasNames = new[] { "Argonite", "Xenonite", "Kryptonite", "Neonite", "Helionite" };

        return kind switch
        {
            ResourceKind.Ore => oreNames[rng.Next(oreNames.Length)] + " Ore",
            ResourceKind.Wood => woodNames[rng.Next(woodNames.Length)] + " Wood",
            ResourceKind.Gas => gasNames[rng.Next(gasNames.Length)] + " Gas",
            _ => $"Resource_{index}"
        };
    }

    private static HarvestMethod GetHarvestMethodForKind(ResourceKind kind)
    {
        return kind switch
        {
            ResourceKind.Ore => HarvestMethod.KineticMining,
            ResourceKind.Wood => HarvestMethod.KineticMining,
            ResourceKind.Gas => HarvestMethod.GasSiphoning,
            _ => HarvestMethod.KineticMining
        };
    }

    private static void AdvanceRng(Random rng)
    {
        // Consume enough random values to advance past one resource generation
        for (int i = 0; i < 8; i++) rng.NextDouble();
    }
}
