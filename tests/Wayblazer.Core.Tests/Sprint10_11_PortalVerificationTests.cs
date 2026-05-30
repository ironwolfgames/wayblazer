using Wayblazer.Core.Models;
using Wayblazer.Core.Systems;

namespace Wayblazer.Core.Tests;

/// <summary>
/// Portal Construction & Simulation — deterministic acceptance tests.
/// Tests material verification against calculated portal requirements.
/// </summary>
public class Sprint10_11_PortalVerificationTests
{
    [Fact]
    public void MeetsRequirements_PassingMaterial_ReturnsTrue()
    {
        var verifier = new PortalVerificationSystem();

        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f,
            [ResourcePropertyType.Resistance] = 3.0f
        };

        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 9.0f,
            [ResourcePropertyType.Resistance] = 4.0f
        };

        Assert.True(verifier.MeetsRequirements(material, requirements));
    }

    [Fact]
    public void MeetsRequirements_FailingMaterial_ReturnsFalse()
    {
        var verifier = new PortalVerificationSystem();

        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f,
            [ResourcePropertyType.Resistance] = 3.0f
        };

        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 7.0f, // Below 8.0
            [ResourcePropertyType.Resistance] = 4.0f
        };

        Assert.False(verifier.MeetsRequirements(material, requirements));
    }

    [Fact]
    public void MeetsRequirements_ExactlyMeetsThreshold_ReturnsTrue()
    {
        var verifier = new PortalVerificationSystem();

        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f
        };

        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f
        };

        Assert.True(verifier.MeetsRequirements(material, requirements));
    }

    [Fact]
    public void MeetsRequirements_MissingProperty_ReturnsFalse()
    {
        var verifier = new PortalVerificationSystem();

        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f,
            [ResourcePropertyType.Conductivity] = 5.0f
        };

        // Material has Strength but not Conductivity
        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 9.0f
        };

        Assert.False(verifier.MeetsRequirements(material, requirements));
    }

    [Fact]
    public void RunSimulation_AllPass_OverallPassTrue()
    {
        var verifier = new PortalVerificationSystem();

        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f,
            [ResourcePropertyType.Resistance] = 4.5f,
            [ResourcePropertyType.Toughness] = 6.5f
        };

        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Resistance] = 5.0f,
            [ResourcePropertyType.Toughness] = 7.0f
        };

        var result = verifier.RunSimulation(material, requirements);

        Assert.True(result.OverallPass);
        Assert.All(result.PropertyResults.Values, r => Assert.True(r.Passes));
    }

    [Fact]
    public void RunSimulation_OneFailure_OverallPassFalse()
    {
        var verifier = new PortalVerificationSystem();

        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f,
            [ResourcePropertyType.Resistance] = 4.5f
        };

        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Resistance] = 3.0f // Fails
        };

        var result = verifier.RunSimulation(material, requirements);

        Assert.False(result.OverallPass);
        Assert.True(result.PropertyResults[ResourcePropertyType.Strength].Passes);
        Assert.False(result.PropertyResults[ResourcePropertyType.Resistance].Passes);
    }

    [Fact]
    public void RunSimulation_ReportsPercentages()
    {
        var verifier = new PortalVerificationSystem();

        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f
        };

        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 10.0f
        };

        var result = verifier.RunSimulation(material, requirements);

        // 10.0 / 8.0 = 125%
        Assert.Equal(125.0f, result.PropertyResults[ResourcePropertyType.Strength].Percentage);
    }

    [Fact]
    public void RunSimulation_FailingProperty_ShowsDeficitPercentage()
    {
        var verifier = new PortalVerificationSystem();

        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 10.0f
        };

        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 7.5f
        };

        var result = verifier.RunSimulation(material, requirements);

        // 7.5 / 10.0 = 75%
        Assert.Equal(75.0f, result.PropertyResults[ResourcePropertyType.Strength].Percentage);
        Assert.False(result.PropertyResults[ResourcePropertyType.Strength].Passes);
    }

    [Fact]
    public void RunSimulation_OverallIntegrity_AveragesPercentages()
    {
        var verifier = new PortalVerificationSystem();

        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f,
            [ResourcePropertyType.Resistance] = 4.0f
        };

        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 10.0f, // 125%
            [ResourcePropertyType.Resistance] = 3.0f  // 75%
        };

        var result = verifier.RunSimulation(material, requirements);

        // Average of 125 and 75 = 100
        Assert.Equal(100.0f, result.OverallIntegrity);
    }

    [Fact]
    public void VerticalSlice_FullDeductionCycle_DefaultPlanet()
    {
        // This is the master integration test for the entire vertical slice.
        // It tests the complete L.D.E.S.A. cycle deterministically.

        // 1. LAND: Planet generated with known constants
        var planet = PlanetaryConstants.Default; // Gravity=3.2

        // 2. DEDUCE: Calculate portal requirements
        var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(planet);
        Assert.Equal(8.0f, requirements[ResourcePropertyType.Strength]); // 3.2 × 2.5

        // 3. ENGINEER: Create a material that meets requirements
        // Simulate crafting a composite with sufficient strength
        var compositeProperties = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.2f,    // Above 8.0 threshold
            [ResourcePropertyType.Resistance] = 5.0f,  // Above 4.5
            [ResourcePropertyType.Toughness] = 7.0f,   // Above 6.5
            [ResourcePropertyType.Conductivity] = 7.0f, // Above 6.56
            [ResourcePropertyType.Reactivity] = 4.0f   // Above 3.75
        };

        // 4. SIMULATE: Verify the material passes
        var verifier = new PortalVerificationSystem();
        var result = verifier.RunSimulation(compositeProperties, requirements);

        Assert.True(result.OverallPass);
        Assert.True(result.OverallIntegrity >= 100.0f);
    }

    [Fact]
    public void VerticalSlice_InsufficientEngineering_Fails()
    {
        var planet = PlanetaryConstants.Default;
        var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(planet);

        // Material that's too weak (didn't use catalyst ore)
        var weakMaterial = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 3.0f,    // Way below 8.0
            [ResourcePropertyType.Resistance] = 2.0f,
            [ResourcePropertyType.Toughness] = 2.0f,
            [ResourcePropertyType.Conductivity] = 2.0f,
            [ResourcePropertyType.Reactivity] = 1.0f
        };

        var verifier = new PortalVerificationSystem();
        var result = verifier.RunSimulation(weakMaterial, requirements);

        Assert.False(result.OverallPass);
        Assert.False(result.PropertyResults[ResourcePropertyType.Strength].Passes);
    }
}
