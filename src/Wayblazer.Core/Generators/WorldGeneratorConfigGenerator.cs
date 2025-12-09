using Wayblazer.Core.Config;
using Wayblazer.Core.Models;
using Wayblazer.Core.Utility;

namespace Wayblazer.Core.Generators;

public static class WorldGeneratorConfigGenerator
{
	public static WorldGeneratorConfig GenerateWorldGeneratorConfig(ResourceNameConfig nameConfig, int complexity, int seed)
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

        // Calculate total composite names needed: Metals (from Ores) + Level 2+ Composites
        var metalCount = resourceKindCounts[ResourceKind.Ore];
		var totalCompositeNameCount = metalCount + maxLevelTwoCompositeResourceCount + maxLevelThreeAndUpCompositeResourceCount;

		// Shuffle and take required names from the config
        var resourceNames = new Dictionary<ResourceKind, List<string>>();

        foreach (var kvp in resourceKindCounts)
        {
            var kind = kvp.Key;
            var count = kvp.Value;
            if (nameConfig.Names.TryGetValue(kind, out var availableNames))
            {
                resourceNames[kind] = availableNames.OrderBy(x => RandomUtility.Next()).Take(count).ToList();
            }
            else
            {
                resourceNames[kind] = new List<string>();
            }
        }

        if (nameConfig.Names.TryGetValue(ResourceKind.Composite, out var compositeNames))
        {
             resourceNames[ResourceKind.Composite] = compositeNames.OrderBy(x => RandomUtility.Next()).Take(totalCompositeNameCount).ToList();
        }
        else
        {
             resourceNames[ResourceKind.Composite] = new List<string>();
        }

		var energyNames = nameConfig.EnergyNames.OrderBy(x => RandomUtility.Next()).Take(energyCount).ToList();

		return new WorldGeneratorConfig
		{
			Seed = seed,
			EnergyNames = energyNames,
			ResourceNames = resourceNames,
			HasElectricalEnergy = hasElectricalEnergy,
			MagicEnergyCount = magicEnergyCount,
			ResourceKindCounts = resourceKindCounts
		};
	}
}
