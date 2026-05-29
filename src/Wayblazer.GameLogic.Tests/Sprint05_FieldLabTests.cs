using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// Sprint 5: Field Lab & Analysis — deterministic acceptance tests.
/// Tests the transition from vague to precise resource data.
/// </summary>
public class Sprint05_FieldLabTests
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

    [Fact]
    public void AnalyzeResource_UnlockssPreciseData()
    {
        var scanner = new ScannerSystem();
        var ore = CreateBaseOre();

        // Before analysis: no precise data
        Assert.Null(scanner.GetPreciseScan(ore));

        // Analyze the resource
        scanner.AnalyzeResource(ore);

        // After analysis: precise data available
        var precise = scanner.GetPreciseScan(ore);
        Assert.NotNull(precise);
        Assert.Equal(2.0f, precise[ResourcePropertyType.Strength]);
        Assert.Equal(5.0f, precise[ResourcePropertyType.Resistance]);
    }

    [Fact]
    public void AnalyzeResource_EarnsAnalysisPoints()
    {
        var scanner = new ScannerSystem();
        var ore = CreateBaseOre();

        int points = scanner.AnalyzeResource(ore);

        Assert.Equal(5, points);
    }

    [Fact]
    public void AnalyzeResource_SecondAnalysis_EarnsZeroPoints()
    {
        var scanner = new ScannerSystem();
        var ore = CreateBaseOre();

        scanner.AnalyzeResource(ore); // First time
        int points = scanner.AnalyzeResource(ore); // Duplicate

        Assert.Equal(0, points);
    }

    [Fact]
    public void AnalyzeResource_DifferentResources_EarnPointsIndependently()
    {
        var scanner = new ScannerSystem();
        var ore1 = CreateBaseOre();
        var ore2 = new RawResource("Catalyst Ore", ResourceKind.Ore,
            new Dictionary<ResourcePropertyType, ResourceProperty>
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 4.0f)
            });

        int points1 = scanner.AnalyzeResource(ore1);
        int points2 = scanner.AnalyzeResource(ore2);

        Assert.Equal(5, points1);
        Assert.Equal(5, points2);
    }

    [Fact]
    public void IsResourceAnalyzed_ReturnsFalse_BeforeAnalysis()
    {
        var scanner = new ScannerSystem();

        Assert.False(scanner.IsResourceAnalyzed("Base Ore"));
    }

    [Fact]
    public void IsResourceAnalyzed_ReturnsTrue_AfterAnalysis()
    {
        var scanner = new ScannerSystem();
        var ore = CreateBaseOre();

        scanner.AnalyzeResource(ore);

        Assert.True(scanner.IsResourceAnalyzed("Base Ore"));
    }

    [Fact]
    public void PreciseScan_ShowsAllProperties_WithExactValues()
    {
        var scanner = new ScannerSystem();
        var ore = CreateBaseOre();
        scanner.AnalyzeResource(ore);

        var precise = scanner.GetPreciseScan(ore)!;

        Assert.Equal(5, precise.Count);
        Assert.Equal(2.0f, precise[ResourcePropertyType.Strength]);
        Assert.Equal(5.0f, precise[ResourcePropertyType.Resistance]);
        Assert.Equal(3.0f, precise[ResourcePropertyType.Toughness]);
        Assert.Equal(4.0f, precise[ResourcePropertyType.Conductivity]);
        Assert.Equal(6.0f, precise[ResourcePropertyType.Reactivity]);
    }
}
