using Wayblazer.Core.Config;
using Wayblazer.Core.Generators;
using Wayblazer.Core.Models;
using Wayblazer.Core.Utility;
using Action = Wayblazer.Core.Models.Action;

namespace Wayblazer.Generators;

public class WorldGenerator
{
	public WorldConfig Generate(WorldGeneratorConfig config)
	{
		// Initialize name queues from config
		_energyNames = new Queue<string>(config.EnergyNames);
		_resourceNames = new Queue<string>(config.ResourceNames);
		_compositeResourceNames = new Queue<string>(config.CompositeResourceNames);

		var energies = GenerateEnergies(config);
		var heatEnergy = energies.Single(e => e.Kind == EnergyKind.Heat);

		var environmentalObjects = new List<EnvironmentalObject>();
		var actions = new List<Action>();
		var buildings = new List<Building>();
		var resources = new List<ResourceInfo>();

		GenerateLevel0Resources(config, resources, environmentalObjects, actions, buildings);

		var furnace = GenerateFurnaceWithWoodToHeatActions(heatEnergy, actions, buildings, resources);
		var metalResources = GenerateMetalResources(heatEnergy, actions, resources, furnace);
		resources.AddRange(metalResources);

		var compositeAction = "Manufacture";
		var factory = new Building("Factory", new List<BuildingAction>());
		buildings.Add(factory);


		List<ResourceInfo> levelTwoResources = GenerateLevelTwoResources(config, energies, actions, resources, metalResources, compositeAction, factory);
		resources.AddRange(levelTwoResources);

		GenerateHigherLevelCompositeResources(config, energies, actions, resources, factory, levelTwoResources, compositeAction);

		var upgrades = GenerateUpgrades(buildings.SelectMany(b => b.Actions).ToList(), resources);

		return new WorldConfig
		{
			Environment = environmentalObjects,
			Resources = resources.Cast<Resource>().ToList(),
			Energy = energies.Cast<Energy>().ToList(),
			Actions = actions,
			Buildings = buildings,
			Upgrades = upgrades,
		};
	}

	private List<EnergyInfo> GenerateEnergies(WorldGeneratorConfig config)
	{
		if (_energyNames is null)
			throw new InvalidOperationException("Energy names not initialized properly");

		var energies = new List<EnergyInfo>();
		var heatEnergy = new EnergyInfo(_energyNames.Dequeue(), EnergyKind.Heat);
		energies.Add(heatEnergy);

		if (config.HasElectricalEnergy)
		{
			var electricityEnergy = new EnergyInfo(_energyNames.Dequeue(), EnergyKind.Electricity);
			energies.Add(electricityEnergy);
		}

		if (config.MagicEnergyCount > 0)
		{
			for (var i = 0; i < config.MagicEnergyCount; i++)
			{
				energies.Add(new EnergyInfo(_energyNames.Dequeue(), EnergyKind.Magic));
			}
		}

		return energies;
	}

	private void GenerateLevel0Resources(WorldGeneratorConfig config, List<ResourceInfo> resources,
		List<EnvironmentalObject> environmentalObjects, List<Action> actions, List<Building> buildings)
	{
		if (_resourceNames is null)
			throw new InvalidOperationException("Resource names not initialized properly");

		var airEnvironmentalObject = new EnvironmentalObject("Air");
		environmentalObjects.Add(airEnvironmentalObject);

		// generate the basic level 0 resources
		foreach (var resourceKindCount in config.ResourceKindCounts.Where(x => x.Key != ResourceKind.Composite))
		{
			var resourceKind = resourceKindCount.Key;
			var count = resourceKindCount.Value;

			if (!s_resourceKindAcquireActions.TryGetValue(resourceKind, out var acquireActionVerb))
			{
				Console.Error.WriteLine($"No acquire action for resource kind {resourceKind}");
				continue;
			}

			var buildingActions = new List<BuildingAction>();
			var currentResourceBatch = new List<ResourceInfo>();
			for (var i = 0; i < count; i++)
			{
				ResourceInfo? resourceInfo = default;

				var acquireActionInputs = new List<SystemObject>();
				if (s_resourceKindEnvironmentalObjects.TryGetValue(resourceKind, out var environmentalObjectName))
				{
					var name = _resourceNames.Dequeue();
					resourceInfo = new ResourceInfo(name, resourceKind, 0);

					var environmentalObject = new EnvironmentalObject($"{name} {environmentalObjectName}");
					environmentalObjects.Add(environmentalObject);
					acquireActionInputs.Add(environmentalObject);
				}
				else if (resourceKind == ResourceKind.Gas)
				{
					var name = _resourceNames.Dequeue();
					resourceInfo = new ResourceInfo(name, resourceKind, 0);

					acquireActionInputs.Add(airEnvironmentalObject);
				}
				else
				{
					Console.Error.WriteLine($"Unsupported base resource kind {resourceKind}");
					continue;
				}

				currentResourceBatch.Add(resourceInfo);

				var acquireAction = new Action($"{acquireActionVerb} {resourceInfo.Name}", acquireActionInputs, [resourceInfo]);
				actions.Add(acquireAction);

				var acquireBuildingAction = new BuildingAction { Action = acquireAction, Time = 1 + RandomUtility.Next(5) };
				buildingActions.Add(acquireBuildingAction);
			}

			if (!s_resourceKindBuildings.TryGetValue(resourceKind, out var buildingName))
			{
				Console.Error.WriteLine($"No building for resource kind {resourceKind}");
				continue;
			}

			var building = new Building(buildingName, buildingActions);
			buildings.Add(building);

			resources.AddRange(currentResourceBatch);
		}
	}

	private static Building GenerateFurnaceWithWoodToHeatActions(EnergyInfo heatEnergy, List<Action> actions, List<Building> buildings, List<ResourceInfo> resources)
	{
		// generate wood to heat energy actions
		var furnace = new Building("Furnace", new List<BuildingAction>());
		buildings.Add(furnace);

		foreach (var wood in resources.Where(x => x.Kind == ResourceKind.Wood))
		{
			var burnWoodAction = new Action($"Burn {wood.Name}", [wood], [heatEnergy]);
			actions.Add(burnWoodAction);

			var burnWoodBuildingAction = new BuildingAction { Action = burnWoodAction, Time = 1 + RandomUtility.Next(5) };
			furnace.Actions.Add(burnWoodBuildingAction);
		}

		return furnace;
	}

	private List<ResourceInfo> GenerateMetalResources(EnergyInfo heatEnergy, List<Action> actions, List<ResourceInfo> resources, Building furnace)
	{
		if (_resourceNames is null)
			throw new InvalidOperationException("Resource names not initialized properly");

		// generate the level one metal resources
		var metalResources = new List<ResourceInfo>();
		foreach (var ore in resources.Where(x => x.Kind == ResourceKind.Ore))
		{
			var smeltedMetalName = _resourceNames.Dequeue();
			var smeltedMetal = new ResourceInfo(smeltedMetalName, ResourceKind.Composite, 1);
			metalResources.Add(smeltedMetal);

			var smeltOreAction = new Action($"Smelt {ore.Name}", new List<SystemObject> { ore, heatEnergy }, new List<SystemObject> { smeltedMetal });
			actions.Add(smeltOreAction);

			var smeltOreBuildingAction = new BuildingAction { Action = smeltOreAction, Time = 1 + RandomUtility.Next(5) };
			furnace.Actions.Add(smeltOreBuildingAction);
		}

		return metalResources;
	}

	private List<ResourceInfo> GenerateLevelTwoResources(WorldGeneratorConfig config, List<EnergyInfo> energies, List<Action> actions, List<ResourceInfo> resources, List<ResourceInfo> metalResources, string compositeAction, Building factory)
	{
		if (_compositeResourceNames is null)
			throw new InvalidOperationException("Composite resource names not initialized properly");

		// generate level 2 composite resources
		var levelTwoResources = new List<ResourceInfo>();
		var basicResourceCount = resources.Count(r => r.Kind == ResourceKind.Wood || r.Kind == ResourceKind.Ground || r.Kind == ResourceKind.Gas) + metalResources.Count;
		var levelTwoCompositeResourceCount = RandomUtility.Next((int)(basicResourceCount * 0.5), basicResourceCount * 2 + 1);
		for (var i = 0; i < levelTwoCompositeResourceCount; i++)
		{
			// select 2-3 input resources and optionally an energy
			var inputCount = 2 + RandomUtility.Next(2);
			var possibleInputs = resources.Where(r => r.Level <= 1).ToList();
			var inputs = new List<SystemObject>();

			for (int j = 0; j < inputCount; j++)
			{
				var input = possibleInputs[RandomUtility.Next(possibleInputs.Count)];
				inputs.Add(input);

				// disallow wood and gas to be inputs at the same time
				if (input.Kind == ResourceKind.Gas)
				{
					possibleInputs = possibleInputs.Where(r => r.Kind != ResourceKind.Wood).ToList();
				}
				else if (input.Kind == ResourceKind.Wood)
				{
					possibleInputs = possibleInputs.Where(r => r.Kind != ResourceKind.Gas).ToList();
				}

				// don't allow the same input twice
				possibleInputs.Remove(input);
			}

			// maybe add energy (50% chance)
			if (RandomUtility.Next(2) == 0)
			{
				var eligibleEnergies = energies.Where(e =>
					!inputs.Any(i => i is ResourceInfo r && r.Kind == ResourceKind.Wood) ||
					(e.Kind != EnergyKind.Electricity && e.Kind != EnergyKind.Heat));

				if (eligibleEnergies.Any())
				{
					var energy = eligibleEnergies.ElementAt(RandomUtility.Next(eligibleEnergies.Count()));
					inputs.Add(energy);
				}
			}

			var name = _compositeResourceNames.Dequeue();
			var composite = new ResourceInfo(name, ResourceKind.Composite, 2);
			levelTwoResources.Add(composite);

			var action = new Action($"{compositeAction} {composite.Name}", inputs, [composite]);
			actions.Add(action);
			factory.Actions.Add(new BuildingAction { Action = action, Time = 2 + RandomUtility.Next(8) });
		}

		return levelTwoResources;
	}

	private void GenerateHigherLevelCompositeResources(WorldGeneratorConfig config, List<EnergyInfo> energies, List<Action> actions, List<ResourceInfo> resources, Building factory, List<ResourceInfo> levelTwoResources, string compositeAction)
	{
		if (_compositeResourceNames is null)
			throw new InvalidOperationException("Composite resource names not initialized properly");

		var currentLevel = 2;
		var currentLevelResources = levelTwoResources;
		var compositeLevelMinimumResourceCount = 2 + RandomUtility.Next(4); // random target minimum level size [2,5]
		while (currentLevelResources.Count > compositeLevelMinimumResourceCount && _compositeResourceNames.Any())
		{
			currentLevel++;
			var prevLevelCount = currentLevelResources.Count;
			var nextLevelSize = RandomUtility.Next(Math.Max(1, prevLevelCount / 3), prevLevelCount + 1);
			var nextLevelResources = new List<ResourceInfo>();

			for (int i = 0; i < nextLevelSize && _compositeResourceNames.Any(); i++)
			{
				// Select 2-3 input resources from previous levels and optionally an energy
				var inputCount = 2 + RandomUtility.Next(2);
				var possibleInputs = resources.Where(r => r.Level < currentLevel).ToList();
				var inputs = new List<SystemObject>();

				for (int j = 0; j < inputCount; j++)
				{
					var input = possibleInputs[RandomUtility.Next(possibleInputs.Count)];
					inputs.Add(input);
					possibleInputs.Remove(input);
				}

				// 40% chance to add magic energy for higher levels
				if (RandomUtility.Next(100) < 40)
				{
					var magicEnergies = energies.Where(e => e.Kind == EnergyKind.Magic);
					if (magicEnergies.Any())
					{
						var magicEnergy = magicEnergies.ElementAt(RandomUtility.Next(magicEnergies.Count()));
						inputs.Add(magicEnergy);
					}
				}

				var name = _compositeResourceNames.Dequeue();
				var composite = new ResourceInfo(name, ResourceKind.Composite, currentLevel);
				nextLevelResources.Add(composite);

				var action = new Action($"{compositeAction} {composite.Name}", inputs, [composite]);
				actions.Add(action);
				factory.Actions.Add(new BuildingAction { Action = action, Time = 3 + RandomUtility.Next(10) });
			}

			resources.AddRange(nextLevelResources);
			currentLevelResources = nextLevelResources;
		}
	}

	private List<Upgrade> GenerateUpgrades(List<BuildingAction> buildingActions, List<ResourceInfo> resources)
	{
		var upgrades = new List<Upgrade>();
		foreach (var buildingAction in buildingActions)
		{
			var upgradeCount = RandomUtility.Next(1, 4);
			for (var i = 0; i < upgradeCount; i++)
			{
				var costs = new List<SystemObject>();
				var benefits = new List<Benefit>();

				// Generate random costs
				var costCount = RandomUtility.Next(1, 4);
				for (var j = 0; j < costCount; j++)
				{
					var cost = resources[RandomUtility.Next(resources.Count)];
					costs.Add(cost);
				}

				// Generate random benefits
				var benefitKind = (BenefitKind)RandomUtility.Next(Enum.GetValues(typeof(BenefitKind)).Length);
				var benefitAmount = RandomUtility.Next(1, 11);
				var benefit = new Benefit($"{buildingAction.Action.Name} Benefit {i + 1}", buildingAction, benefitKind, benefitAmount);
				benefits.Add(benefit);

				var upgrade = new Upgrade($"{buildingAction.Action.Name} Upgrade {i + 1}", costs, benefits);
				upgrades.Add(upgrade);
			}
		}

		return upgrades;
	}

	private static readonly Dictionary<ResourceKind, string> s_resourceKindAcquireActions = new()
	{
		{ ResourceKind.Ore, "Mine" },
		{ ResourceKind.Ground, "Extract" },
		{ ResourceKind.Wood, "Chop" },
		{ ResourceKind.Gas, "Distill" },
		{ ResourceKind.Composite, "Make" },
	};

	private static readonly Dictionary<ResourceKind, string> s_resourceKindBuildings = new()
	{
		{ ResourceKind.Ore, "Mine" },
		{ ResourceKind.Ground, "Extractor" },
		{ ResourceKind.Wood, "Lumber Camp" },
		{ ResourceKind.Gas, "Distiller" },
		{ ResourceKind.Composite, "Factory" },
	};

	private static readonly Dictionary<ResourceKind, string> s_resourceKindEnvironmentalObjects = new()
	{
		{ ResourceKind.Ore, "Deposit" },
		{ ResourceKind.Ground, "Deposit" },
		{ ResourceKind.Wood, "Tree" },
	};

	private Queue<string>? _energyNames;
	private Queue<string>? _resourceNames;
	private Queue<string>? _compositeResourceNames;
}
