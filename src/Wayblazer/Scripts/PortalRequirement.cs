using System;
using Godot.Collections;

namespace Wayblazer;

public class PortalRequirement
{
	public Dictionary<ResourcePropertyType, float> ResourcePropertyRequirements { get; }

	private PortalRequirement(Dictionary<ResourcePropertyType, float> resourcePropertyRequirements)
	{
		ResourcePropertyRequirements = resourcePropertyRequirements;
	}

	public static PortalRequirement CreatePortalRequirementForPlanet(PlanetaryConstants planetaryConstants, float difficultyMultiplier = 1.0f)
	{
		var rand = new Random();
		var resourcePropertyRequirements = new Dictionary<ResourcePropertyType, float>
		{
			{ ResourcePropertyType.Conductivity, (planetaryConstants.AtmosphericPressure * 0.5f + planetaryConstants.Gravity * 0.5f) * GetRandomizedMultiplier(rand, c_conductivityMultiplierMinimum, c_conductivityMultiplierMaximum) * difficultyMultiplier },
			{ ResourcePropertyType.Reactivity, (planetaryConstants.TectonicVolatility * 0.7f + planetaryConstants.AtmosphericCorrosion * 0.3f) * GetRandomizedMultiplier(rand, c_reactivityMultiplierMinimum, c_reactivityMultiplierMaximum) * difficultyMultiplier },
			{ ResourcePropertyType.Resistance, ((planetaryConstants.HighTemperature - planetaryConstants.LowTemperature) * 0.5f + planetaryConstants.AtmosphericCorrosion * 0.5f) * GetRandomizedMultiplier(rand, c_resistanceMultiplierMinimum, c_resistanceMultiplierMaximum) * difficultyMultiplier },
			{ ResourcePropertyType.Strength, planetaryConstants.Gravity * GetRandomizedMultiplier(rand, c_strengthMultiplierMinimum, c_strengthMultiplierMaximum) * difficultyMultiplier },
			{ ResourcePropertyType.Toughness, (planetaryConstants.TectonicVolatility * 0.65f + planetaryConstants.AtmosphericPressure * 0.35f) * GetRandomizedMultiplier(rand, c_toughnessMultiplierMinimum, c_toughnessMultiplierMaximum) * difficultyMultiplier },
		};

		return new PortalRequirement(resourcePropertyRequirements);
	}

	private static float GetRandomizedMultiplier(Random rand, float min, float max)
	{
		return (float)(min + (rand.NextDouble() * (max - min)));
	}

	private const float c_conductivityMultiplierMinimum = 0.75f;
	private const float c_conductivityMultiplierMaximum = 1.25f;
	private const float c_reactivityMultiplierMinimum = 0.75f;
	private const float c_reactivityMultiplierMaximum = 1.25f;
	private const float c_resistanceMultiplierMinimum = 0.75f;
	private const float c_resistanceMultiplierMaximum = 1.25f;
	private const float c_strengthMultiplierMinimum = 0.75f;
	private const float c_strengthMultiplierMaximum = 1.25f;
	private const float c_toughnessMultiplierMinimum = 0.75f;
	private const float c_toughnessMultiplierMaximum = 1.25f;
}
