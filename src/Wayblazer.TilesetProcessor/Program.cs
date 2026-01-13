using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using WFC.Utility;

namespace Wayblazer.TilesetProcessor;

class Program
{
	static void Main(string[] args)
	{
		if (args.Length < 2)
		{
			Console.WriteLine("Usage: Wayblazer.TilesetProcessor <input-tileset-path> <output-json-path> [tile-size]");
			Console.WriteLine("  input-tileset-path: Path to the tileset PNG file");
			Console.WriteLine("  output-json-path: Path to save the generated JSON");
			Console.WriteLine("  tile-size: Optional tile size in pixels (default: 32)");
			Environment.Exit(1);
		}

		string inputPath = args[0];
		string outputPath = args[1];
		int tileSize = args.Length > 2 ? int.Parse(args[2]) : 32;

		Console.WriteLine($"Processing tileset: {inputPath}");
		Console.WriteLine($"Tile size: {tileSize}px");

		if (!File.Exists(inputPath))
		{
			Console.WriteLine($"Error: Input file not found: {inputPath}");
			Environment.Exit(1);
		}

		// Create color-to-biome mapping based on BiomeType enum
		var colorToBiomeMap = CreateColorToBiomeMap();

		// Process the tileset
		var processor = new WFC.Utility.TilesetProcessor(colorToBiomeMap);
		var result = processor.ProcessTileset(inputPath, tileSize);

		Console.WriteLine($"Processed {result.ProtoTiles.Count} tiles");

		// Serialize to JSON
		var options = new JsonSerializerOptions
		{
			WriteIndented = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		string json = JsonSerializer.Serialize(result, options);

		// Ensure output directory exists
		var outputDir = Path.GetDirectoryName(outputPath);
		if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
		{
			Directory.CreateDirectory(outputDir);
		}

		File.WriteAllText(outputPath, json);
		Console.WriteLine($"Output written to: {outputPath}");

		// Print summary
		Console.WriteLine("\nTile Summary:");
		for (int i = 0; i < result.ProtoTiles.Count; i++)
		{
			var tile = result.ProtoTiles[i];
			var biomes = result.TileToBiomes[i];
			var neighborCounts = tile.NeighborIndices.Select(n => n.Count).ToList();

			Console.WriteLine($"  Tile {i}: Biomes={string.Join(",", biomes)}, " +
				$"Neighbors=[N:{neighborCounts[0]}, E:{neighborCounts[1]}, S:{neighborCounts[2]}, W:{neighborCounts[3]}]");
		}
	}

	static Dictionary<Color, int> CreateColorToBiomeMap()
	{
		return new Dictionary<Color, int>
		{
			{ ColorFromHex("#4fa4b8"), 0 }, // Ocean
			{ ColorFromHex("#92e8c0"), 0 }, // Ocean
			{ ColorFromHex("#e3ca8f"), 1 }, // Beach
			{ ColorFromHex("#3b2027"), 1 }, // Beach
			{ ColorFromHex("#ffee83"), 2 }, // Plains
			{ ColorFromHex("#cf752b"), 2 }, // Plains
			{ ColorFromHex("#f0b541"), 3 }, // Desert
			{ ColorFromHex("#ab5130"), 3 }, // Desert
			{ ColorFromHex("#63ab3f"), 4 }, // Jungle
			{ ColorFromHex("#4b852e"), 4 }, // Jungle
			{ ColorFromHex("#3b7d4f"), 5 }, // ForestDeciduous
			{ ColorFromHex("#283540"), 5 }, // ForestDeciduous
			{ ColorFromHex("#2f5753"), 6 }, // ForestConiferous
			{ ColorFromHex("#8f4d57"), 6 }, // ForestConiferous
			{ ColorFromHex("#52333f"), 6 }, // ForestConiferous
			{ ColorFromHex("#f5ffe8"), 7 }, // Tundra
			{ ColorFromHex("#dfe0e8"), 7 }, // Tundra
			{ ColorFromHex("#a3a7c2"), 8 }, // Mountain
			{ ColorFromHex("#686f99"), 8 }, // Mountain
			{ ColorFromHex("#c8d45d"), 9 }, // Swamp
			{ ColorFromHex("#4c6885"), 9 }, // Swamp
		};
	}

	static Color ColorFromHex(string hex)
	{
		hex = hex.TrimStart('#');
		return Color.FromArgb(
			Convert.ToInt32(hex.Substring(0, 2), 16),
			Convert.ToInt32(hex.Substring(2, 2), 16),
			Convert.ToInt32(hex.Substring(4, 2), 16)
		);
	}
}
