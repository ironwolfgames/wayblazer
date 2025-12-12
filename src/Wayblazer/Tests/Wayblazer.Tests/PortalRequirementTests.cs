using System.Linq;
using Xunit;

namespace Wayblazer.Tests;

public class PortalRequirementTests
{
	[Fact]
	public void CreatePortalRequirementForPlanet_CreatesAllResourcePropertyTypes()
	{
		var planetaryConstants = PlanetaryConstants.Default;

		var requirement = PortalRequirement.CreatePortalRequirementForPlanet(planetaryConstants);

		Assert.NotNull(requirement.ResourcePropertyRequirements);
		Assert.Equal(5, requirement.ResourcePropertyRequirements.Count);
		Assert.Contains(ResourcePropertyType.Conductivity, requirement.ResourcePropertyRequirements.Keys);
		Assert.Contains(ResourcePropertyType.Reactivity, requirement.ResourcePropertyRequirements.Keys);
		Assert.Contains(ResourcePropertyType.Resistance, requirement.ResourcePropertyRequirements.Keys);
		Assert.Contains(ResourcePropertyType.Strength, requirement.ResourcePropertyRequirements.Keys);
		Assert.Contains(ResourcePropertyType.Toughness, requirement.ResourcePropertyRequirements.Keys);
	}

	[Fact]
	public void CreatePortalRequirementForPlanet_AllValuesArePositive()
	{
		var planetaryConstants = PlanetaryConstants.Default;

		var requirement = PortalRequirement.CreatePortalRequirementForPlanet(planetaryConstants);

		foreach (var value in requirement.ResourcePropertyRequirements.Values)
		{
			Assert.True(value > 0, $"Expected positive value, got {value}");
		}
	}

	[Fact]
	public void CreatePortalRequirementForPlanet_DifficultyMultiplierScalesValues()
	{
		var planetaryConstants = PlanetaryConstants.Default;
		var difficultyMultiplier = 2.0f;

		var normalRequirement = PortalRequirement.CreatePortalRequirementForPlanet(planetaryConstants, 1.0f);
		var harderRequirement = PortalRequirement.CreatePortalRequirementForPlanet(planetaryConstants, difficultyMultiplier);

		// Due to randomization, we can't expect exact 2x values, but harder should generally be higher
		// So we'll check that at least the sum of all requirements is significantly higher
		var normalSum = normalRequirement.ResourcePropertyRequirements.Values.Sum();
		var harderSum = harderRequirement.ResourcePropertyRequirements.Values.Sum();

		// With 2x multiplier, even with random variation (0.75-1.25), harder should be noticeably higher
		Assert.True(harderSum > normalSum * 1.2f,
			$"Expected harder requirement sum ({harderSum}) to be at least 1.2x normal sum ({normalSum})");
	}

	[Theory]
	[InlineData(0f, 0f, 0f, 0f, 0f, 0f)] // Minimal planet
	[InlineData(10f, 10f, 1f, 1f, -100f, 100f)] // Extreme planet
	[InlineData(5f, 5f, 0.5f, 0.5f, -50f, 50f)] // Moderate planet
	public void CreatePortalRequirementForPlanet_HandlesVariousPlanetaryConstants(
		float gravity, float atmosphericPressure, float atmosphericCorrosion,
		float tectonicVolatility, float lowTemp, float highTemp)
	{
		var planetaryConstants = new PlanetaryConstants(
			gravity, atmosphericPressure, atmosphericCorrosion,
			tectonicVolatility, lowTemp, highTemp);

		var requirement = PortalRequirement.CreatePortalRequirementForPlanet(planetaryConstants);

		Assert.NotNull(requirement);
		Assert.NotNull(requirement.ResourcePropertyRequirements);
		Assert.Equal(5, requirement.ResourcePropertyRequirements.Count);
	}

	[Fact]
	public void CreatePortalRequirementForPlanet_ConductivityReflectsAtmosphericPressureAndGravity()
	{
		var highPressureGravityPlanet = new PlanetaryConstants(
			gravity: 10f, atmosphericPressure: 10f,
			atmosphericCorrosion: 0.5f, tectonicVolatility: 0.5f,
			lowTemperature: 0f, highTemperature: 100f);

		var lowPressureGravityPlanet = new PlanetaryConstants(
			gravity: 1f, atmosphericPressure: 1f,
			atmosphericCorrosion: 0.5f, tectonicVolatility: 0.5f,
			lowTemperature: 0f, highTemperature: 100f);

		var highRequirement = PortalRequirement.CreatePortalRequirementForPlanet(highPressureGravityPlanet);
		var lowRequirement = PortalRequirement.CreatePortalRequirementForPlanet(lowPressureGravityPlanet);

		// Even with randomization (0.75-1.25 multiplier), high should be significantly higher
		Assert.True(highRequirement.ResourcePropertyRequirements[ResourcePropertyType.Conductivity] >
			lowRequirement.ResourcePropertyRequirements[ResourcePropertyType.Conductivity],
			"Higher atmospheric pressure and gravity should require more conductivity");
	}

	[Fact]
	public void CreatePortalRequirementForPlanet_ReactivityReflectsTectonicAndCorrosion()
	{
		var volatilePlanet = new PlanetaryConstants(
			gravity: 5f, atmosphericPressure: 5f,
			atmosphericCorrosion: 1f, tectonicVolatility: 1f,
			lowTemperature: 0f, highTemperature: 100f);

		var stablePlanet = new PlanetaryConstants(
			gravity: 5f, atmosphericPressure: 5f,
			atmosphericCorrosion: 0f, tectonicVolatility: 0f,
			lowTemperature: 0f, highTemperature: 100f);

		var volatileRequirement = PortalRequirement.CreatePortalRequirementForPlanet(volatilePlanet);
		var stableRequirement = PortalRequirement.CreatePortalRequirementForPlanet(stablePlanet);

		Assert.True(volatileRequirement.ResourcePropertyRequirements[ResourcePropertyType.Reactivity] >
			stableRequirement.ResourcePropertyRequirements[ResourcePropertyType.Reactivity],
			"Higher tectonic volatility and corrosion should require more reactivity");
	}

	[Fact]
	public void CreatePortalRequirementForPlanet_ResistanceReflectsTemperatureRangeAndCorrosion()
	{
		var extremeTempPlanet = new PlanetaryConstants(
			gravity: 5f, atmosphericPressure: 5f,
			atmosphericCorrosion: 1f, tectonicVolatility: 0.5f,
			lowTemperature: -200f, highTemperature: 200f); // 400 degree range

		var mildTempPlanet = new PlanetaryConstants(
			gravity: 5f, atmosphericPressure: 5f,
			atmosphericCorrosion: 1f, tectonicVolatility: 0.5f,
			lowTemperature: 10f, highTemperature: 30f); // 20 degree range

		var extremeRequirement = PortalRequirement.CreatePortalRequirementForPlanet(extremeTempPlanet);
		var mildRequirement = PortalRequirement.CreatePortalRequirementForPlanet(mildTempPlanet);

		Assert.True(extremeRequirement.ResourcePropertyRequirements[ResourcePropertyType.Resistance] >
			mildRequirement.ResourcePropertyRequirements[ResourcePropertyType.Resistance],
			"Larger temperature range should require more resistance");
	}

	[Fact]
	public void CreatePortalRequirementForPlanet_StrengthReflectsGravity()
	{
		var highGravityPlanet = new PlanetaryConstants(
			gravity: 10f, atmosphericPressure: 5f,
			atmosphericCorrosion: 0.5f, tectonicVolatility: 0.5f,
			lowTemperature: 0f, highTemperature: 100f);

		var lowGravityPlanet = new PlanetaryConstants(
			gravity: 1f, atmosphericPressure: 5f,
			atmosphericCorrosion: 0.5f, tectonicVolatility: 0.5f,
			lowTemperature: 0f, highTemperature: 100f);

		var highRequirement = PortalRequirement.CreatePortalRequirementForPlanet(highGravityPlanet);
		var lowRequirement = PortalRequirement.CreatePortalRequirementForPlanet(lowGravityPlanet);

		Assert.True(highRequirement.ResourcePropertyRequirements[ResourcePropertyType.Strength] >
			lowRequirement.ResourcePropertyRequirements[ResourcePropertyType.Strength],
			"Higher gravity should require more strength");
	}

	[Fact]
	public void CreatePortalRequirementForPlanet_ToughnessReflectsTectonicAndPressure()
	{
		var toughPlanet = new PlanetaryConstants(
			gravity: 5f, atmosphericPressure: 10f,
			atmosphericCorrosion: 0.5f, tectonicVolatility: 1f,
			lowTemperature: 0f, highTemperature: 100f);

		var easyPlanet = new PlanetaryConstants(
			gravity: 5f, atmosphericPressure: 1f,
			atmosphericCorrosion: 0.5f, tectonicVolatility: 0f,
			lowTemperature: 0f, highTemperature: 100f);

		var toughRequirement = PortalRequirement.CreatePortalRequirementForPlanet(toughPlanet);
		var easyRequirement = PortalRequirement.CreatePortalRequirementForPlanet(easyPlanet);

		Assert.True(toughRequirement.ResourcePropertyRequirements[ResourcePropertyType.Toughness] >
			easyRequirement.ResourcePropertyRequirements[ResourcePropertyType.Toughness],
			"Higher tectonic volatility and pressure should require more toughness");
	}

	[Fact]
	public void CreatePortalRequirementForPlanet_IsRandomizedBetweenCalls()
	{
		var planetaryConstants = PlanetaryConstants.Default;

		// Create multiple requirements with same input
		var requirements = Enumerable.Range(0, 10)
			.Select(_ => PortalRequirement.CreatePortalRequirementForPlanet(planetaryConstants))
			.ToList();

		// At least some values should differ due to randomization
		var conductivityValues = requirements
			.Select(r => r.ResourcePropertyRequirements[ResourcePropertyType.Conductivity])
			.Distinct()
			.Count();

		Assert.True(conductivityValues > 1,
			"Expected randomization to produce different conductivity values across multiple calls");
	}

	[Fact]
	public void CreatePortalRequirementForPlanet_RandomizationStaysWithinBounds()
	{
		var planetaryConstants = new PlanetaryConstants(
			gravity: 5f, atmosphericPressure: 5f,
			atmosphericCorrosion: 0.5f, tectonicVolatility: 0.5f,
			lowTemperature: 0f, highTemperature: 100f);

		// Create many requirements to test randomization bounds
		var requirements = Enumerable.Range(0, 100)
			.Select(_ => PortalRequirement.CreatePortalRequirementForPlanet(planetaryConstants))
			.ToList();

		// Calculate base values without randomization
		var baseStrength = planetaryConstants.Gravity * 1.0f; // Strength uses only gravity

		// All strength values should be within 0.75-1.25 of base
		foreach (var req in requirements)
		{
			var strength = req.ResourcePropertyRequirements[ResourcePropertyType.Strength];
			Assert.InRange(strength, baseStrength * 0.75f * 0.95f, baseStrength * 1.25f * 1.05f);
		}
	}

	[Theory]
	[InlineData(0.5f)]
	[InlineData(1.0f)]
	[InlineData(1.5f)]
	[InlineData(2.0f)]
	[InlineData(3.0f)]
	public void CreatePortalRequirementForPlanet_DifferentDifficultyMultipliers(float difficultyMultiplier)
	{
		var planetaryConstants = PlanetaryConstants.Default;

		var requirement = PortalRequirement.CreatePortalRequirementForPlanet(planetaryConstants, difficultyMultiplier);

		Assert.NotNull(requirement);
		Assert.NotNull(requirement.ResourcePropertyRequirements);

		// All values should be positive regardless of difficulty
		foreach (var value in requirement.ResourcePropertyRequirements.Values)
		{
			Assert.True(value > 0);
		}
	}
}
