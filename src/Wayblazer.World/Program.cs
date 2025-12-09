using Wayblazer.Core.Config;
using Wayblazer.Core.Models;
using Wayblazer.Core.Utility;
using Wayblazer.Generators;
using System.Text.Json;

namespace Wayblazer;

internal class Program
{
	/// <param name="configFileTemplate">The template path to the config file.</param>
	/// <param name="worldFileTemplate">The template path to the world file to output to.</param>
	internal static void Main(string configFileTemplate, string worldFileTemplate)
	{
		int seed = new Random().Next();
		var configFiles = new List<string>
		{
			configFileTemplate.Replace("N", "1"),
			configFileTemplate.Replace("N", "2"),
			configFileTemplate.Replace("N", "3"),
			configFileTemplate.Replace("N", "4"),
			configFileTemplate.Replace("N", "5"),
		};

		var worldNumber = 1;
		foreach (var configFile in configFiles)
		{
			var worldFile = worldFileTemplate.Replace("N", $"{worldNumber++}");
			GenerateWorldConfig(configFile, worldFile, seed);
			PrettyPrintWorldConfig(worldFile);
			Console.WriteLine();
			Console.WriteLine();
		}
	}

	internal static void GenerateWorldConfig(string configFile, string worldFile, int seed)
	{
		if (!File.Exists(configFile))
		{
			Console.WriteLine($"Config file not found: {configFile}");
			Environment.Exit(1);
			return;
		}

		var config = LoadConfig(configFile);
		if (config is null)
		{
			Console.WriteLine($"Failed to load config from config file {configFile}.");
			Environment.Exit(2);
			return;
		}

		if (config.Seed is not null)
		{
			RandomUtility.SetSeed(config.Seed.Value);
		}

		var generator = new WorldGenerator();
		var worldConfig = generator.Generate(config);

		SaveWorldConfig(worldFile, worldConfig);
	}

	internal static void PrettyPrintWorldConfig(string worldFile)
	{
		var outputActions = false;
		var outputBuildings = true;

		var worldConfig = LoadWorldConfig(worldFile);
		if (worldConfig is null)
		{
			Console.WriteLine($"Failed to load world config from world file {worldFile}.");
			return;
		}

		Console.WriteLine($"World: {Path.GetFileNameWithoutExtension(worldFile)}");
		Console.WriteLine($"Energy:\n{string.Join("\n", worldConfig.Energy.Select(x => x.Name))}\n");
		Console.WriteLine($"Wood Resources:\n{string.Join("\n", worldConfig.Resources.Where(x => x.Kind == ResourceKind.Wood).Select(x => x.Name))}\n");
		Console.WriteLine($"Ground Resources:\n{string.Join("\n", worldConfig.Resources.Where(x => x.Kind == ResourceKind.Ground).Select(x => x.Name))}\n");
		Console.WriteLine($"Ore Resources:\n{string.Join("\n", worldConfig.Resources.Where(x => x.Kind == ResourceKind.Ore).Select(x => x.Name))}\n");
		Console.WriteLine($"Gas Resources:\n{string.Join("\n", worldConfig.Resources.Where(x => x.Kind == ResourceKind.Gas).Select(x => x.Name))}\n");
		var compositeResourcesByLevel = worldConfig.Resources
			.Where(x => x.Kind == ResourceKind.Composite)
			.GroupBy(x => x.Level)
			.OrderBy(x => x.Key);
		foreach (var compositeResources in compositeResourcesByLevel)
		{
			Console.WriteLine($"Composite Resources (Level {compositeResources.Key}):\n{string.Join("\n", compositeResources.Select(x => x.Name))}\n");
		}

		Console.WriteLine($"Environmental Objects:\n{string.Join("\n", worldConfig.Environment.Select(x => x.Name))}\n");

		if (outputActions)
		{
			Console.WriteLine("Actions:");
			foreach (var action in worldConfig.Actions)
			{
				if (action.Inputs.Any(x => x is EnvironmentalObject))
					continue;

				Console.WriteLine($"{action.Name}: {string.Join(" + ", action.Inputs.Select(x => x.Name))} -> {string.Join(" + ", action.Outputs.Select(x => x.Name))}");
			}
		}

		if (outputBuildings)
		{
			Console.WriteLine("Buildings:");
			foreach (var building in worldConfig.Buildings)
			{
				Console.WriteLine($"{building.Name}:");
				foreach (var buildingAction in building.Actions)
				{
					Console.WriteLine($"\t{buildingAction.Action.Name} in {buildingAction.Time} seconds");
				}
			}
		}
	}

	private static WorldGeneratorConfig? LoadConfig(string path)
	{
		return JsonSerializer.Deserialize<WorldGeneratorConfig>(File.ReadAllText(path));
	}

	private static WorldConfig? LoadWorldConfig(string path)
	{
		return JsonSerializer.Deserialize<WorldConfig>(File.ReadAllText(path));
	}

	private static void SaveWorldConfig(string path, WorldConfig config)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		File.WriteAllText(path, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
	}
}
