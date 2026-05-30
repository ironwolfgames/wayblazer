using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// Planetary Observatory & Deduction — deterministic acceptance tests.
/// Tests measuring planetary constants and calculating portal requirements.
/// </summary>
public class Sprint06_ObservatoryTests
{
    [Fact]
    public void MeasureConstant_ReturnsExactGravity()
    {
        var system = new PlanetaryAnalysisSystem();
        var planet = PlanetaryConstants.Default; // Gravity = 3.2

        var (value, _) = system.MeasureConstant("Gravity", planet);

        Assert.Equal(3.2f, value);
    }

    [Fact]
    public void MeasureConstant_EarnsAnalysisPoints()
    {
        var system = new PlanetaryAnalysisSystem();
        var planet = PlanetaryConstants.Default;

        var (_, points) = system.MeasureConstant("Gravity", planet);

        Assert.Equal(8, points);
    }

    [Fact]
    public void MeasureConstant_Duplicate_EarnsZeroPoints()
    {
        var system = new PlanetaryAnalysisSystem();
        var planet = PlanetaryConstants.Default;

        system.MeasureConstant("Gravity", planet);
        var (_, points) = system.MeasureConstant("Gravity", planet);

        Assert.Equal(0, points);
    }

    [Fact]
    public void MeasureConstant_AllConstants_CanBeMeasured()
    {
        var system = new PlanetaryAnalysisSystem();
        var planet = PlanetaryConstants.Default;

        var constants = new[] { "Gravity", "AtmosphericPressure", "AtmosphericCorrosion",
                                "TectonicVolatility", "TemperatureRange" };

        foreach (var constant in constants)
        {
            var (value, _) = system.MeasureConstant(constant, planet);
            Assert.True(value >= 0 || constant == "TemperatureRange",
                $"Expected non-negative value for {constant}, got {value}");
        }
    }

    [Fact]
    public void IsConstantMeasured_TracksMeasurements()
    {
        var system = new PlanetaryAnalysisSystem();
        var planet = PlanetaryConstants.Default;

        Assert.False(system.IsConstantMeasured("Gravity"));

        system.MeasureConstant("Gravity", planet);

        Assert.True(system.IsConstantMeasured("Gravity"));
        Assert.False(system.IsConstantMeasured("AtmosphericPressure"));
    }

    [Fact]
    public void CalculatePortalRequirements_DefaultPlanet_StrengthFormula()
    {
        // Strength = Gravity × 2.5
        var planet = PlanetaryConstants.Default; // Gravity = 3.2
        var expected = 3.2f * 2.5f; // = 8.0

        var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(planet);

        Assert.Equal(expected, requirements[ResourcePropertyType.Strength]);
    }

    [Fact]
    public void CalculatePortalRequirements_DefaultPlanet_ResistanceFormula()
    {
        // Resistance = (TemperatureRange / 20) + (Corrosion × 3)
        var planet = PlanetaryConstants.Default; // Range = 60, Corrosion = 0.5
        var expected = (60f / 20f) + (0.5f * 3f); // = 3.0 + 1.5 = 4.5

        var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(planet);

        Assert.Equal(expected, requirements[ResourcePropertyType.Resistance]);
    }

    [Fact]
    public void CalculatePortalRequirements_DefaultPlanet_ToughnessFormula()
    {
        // Toughness = (TectonicVolatility × 4) + (AtmosphericPressure × 0.5)
        var planet = PlanetaryConstants.Default; // Tectonic = 1.0, Pressure = 5.0
        var expected = (1.0f * 4f) + (5.0f * 0.5f); // = 4.0 + 2.5 = 6.5

        var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(planet);

        Assert.Equal(expected, requirements[ResourcePropertyType.Toughness]);
    }

    [Fact]
    public void CalculatePortalRequirements_DefaultPlanet_ConductivityFormula()
    {
        // Conductivity = (AtmosphericPressure + Gravity) × 0.8
        var planet = PlanetaryConstants.Default; // Pressure = 5, Gravity = 3.2
        var expected = (5.0f + 3.2f) * 0.8f; // = 6.56

        var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(planet);

        Assert.Equal(expected, requirements[ResourcePropertyType.Conductivity], precision: 2);
    }

    [Fact]
    public void CalculatePortalRequirements_DefaultPlanet_ReactivityFormula()
    {
        // Reactivity = (TectonicVolatility + Corrosion) × 2.5
        var planet = PlanetaryConstants.Default; // Tectonic = 1.0, Corrosion = 0.5
        var expected = (1.0f + 0.5f) * 2.5f; // = 3.75

        var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(planet);

        Assert.Equal(expected, requirements[ResourcePropertyType.Reactivity]);
    }

    [Fact]
    public void CalculatePortalRequirements_ReturnsAllFiveProperties()
    {
        var planet = PlanetaryConstants.Default;

        var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(planet);

        Assert.Equal(5, requirements.Count);
        Assert.Contains(ResourcePropertyType.Strength, requirements.Keys);
        Assert.Contains(ResourcePropertyType.Resistance, requirements.Keys);
        Assert.Contains(ResourcePropertyType.Toughness, requirements.Keys);
        Assert.Contains(ResourcePropertyType.Conductivity, requirements.Keys);
        Assert.Contains(ResourcePropertyType.Reactivity, requirements.Keys);
    }

    [Fact]
    public void CalculatePortalRequirements_HighGravity_RequiresMoreStrength()
    {
        var normalPlanet = PlanetaryConstants.Default;
        var heavyPlanet = new PlanetaryConstants(9.0f, 5f, 0.5f, 1.0f, -20f, 40f);

        var normalReq = PlanetaryAnalysisSystem.CalculatePortalRequirements(normalPlanet);
        var heavyReq = PlanetaryAnalysisSystem.CalculatePortalRequirements(heavyPlanet);

        Assert.True(heavyReq[ResourcePropertyType.Strength] > normalReq[ResourcePropertyType.Strength]);
    }

    [Fact]
    public void MeasureConstant_UnknownConstant_ThrowsArgumentException()
    {
        var system = new PlanetaryAnalysisSystem();
        var planet = PlanetaryConstants.Default;

        Assert.Throws<ArgumentException>(() => system.MeasureConstant("FakeConstant", planet));
    }
}
