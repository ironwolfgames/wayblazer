using Wayblazer.Core.Config;
using Wayblazer.Core.Models;
using Wayblazer.Core.Utility;

namespace Wayblazer.Core.Generators;

public static class WorldGeneratorConfigGenerator
{
	public static WorldGeneratorConfig GenerateWorldGeneratorConfig(GeneratedNameConfig nameConfig, int complexity, int seed)
	{
		bool hasElectricalEnergy = RandomUtility.NextBool();
		var magicEnergyCount = RandomUtility.Next(0, complexity - 1);

		var minResourceKindCount = (int)Math.Max(1, 0.7 * complexity);
		var maxResourceKindCount = (int)Math.Round(1.6 * complexity);
		var resourceKindCounts = new Dictionary<ResourceKind, int>
		{
			[ResourceKind.Ore] = RandomUtility.Next(minResourceKindCount, maxResourceKindCount),
			[ResourceKind.Ground] = RandomUtility.Next(minResourceKindCount, maxResourceKindCount),
			[ResourceKind.Wood] = RandomUtility.Next(minResourceKindCount, maxResourceKindCount),
			[ResourceKind.Gas] = RandomUtility.Next(minResourceKindCount, maxResourceKindCount)
		};

		var energyCount = 1 + (hasElectricalEnergy ? 1 : 0) + magicEnergyCount;
		var baseResourceCount = resourceKindCounts.Values.Sum();
		var maxLevelTwoCompositeResourceCount = 2 * baseResourceCount;
		var currentLevelMaxResourceCount = (int)Math.Ceiling(maxLevelTwoCompositeResourceCount * 0.5);
		var maxLevelThreeAndUpCompositeResourceCount = currentLevelMaxResourceCount;
		while (currentLevelMaxResourceCount > 5)
		{
			currentLevelMaxResourceCount = (int)Math.Ceiling(currentLevelMaxResourceCount * 0.5);
			maxLevelThreeAndUpCompositeResourceCount += currentLevelMaxResourceCount;
		}

		var totalResourceNameCount = baseResourceCount + baseResourceCount; // base + metals
		var totalCompositeNameCount = maxLevelTwoCompositeResourceCount + maxLevelThreeAndUpCompositeResourceCount;

		// Shuffle and take required names from the config
		var shuffledUniqueNames = nameConfig.UniqueNames.OrderBy(x => RandomUtility.Next()).ToList();
		var energyNames = shuffledUniqueNames.Take(energyCount).ToList();
		var resourceNames = shuffledUniqueNames.Skip(energyCount).Take(totalResourceNameCount).ToList();
		var compositeResourceNames = shuffledUniqueNames.Skip(energyCount + totalResourceNameCount).Take(totalCompositeNameCount).ToList();

		return new WorldGeneratorConfig
		{
			Seed = seed,
			EnergyNames = energyNames,
			ResourceNames = resourceNames,
			CompositeResourceNames = compositeResourceNames,
			HasElectricalEnergy = hasElectricalEnergy,
			MagicEnergyCount = magicEnergyCount,
			ResourceKindCounts = resourceKindCounts
		};
	}
}
