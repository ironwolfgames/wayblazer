using System.Text.Json;
using Wayblazer.Core.Config;
using Wayblazer.Core.Generators;
using Wayblazer.Core.Models;
using Wayblazer.Core.Utility;

namespace Wayblazer.Configurator;

internal class Program
{
	/// <summary>
	/// Generates five world generator configurations, one for each complexity between 1 and 5, for the given configuration folder path.
	/// </summary>
	/// <param name="configFolderPath">The path to load/generate name lists and world generator configuration files from and to</param>
	/// <param name="seed">The seed to use for random number generation.</param>
	public static void Main(string configFolderPath, int? seed = null)
	{
		Directory.CreateDirectory(configFolderPath);

		var nameConfig = LoadNameConfig(configFolderPath);
		if (seed.HasValue)
			RandomUtility.SetSeed(seed.Value);

		for (var complexity = 1; complexity <= 5; complexity++)
		{
			var worldGeneratorConfig = WorldGeneratorConfigGenerator.GenerateWorldGeneratorConfig(nameConfig, complexity, seed ?? RandomUtility.Next());
			var worldConfigFile = Path.Combine(configFolderPath, $"world-config-{complexity}.json");
			File.WriteAllText(worldConfigFile, JsonSerializer.Serialize(worldGeneratorConfig, new JsonSerializerOptions { WriteIndented = true }));
		}
	}

	private static ResourceNameConfig LoadNameConfig(string configFolderPath)
	{
		var nameConfigFile = new FileInfo(Path.Combine(configFolderPath, "name-config.json"));
		var nameConfig = nameConfigFile.Exists ?
			JsonSerializer.Deserialize<ResourceNameConfig>(File.ReadAllText(nameConfigFile.FullName)) :
			CreateDefaultNameConfig();

		if (!nameConfigFile.Exists)
			File.WriteAllText(nameConfigFile.FullName, JsonSerializer.Serialize(nameConfig, new JsonSerializerOptions { WriteIndented = true }));

		return nameConfig!;
	}

	private static ResourceNameConfig CreateDefaultNameConfig()
	{
		// Provide a default list of names for resources, energies, etc.
		return new ResourceNameConfig
		{
			EnergyNames = new List<string> { "Flame", "Spark", "Aether", "Mana", "Essence" },
			Names = new Dictionary<ResourceKind, List<string>>
			{
				[ResourceKind.Ore] = new List<string> { "Iron", "Copper", "Silver", "Gold", "Steel", "Bronze", "Titanium", "Cobalt" },
				[ResourceKind.Wood] = new List<string> { "Oak", "Pine", "Birch", "Maple", "Cedar", "Ash", "Willow", "Elm" },
				[ResourceKind.Ground] = new List<string> { "Clay", "Sand", "Stone", "Granite", "Marble", "Basalt", "Limestone", "Slate" },
				[ResourceKind.Gas] = new List<string> { "Oxygen", "Nitrogen", "Helium", "Argon", "Neon", "Xenon", "Krypton", "Radon" },
				[ResourceKind.Composite] = new List<string>
				{
					"Alloy", "Compound", "Mixture", "Blend", "Fusion", "Synthesis", "Amalgam", "Composite",
					"Crystal", "Gem", "Ingot", "Bar", "Plate", "Wire", "Sheet", "Powder",
					"Ceramic", "Glass", "Plastic", "Rubber", "Fiber", "Fabric", "Polymer", "Resin",
					"Catalyst", "Reagent", "Solvent", "Solution", "Emulsion", "Colloid", "Suspension", "Gel",
					"Prismatic", "Radiant", "Luminous", "Ethereal", "Celestial", "Astral", "Cosmic", "Void",
					"Arcane", "Mystic", "Enchanted", "Blessed", "Sacred", "Divine", "Holy", "Pure",
					"Refined", "Forged", "Tempered", "Hardened", "Strengthened", "Enhanced", "Improved", "Superior",
					"Advanced", "Complex", "Intricate", "Elaborate", "Sophisticated", "Perfected", "Masterwork", "Legendary"
				}
			}
		};
	}
}
