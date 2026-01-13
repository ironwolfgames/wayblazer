using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WFC.Core;

namespace WFC.Utility;

/// <summary>
/// Processes a tileset image and generates ProtoTiles for wave function collapse.
/// </summary>
public class TilesetProcessor
{
	public TilesetProcessor(Dictionary<Color, int> colorToBiomeMap)
	{
		_colorToBiomeMap = colorToBiomeMap;
	}

	/// <summary>
	/// Processes a tileset image and generates ProtoTiles with neighbor information.
	/// </summary>
	/// <param name="tilesetPath">Path to the tileset image file</param>
	/// <param name="tileSize">Size of each tile in pixels (assumes square tiles)</param>
	/// <returns>Processing result containing ProtoTiles and biome mappings</returns>
	public TilesetProcessingResult ProcessTileset(string tilesetPath, int tileSize)
	{
		using var bitmap = new Bitmap(tilesetPath);

		// Extract individual tiles
		var tiles = ExtractTiles(bitmap, tileSize);

		// Calculate edge hashes for each tile
		var tileEdgeHashes = new List<TileEdgeData>();
		for (int i = 0; i < tiles.Count; i++)
		{
			var edgeData = CalculateTileEdgeHashes(tiles[i], i);
			tileEdgeHashes.Add(edgeData);
		}

		// Find valid neighbors for each tile
		var protoTiles = new List<ProtoTile>();
		var tileToBiomes = new Dictionary<int, List<int>>();

		for (int i = 0; i < tileEdgeHashes.Count; i++)
		{
			var currentTile = tileEdgeHashes[i];

			// Find neighbors for each cardinal direction
			var neighborIndices = new List<List<int>>
			{
				FindMatchingNeighbors(currentTile.NorthHash, i, tileEdgeHashes, EdgeDirection.South),
				FindMatchingNeighbors(currentTile.EastHash, i, tileEdgeHashes, EdgeDirection.West),
				FindMatchingNeighbors(currentTile.SouthHash, i, tileEdgeHashes, EdgeDirection.North),
				FindMatchingNeighbors(currentTile.WestHash, i, tileEdgeHashes, EdgeDirection.East)
			};

			protoTiles.Add(new ProtoTile
			{
				Id = $"Tile_{i}_{string.Join("_", currentTile.Biomes.Distinct())}",
				Weight = 1,
				NeighborIndices = neighborIndices
			});

			tileToBiomes[i] = currentTile.Biomes.Distinct().ToList();
		}

		return new TilesetProcessingResult
		{
			ProtoTiles = protoTiles,
			TileToBiomes = tileToBiomes
		};
	}

	private List<Bitmap> ExtractTiles(Bitmap tileset, int tileSize)
	{
		var tiles = new List<Bitmap>();
		int tilesX = tileset.Width / tileSize;
		int tilesY = tileset.Height / tileSize;

		for (int y = 0; y < tilesY; y++)
		{
			for (int x = 0; x < tilesX; x++)
			{
				var tile = new Bitmap(tileSize, tileSize);
				using var g = Graphics.FromImage(tile);
				g.DrawImage(tileset,
					new Rectangle(0, 0, tileSize, tileSize),
					new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize),
					GraphicsUnit.Pixel);
				tiles.Add(tile);
			}
		}

		return tiles;
	}

	private TileEdgeData CalculateTileEdgeHashes(Bitmap tile, int tileIndex)
	{
		int size = tile.Width;
		var allBiomes = new List<int>();

		// Sample pixels along each edge (2 pixels per edge, near corners but excluding corners)
		// For a tile of size N, sample at positions 1 and N-2 along each edge
		int sample1 = 1;
		int sample2 = size - 2;

		// North edge (top): sample horizontally
		var northBiome1 = GetBiomeFromColor(tile.GetPixel(sample1, 0));
		var northBiome2 = GetBiomeFromColor(tile.GetPixel(sample2, 0));
		var northHash = new BiomeEdgeHash(northBiome1, northBiome2);
		allBiomes.Add(northBiome1);
		allBiomes.Add(northBiome2);

		// East edge (right): sample vertically
		var eastBiome1 = GetBiomeFromColor(tile.GetPixel(size - 1, sample1));
		var eastBiome2 = GetBiomeFromColor(tile.GetPixel(size - 1, sample2));
		var eastHash = new BiomeEdgeHash(eastBiome1, eastBiome2);
		allBiomes.Add(eastBiome1);
		allBiomes.Add(eastBiome2);

		// South edge (bottom): sample horizontally
		var southBiome1 = GetBiomeFromColor(tile.GetPixel(sample1, size - 1));
		var southBiome2 = GetBiomeFromColor(tile.GetPixel(sample2, size - 1));
		var southHash = new BiomeEdgeHash(southBiome1, southBiome2);
		allBiomes.Add(southBiome1);
		allBiomes.Add(southBiome2);

		// West edge (left): sample vertically
		var westBiome1 = GetBiomeFromColor(tile.GetPixel(0, sample1));
		var westBiome2 = GetBiomeFromColor(tile.GetPixel(0, sample2));
		var westHash = new BiomeEdgeHash(westBiome1, westBiome2);
		allBiomes.Add(westBiome1);
		allBiomes.Add(westBiome2);

		return new TileEdgeData
		{
			TileIndex = tileIndex,
			NorthHash = northHash,
			EastHash = eastHash,
			SouthHash = southHash,
			WestHash = westHash,
			Biomes = allBiomes
		};
	}

	private int GetBiomeFromColor(Color color)
	{
		// Find the closest matching color in the color-to-biome map
		if (_colorToBiomeMap.TryGetValue(color, out int biome))
		{
			return biome;
		}

		// If exact match not found, find closest color
		var closestColor = _colorToBiomeMap.Keys
			.OrderBy(c => ColorDistance(c, color))
			.First();

		return _colorToBiomeMap[closestColor];
	}

	private double ColorDistance(Color c1, Color c2)
	{
		int dr = c1.R - c2.R;
		int dg = c1.G - c2.G;
		int db = c1.B - c2.B;
		return Math.Sqrt(dr * dr + dg * dg + db * db);
	}

	private List<int> FindMatchingNeighbors(BiomeEdgeHash currentEdge, int currentTileIndex,
		List<TileEdgeData> allTiles, EdgeDirection matchDirection)
	{
		var neighbors = new List<int>();
		var complementaryHash = currentEdge.GetComplementary();

		for (int i = 0; i < allTiles.Count; i++)
		{
			// Skip self
			if (i == currentTileIndex)
				continue;

			var otherTile = allTiles[i];
			BiomeEdgeHash otherEdge = matchDirection switch
			{
				EdgeDirection.North => otherTile.NorthHash,
				EdgeDirection.East => otherTile.EastHash,
				EdgeDirection.South => otherTile.SouthHash,
				EdgeDirection.West => otherTile.WestHash,
				_ => throw new ArgumentException("Invalid edge direction")
			};

			if (complementaryHash.Equals(otherEdge))
			{
				neighbors.Add(i);
			}
		}

		return neighbors;
	}

	private readonly Dictionary<Color, int> _colorToBiomeMap;
}

internal enum EdgeDirection
{
	North,
	East,
	South,
	West
}

internal class TileEdgeData
{
	public int TileIndex { get; set; }
	public BiomeEdgeHash NorthHash { get; set; } = null!;
	public BiomeEdgeHash EastHash { get; set; } = null!;
	public BiomeEdgeHash SouthHash { get; set; } = null!;
	public BiomeEdgeHash WestHash { get; set; } = null!;
	public List<int> Biomes { get; set; } = [];
}
