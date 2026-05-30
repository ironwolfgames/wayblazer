using Wayblazer.Core.Models;
using Wayblazer.Core.Systems;

namespace Wayblazer.Core.Tests;

/// <summary>
/// Tests for difficulty scaling as the player advances to harder worlds.
/// Validates complexity progression and solvability guarantees.
/// </summary>
public class DifficultyScalingTests
{
    [Theory]
    [InlineData(1, 4)]
    [InlineData(2, 5)]
    [InlineData(3, 6)]
    [InlineData(4, 7)]
    public void GetResourceCount_IncreasesWithComplexity(int complexity, int expectedCount)
    {
        Assert.Equal(expectedCount, DifficultySystem.GetResourceCount(complexity));
    }

    [Fact]
    public void GetRequirementMultiplier_Complexity1_IsBaseline()
    {
        Assert.Equal(1.0f, DifficultySystem.GetRequirementMultiplier(1));
    }

    [Fact]
    public void GetRequirementMultiplier_IncreasesWithComplexity()
    {
        float mult1 = DifficultySystem.GetRequirementMultiplier(1);
        float mult2 = DifficultySystem.GetRequirementMultiplier(2);
        float mult3 = DifficultySystem.GetRequirementMultiplier(3);

        Assert.True(mult2 > mult1);
        Assert.True(mult3 > mult2);
    }

    [Fact]
    public void IsPlanetSolvable_DefaultPlanet_WithVSResources_IsTrue()
    {
        var resources = new List<RawResource>
        {
            new("Base Ore", ResourceKind.Ore, new()
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 2.0f),
                [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 5.0f),
                [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 5.0f),
                [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 4.0f),
                [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 2.0f)
            }),
            new("Catalyst Ore", ResourceKind.Ore, new()
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 6.0f),
                [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 3.0f),
                [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 6.0f),
                [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 8.0f),
                [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 3.0f)
            }),
            new("Soft Wood", ResourceKind.Wood, new()
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 1.0f),
                [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 1.5f),
                [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 4.0f),
                [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 0.5f),
                [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 7.0f)
            })
        };

        var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(PlanetaryConstants.Default);

        Assert.True(DifficultySystem.IsPlanetSolvable(resources, requirements));
    }

    [Fact]
    public void IsPlanetSolvable_ImpossibleRequirements_ReturnsFalse()
    {
        var resources = new List<RawResource>
        {
            new("Weak Ore", ResourceKind.Ore, new()
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 1.0f)
            })
        };

        var impossibleReqs = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 100.0f // Way too high
        };

        Assert.False(DifficultySystem.IsPlanetSolvable(resources, impossibleReqs));
    }

    [Fact]
    public void IsPlanetSolvable_NoResources_ReturnsFalse()
    {
        var emptyResources = new List<RawResource>();
        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 5.0f
        };

        Assert.False(DifficultySystem.IsPlanetSolvable(emptyResources, requirements));
    }

    [Fact]
    public void GetDifficultyName_ReturnsExpectedNames()
    {
        Assert.Equal("Frontier", DifficultySystem.GetDifficultyName(1));
        Assert.Equal("Challenging", DifficultySystem.GetDifficultyName(2));
        Assert.Equal("Hostile", DifficultySystem.GetDifficultyName(3));
        Assert.Equal("Extreme", DifficultySystem.GetDifficultyName(4));
        Assert.Equal("Impossible", DifficultySystem.GetDifficultyName(5));
    }

    [Fact]
    public void GetTechNodeCount_IncreasesWithComplexity()
    {
        int nodes1 = DifficultySystem.GetTechNodeCount(1);
        int nodes3 = DifficultySystem.GetTechNodeCount(3);

        Assert.True(nodes3 > nodes1);
    }

    [Fact]
    public void GeneratedPlanet_Complexity1_IsSolvable()
    {
        var gen = new ResourceGenerationSystem();

        // Test multiple seeds to ensure generator produces solvable worlds
        int solvableCount = 0;
        for (int seed = 0; seed < 20; seed++)
        {
            var resources = gen.GenerateResources(seed, complexity: 1);
            var planet = gen.GeneratePlanetaryConstants(seed, complexity: 1);
            var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(planet);

            if (DifficultySystem.IsPlanetSolvable(resources, requirements))
                solvableCount++;
        }

        // At least 50% of complexity-1 worlds should be solvable
        // (real game would use rejection sampling to guarantee solvability)
        Assert.True(solvableCount >= 10,
            $"Only {solvableCount}/20 complexity-1 worlds were solvable (expected >= 10)");
    }

    [Fact]
    public void HigherComplexity_ProducesHigherPortalRequirements()
    {
        var gen = new ResourceGenerationSystem();

        var easyPlanet = gen.GeneratePlanetaryConstants(42, complexity: 1);
        var hardPlanet = gen.GeneratePlanetaryConstants(42, complexity: 4);

        var easyReqs = PlanetaryAnalysisSystem.CalculatePortalRequirements(easyPlanet);
        var hardReqs = PlanetaryAnalysisSystem.CalculatePortalRequirements(hardPlanet);

        // At least the strength requirement should be higher for harder planets
        Assert.True(hardReqs[ResourcePropertyType.Strength] > easyReqs[ResourcePropertyType.Strength],
            "Harder planet should require more strength");
    }
}
