using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// Tests for deterministic resource generation from seeds.
/// Validates that same seed always produces same output, and that
/// complexity scaling works correctly.
/// </summary>
public class ResourceGenerationTests
{
    [Fact]
    public void SameSeed_ProducesSameResources()
    {
        var gen = new ResourceGenerationSystem();

        var resources1 = gen.GenerateResources(42, complexity: 1);
        var resources2 = gen.GenerateResources(42, complexity: 1);

        Assert.Equal(resources1.Count, resources2.Count);
        for (int i = 0; i < resources1.Count; i++)
        {
            Assert.Equal(resources1[i].Name, resources2[i].Name);
            Assert.Equal(resources1[i].Kind, resources2[i].Kind);
            foreach (var propType in resources1[i].Properties.Keys)
            {
                Assert.Equal(
                    resources1[i].Properties[propType].Value,
                    resources2[i].Properties[propType].Value);
            }
        }
    }

    [Fact]
    public void DifferentSeeds_ProduceDifferentResources()
    {
        var gen = new ResourceGenerationSystem();

        var resources1 = gen.GenerateResources(42);
        var resources2 = gen.GenerateResources(99);

        // At least one resource should differ (extremely unlikely to match)
        bool anyDifferent = false;
        for (int i = 0; i < Math.Min(resources1.Count, resources2.Count); i++)
        {
            if (resources1[i].Name != resources2[i].Name)
            {
                anyDifferent = true;
                break;
            }
            foreach (var propType in resources1[i].Properties.Keys)
            {
                if (Math.Abs(resources1[i].Properties[propType].Value -
                    resources2[i].Properties[propType].Value) > 0.001f)
                {
                    anyDifferent = true;
                    break;
                }
            }
            if (anyDifferent) break;
        }

        Assert.True(anyDifferent, "Different seeds should produce different resources");
    }

    [Fact]
    public void HigherComplexity_ProducesMoreResources()
    {
        var gen = new ResourceGenerationSystem();

        var resources1 = gen.GenerateResources(42, complexity: 1);
        var resources3 = gen.GenerateResources(42, complexity: 3);

        Assert.True(resources3.Count > resources1.Count);
    }

    [Fact]
    public void GeneratedResources_HaveAllFiveProperties()
    {
        var gen = new ResourceGenerationSystem();
        var resources = gen.GenerateResources(42);

        foreach (var resource in resources)
        {
            Assert.Equal(5, resource.Properties.Count);
            Assert.Contains(ResourcePropertyType.Strength, resource.Properties.Keys);
            Assert.Contains(ResourcePropertyType.Resistance, resource.Properties.Keys);
            Assert.Contains(ResourcePropertyType.Toughness, resource.Properties.Keys);
            Assert.Contains(ResourcePropertyType.Conductivity, resource.Properties.Keys);
            Assert.Contains(ResourcePropertyType.Reactivity, resource.Properties.Keys);
        }
    }

    [Fact]
    public void GeneratedResources_ValuesWithinExpectedRange()
    {
        var gen = new ResourceGenerationSystem();
        var resources = gen.GenerateResources(42, complexity: 1);

        foreach (var resource in resources)
        {
            foreach (var prop in resource.Properties.Values)
            {
                Assert.True(prop.Value >= 1.0f, $"Value {prop.Value} below minimum 1.0");
                Assert.True(prop.Value <= 8.0f, $"Value {prop.Value} above max for complexity 1");
            }
        }
    }

    [Fact]
    public void HigherComplexity_ProducesHigherMaxValues()
    {
        var gen = new ResourceGenerationSystem();

        // Generate many resources at different complexities
        var lowResources = gen.GenerateResources(42, complexity: 1);
        var highResources = gen.GenerateResources(42, complexity: 4);

        float maxLow = lowResources
            .SelectMany(r => r.Properties.Values)
            .Max(p => p.Value);

        float maxHigh = highResources
            .SelectMany(r => r.Properties.Values)
            .Max(p => p.Value);

        // Higher complexity should allow higher property values
        Assert.True(maxHigh > maxLow,
            $"Complexity 4 max ({maxHigh}) should exceed complexity 1 max ({maxLow})");
    }

    [Fact]
    public void GeneratedResources_HaveValidKinds()
    {
        var gen = new ResourceGenerationSystem();
        var resources = gen.GenerateResources(42);

        // Should have at least Ore, Wood, and Gas types
        Assert.Contains(resources, r => r.Kind == ResourceKind.Ore);
        Assert.Contains(resources, r => r.Kind == ResourceKind.Wood);
    }

    [Fact]
    public void GeneratedResources_HaveAppropriateHarvestMethods()
    {
        var gen = new ResourceGenerationSystem();
        var resources = gen.GenerateResources(42, complexity: 2);

        foreach (var resource in resources)
        {
            if (resource.Kind == ResourceKind.Gas)
            {
                Assert.Equal(HarvestMethod.GasSiphoning, resource.RequiredHarvestMethod);
            }
            else
            {
                Assert.Equal(HarvestMethod.KineticMining, resource.RequiredHarvestMethod);
            }
        }
    }

    [Fact]
    public void GeneratePlanetaryConstants_SameSeed_SameResult()
    {
        var gen = new ResourceGenerationSystem();

        var planet1 = gen.GeneratePlanetaryConstants(42);
        var planet2 = gen.GeneratePlanetaryConstants(42);

        Assert.Equal(planet1.Gravity, planet2.Gravity);
        Assert.Equal(planet1.AtmosphericPressure, planet2.AtmosphericPressure);
        Assert.Equal(planet1.AtmosphericCorrosion, planet2.AtmosphericCorrosion);
        Assert.Equal(planet1.TectonicVolatility, planet2.TectonicVolatility);
        Assert.Equal(planet1.TemperatureRange, planet2.TemperatureRange);
    }

    [Fact]
    public void GeneratePlanetaryConstants_HigherComplexity_MoreExtreme()
    {
        var gen = new ResourceGenerationSystem();

        var easy = gen.GeneratePlanetaryConstants(42, complexity: 1);
        var hard = gen.GeneratePlanetaryConstants(42, complexity: 4);

        // At higher complexity, at least one constant should be more extreme
        bool anyMoreExtreme =
            hard.Gravity > easy.Gravity ||
            hard.AtmosphericPressure > easy.AtmosphericPressure ||
            hard.AtmosphericCorrosion > easy.AtmosphericCorrosion;

        Assert.True(anyMoreExtreme,
            "Higher complexity should produce more extreme planetary conditions");
    }

    [Fact]
    public void GeneratePlanetaryConstants_ValuesPositive()
    {
        var gen = new ResourceGenerationSystem();

        for (int seed = 0; seed < 20; seed++)
        {
            var planet = gen.GeneratePlanetaryConstants(seed);
            Assert.True(planet.Gravity > 0, $"Gravity must be positive (seed={seed})");
            Assert.True(planet.AtmosphericPressure > 0, $"Pressure must be positive (seed={seed})");
            Assert.True(planet.AtmosphericCorrosion >= 0, $"Corrosion must be non-negative (seed={seed})");
            Assert.True(planet.TectonicVolatility >= 0, $"Tectonic must be non-negative (seed={seed})");
            Assert.True(planet.TemperatureRange > 0, $"Temp range must be positive (seed={seed})");
        }
    }

    [Fact]
    public void GenerateResource_ByIndex_Deterministic()
    {
        var gen = new ResourceGenerationSystem();

        var resource1 = gen.GenerateResource(42, index: 2);
        var resource2 = gen.GenerateResource(42, index: 2);

        Assert.Equal(resource1.Name, resource2.Name);
        Assert.Equal(resource1.Kind, resource2.Kind);
    }
}
