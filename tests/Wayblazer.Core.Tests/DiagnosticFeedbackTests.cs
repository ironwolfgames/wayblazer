using Wayblazer.Core.Models;
using Wayblazer.Core.Systems;

namespace Wayblazer.Core.Tests;

/// <summary>
/// Tests for detailed diagnostic feedback from the simulation system.
/// Validates that failure reports provide actionable information to the player.
/// </summary>
public class DiagnosticFeedbackTests
{
    [Fact]
    public void RunSimulation_AllPass_ReportsEachPropertyPassing()
    {
        var verifier = new PortalVerificationSystem();
        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f,
            [ResourcePropertyType.Resistance] = 4.0f,
            [ResourcePropertyType.Toughness] = 6.0f
        };
        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 9.0f,
            [ResourcePropertyType.Resistance] = 5.0f,
            [ResourcePropertyType.Toughness] = 7.0f
        };

        var result = verifier.RunSimulation(material, requirements);

        Assert.Equal(3, result.PropertyResults.Count);
        Assert.All(result.PropertyResults.Values, r =>
        {
            Assert.True(r.Passes);
            Assert.True(r.Percentage >= 100f);
        });
    }

    [Fact]
    public void RunSimulation_Failure_IdentifiesDeficitProperty()
    {
        var verifier = new PortalVerificationSystem();
        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f,
            [ResourcePropertyType.Resistance] = 4.0f,
            [ResourcePropertyType.Toughness] = 6.0f
        };
        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 5.0f,  // FAILING
            [ResourcePropertyType.Resistance] = 5.0f, // Passing
            [ResourcePropertyType.Toughness] = 7.0f   // Passing
        };

        var result = verifier.RunSimulation(material, requirements);

        Assert.False(result.OverallPass);
        Assert.False(result.PropertyResults[ResourcePropertyType.Strength].Passes);
        Assert.True(result.PropertyResults[ResourcePropertyType.Resistance].Passes);
        Assert.True(result.PropertyResults[ResourcePropertyType.Toughness].Passes);
    }

    [Fact]
    public void RunSimulation_ReportsActualAndRequiredValues()
    {
        var verifier = new PortalVerificationSystem();
        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f
        };
        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 6.0f
        };

        var result = verifier.RunSimulation(material, requirements);
        var strengthResult = result.PropertyResults[ResourcePropertyType.Strength];

        Assert.Equal(8.0f, strengthResult.RequiredValue);
        Assert.Equal(6.0f, strengthResult.ActualValue);
        Assert.Equal(75.0f, strengthResult.Percentage); // 6/8 = 75%
    }

    [Fact]
    public void RunSimulation_MultipleFailures_AllReported()
    {
        var verifier = new PortalVerificationSystem();
        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f,
            [ResourcePropertyType.Resistance] = 5.0f,
            [ResourcePropertyType.Toughness] = 6.0f,
            [ResourcePropertyType.Conductivity] = 7.0f,
            [ResourcePropertyType.Reactivity] = 4.0f
        };
        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 3.0f,     // Fail
            [ResourcePropertyType.Resistance] = 2.0f,   // Fail
            [ResourcePropertyType.Toughness] = 10.0f,   // Pass
            [ResourcePropertyType.Conductivity] = 1.0f, // Fail
            [ResourcePropertyType.Reactivity] = 5.0f    // Pass
        };

        var result = verifier.RunSimulation(material, requirements);

        var failures = result.PropertyResults.Values.Where(r => !r.Passes).ToList();
        var passes = result.PropertyResults.Values.Where(r => r.Passes).ToList();

        Assert.Equal(3, failures.Count);
        Assert.Equal(2, passes.Count);
    }

    [Fact]
    public void RunSimulation_MissingProperty_ReportsAsZeroPercent()
    {
        var verifier = new PortalVerificationSystem();
        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f,
            [ResourcePropertyType.Conductivity] = 5.0f
        };
        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 9.0f
            // Conductivity is missing entirely
        };

        var result = verifier.RunSimulation(material, requirements);

        Assert.False(result.OverallPass);
        Assert.Equal(0.0f, result.PropertyResults[ResourcePropertyType.Conductivity].ActualValue);
        Assert.Equal(0.0f, result.PropertyResults[ResourcePropertyType.Conductivity].Percentage);
    }

    [Fact]
    public void RunSimulation_ExactlyMeetsThreshold_Reports100Percent()
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

        var result = verifier.RunSimulation(material, requirements);

        Assert.True(result.OverallPass);
        Assert.Equal(100.0f, result.PropertyResults[ResourcePropertyType.Strength].Percentage);
    }

    [Fact]
    public void OverallIntegrity_AllPass_Above100()
    {
        var verifier = new PortalVerificationSystem();
        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 5.0f,
            [ResourcePropertyType.Resistance] = 4.0f
        };
        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 10.0f, // 200%
            [ResourcePropertyType.Resistance] = 8.0f  // 200%
        };

        var result = verifier.RunSimulation(material, requirements);

        Assert.True(result.OverallIntegrity >= 100.0f);
        Assert.Equal(200.0f, result.OverallIntegrity);
    }

    [Fact]
    public void OverallIntegrity_MixedResults_AveragesCorrectly()
    {
        var verifier = new PortalVerificationSystem();
        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Resistance] = 10.0f,
            [ResourcePropertyType.Toughness] = 10.0f
        };
        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 12.0f, // 120%
            [ResourcePropertyType.Resistance] = 8.0f, // 80%
            [ResourcePropertyType.Toughness] = 10.0f  // 100%
        };

        var result = verifier.RunSimulation(material, requirements);

        Assert.Equal(100.0f, result.OverallIntegrity); // (120+80+100)/3
    }

    [Fact]
    public void RunSimulation_ZeroRequirement_TreatedAs100Percent()
    {
        var verifier = new PortalVerificationSystem();
        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 0.0f // Edge case
        };
        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 5.0f
        };

        var result = verifier.RunSimulation(material, requirements);

        Assert.True(result.OverallPass);
        Assert.Equal(100.0f, result.PropertyResults[ResourcePropertyType.Strength].Percentage);
    }

    [Fact]
    public void RunSimulation_CanSortFailuresBySeverity()
    {
        var verifier = new PortalVerificationSystem();
        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Resistance] = 8.0f,
            [ResourcePropertyType.Toughness] = 6.0f
        };
        var material = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 2.0f,  // 20% - worst
            [ResourcePropertyType.Resistance] = 6.0f, // 75%
            [ResourcePropertyType.Toughness] = 5.0f   // 83%
        };

        var result = verifier.RunSimulation(material, requirements);

        // Sort failures by percentage (worst first)
        var sorted = result.PropertyResults.Values
            .Where(r => !r.Passes)
            .OrderBy(r => r.Percentage)
            .ToList();

        Assert.Equal(ResourcePropertyType.Strength, sorted[0].PropertyType);
        Assert.Equal(ResourcePropertyType.Resistance, sorted[1].PropertyType);
    }
}
