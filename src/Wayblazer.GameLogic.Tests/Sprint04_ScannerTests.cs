using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// Sprint 4: Hand Scanner — deterministic acceptance tests.
/// Tests the vague scanning system that provides initial deduction clues.
/// </summary>
public class Sprint04_ScannerTests
{
    private static RawResource CreateBaseOre() => new(
        "Base Ore", ResourceKind.Ore,
        new Dictionary<ResourcePropertyType, ResourceProperty>
        {
            [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 2.0f),
            [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 5.0f),
            [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 3.0f),
            [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 4.0f),
            [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 6.0f)
        });

    private static RawResource CreateCatalystOre() => new(
        "Catalyst Ore", ResourceKind.Ore,
        new Dictionary<ResourcePropertyType, ResourceProperty>
        {
            [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 4.0f),
            [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 2.0f),
            [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 2.5f),
            [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 8.0f),
            [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 9.0f)
        });

    [Fact]
    public void VagueScan_BaseOre_ReturnsCorrectDescriptions()
    {
        var scanner = new ScannerSystem();
        var ore = CreateBaseOre();

        var result = scanner.GetVagueScan(ore);

        Assert.Equal("Low", result[ResourcePropertyType.Strength]);     // 2.0 < 3.0
        Assert.Equal("Medium", result[ResourcePropertyType.Resistance]); // 5.0 is between 3.0 and 7.0
        Assert.Equal("Medium", result[ResourcePropertyType.Toughness]);  // 3.0 is exactly boundary
        Assert.Equal("Medium", result[ResourcePropertyType.Conductivity]); // 4.0
        Assert.Equal("Medium", result[ResourcePropertyType.Reactivity]); // 6.0
    }

    [Fact]
    public void VagueScan_CatalystOre_HighConductivityAndReactivity()
    {
        var scanner = new ScannerSystem();
        var ore = CreateCatalystOre();

        var result = scanner.GetVagueScan(ore);

        Assert.Equal("High", result[ResourcePropertyType.Conductivity]); // 8.0 > 7.0
        Assert.Equal("High", result[ResourcePropertyType.Reactivity]);   // 9.0 > 7.0
    }

    [Fact]
    public void VagueScan_AlwaysAvailable_NoPrerequisites()
    {
        var scanner = new ScannerSystem();
        var ore = CreateBaseOre();

        // Should work immediately without any analysis
        var result = scanner.GetVagueScan(ore);

        Assert.NotEmpty(result);
        Assert.Equal(5, result.Count); // All 5 property types
    }

    [Fact]
    public void PreciseScan_BeforeAnalysis_ReturnsNull()
    {
        var scanner = new ScannerSystem();
        var ore = CreateBaseOre();

        var result = scanner.GetPreciseScan(ore);

        Assert.Null(result);
    }

    [Fact]
    public void VaguePlanetaryScan_DefaultPlanet_ReturnsVagueDescriptions()
    {
        var scanner = new ScannerSystem();
        var constants = PlanetaryConstants.Default;

        var result = scanner.GetVaguePlanetaryScan(constants);

        Assert.Equal(5, result.Count);
        Assert.Contains("Gravity", result.Keys);
        Assert.Contains("Atmospheric Pressure", result.Keys);
        Assert.Contains("Corrosion", result.Keys);
        Assert.Contains("Tectonic Activity", result.Keys);
        Assert.Contains("Temperature Range", result.Keys);

        // All values should be one of "Low", "Moderate", or "High"
        foreach (var value in result.Values)
        {
            Assert.Contains(value, new[] { "Low", "Moderate", "High" });
        }
    }

    [Fact]
    public void VaguePlanetaryScan_HighGravityPlanet_ReportsHigh()
    {
        var scanner = new ScannerSystem();
        var planet = new PlanetaryConstants(
            gravity: 9.0f,
            atmosphericPressure: 1.0f,
            atmosphericCorrosion: 0.1f,
            tectonicVolatility: 0.1f,
            lowTemperature: 10f,
            highTemperature: 30f);

        var result = scanner.GetVaguePlanetaryScan(planet);

        Assert.Equal("High", result["Gravity"]);
        Assert.Equal("Low", result["Corrosion"]);
    }
}
