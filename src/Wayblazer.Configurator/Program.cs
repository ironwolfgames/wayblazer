using System.Text.Json;
using Wayblazer.Core.Config;
using Wayblazer.Core.Generators;
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

	private static GeneratedNameConfig LoadNameConfig(string configFolderPath)
	{
		var nameConfigFile = new FileInfo(Path.Combine(configFolderPath, "name-config.json"));
		var nameConfig = nameConfigFile.Exists ?
			JsonSerializer.Deserialize<GeneratedNameConfig>(File.ReadAllText(nameConfigFile.FullName)) :
			CreateDefaultNameConfig();

		if (!nameConfigFile.Exists)
			File.WriteAllText(nameConfigFile.FullName, JsonSerializer.Serialize(nameConfig, new JsonSerializerOptions { WriteIndented = true }));

		return nameConfig!;
	}

	private static GeneratedNameConfig CreateDefaultNameConfig()
	{
		// Provide a default list of names for resources, energies, etc.
		return new GeneratedNameConfig
		{
			Stems = new List<string>(),
			Prefixes = new List<string>(),
			Suffixes = new List<string>(),
			UniqueNames = new List<string>
			{
				// Energy names
				"Flame", "Spark", "Aether", "Mana", "Essence",
				// Resource names (metals, woods, grounds, gases)
				"Iron", "Copper", "Silver", "Gold", "Steel", "Bronze", "Titanium", "Cobalt",
				"Oak", "Pine", "Birch", "Maple", "Cedar", "Ash", "Willow", "Elm",
				"Clay", "Sand", "Stone", "Granite", "Marble", "Basalt", "Limestone", "Slate",
				"Oxygen", "Nitrogen", "Helium", "Argon", "Neon", "Xenon", "Krypton", "Radon",
				// Composite resource names
				"Alloy", "Compound", "Mixture", "Blend", "Fusion", "Synthesis", "Amalgam", "Composite",
				"Crystal", "Gem", "Ingot", "Bar", "Plate", "Wire", "Sheet", "Powder",
				"Ceramic", "Glass", "Plastic", "Rubber", "Fiber", "Fabric", "Polymer", "Resin",
				"Catalyst", "Reagent", "Solvent", "Solution", "Emulsion", "Colloid", "Suspension", "Gel",
				"Prismatic", "Radiant", "Luminous", "Ethereal", "Celestial", "Astral", "Cosmic", "Void",
				"Arcane", "Mystic", "Enchanted", "Blessed", "Sacred", "Divine", "Holy", "Pure",
				"Refined", "Forged", "Tempered", "Hardened", "Strengthened", "Enhanced", "Improved", "Superior",
				"Advanced", "Complex", "Intricate", "Elaborate", "Sophisticated", "Perfected", "Masterwork", "Legendary"
			}
		};
	}
}
