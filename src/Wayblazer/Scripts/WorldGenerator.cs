using System;
using System.Linq;
using Godot;
using Godot.Collections;
using WFC.Core;

namespace Wayblazer;

[Tool]
[GlobalClass]
public partial class WorldGenerator : TileMapLayer
{
	/// <summary>
	/// Width of the world in tiles.
	/// </summary>
	[Export]
	public int Width { get; set; } = 1024;

	/// <summary>
	/// Height of the world in tiles.
	/// </summary>
	[Export]
	public int Height { get; set; } = 1024;

	/// <summary>
	/// Seed for random number generation. Use -1 for a random seed.
	/// </summary>
	[Export]
	public int Seed { get; set; } = -1;

	[ExportGroup("Ocean Configuration")]
	/// <summary>
	/// When enabled, generates oceans and continents using a low-frequency noise map.
	/// </summary>
	[Export]
	public bool EnableOceanContinentGeneration { get; set; } = true;
	/// <summary>
	// Threshold for ocean vs continent (values below this become ocean)
	/// </summary>
	[Export]
	public float OceanContinentNoiseMapThreshold { get; set; } = 0.45f;

	/// <summary>
	/// Frequency of the noise map used to determine ocean vs continent placement.
	/// </summary>
	[Export]
	public float OceanContinentNoiseMapFrequency { get; set; } = 0.009f;

	/// <summary>
	/// Number of octaves for the ocean/continent noise map. Higher values add more detail to coastlines.
	/// </summary>
	[Export]
	public int OceanContinentNoiseMapOctaves { get; set; } = 6;

	/// <summary>
	/// When enabled, forces all map edges to be ocean with a smooth falloff gradient.
	/// </summary>
	[Export]
	public bool EdgesAreOcean { get; set; } = false;

	/// <summary>
	/// Minimum distance in tiles from the edge where ocean falloff begins. Used per cardinal direction.
	/// </summary>
	[Export]
	public float EdgeFalloffDistanceMin { get; set; } = 5.0f;

	/// <summary>
	/// Maximum distance in tiles from the edge where ocean falloff begins. Used per cardinal direction.
	/// </summary>
	[Export]
	public float EdgeFalloffDistanceMax { get; set; } = 15.0f;

	/// <summary>
	/// Frequency of the noise map used to create irregular coastlines when EdgesAreOcean is enabled.
	/// </summary>
	[Export]
	public float OceanEdgeNoiseMapFrequency { get; set; } = 0.05f;

	[ExportGroup("World Map Configuration")]
	/// <summary>
	/// Noise configuration for the base height map used in biome generation.
	/// </summary>
	[Export]
	public NoiseLayerConfig WorldMapNoiseConfig { get; set; } = new NoiseLayerConfig();

	/// <summary>
	/// Noise configuration for temperature/latitude variation. Low frequency creates large continental climate zones.
	/// </summary>
	[Export]
	public NoiseLayerConfig TemperatureNoiseConfig { get; set; } = new NoiseLayerConfig() { Frequency = 0.03f, Octaves = 4 };

	/// <summary>
	/// How much the temperature noise affects biome boundaries (0.0 to 1.0). Higher values create more variation.
	/// </summary>
	[Export]
	public float TemperatureNoiseInfluence { get; set; } = 0.3f;

	/// <summary>
	/// Defines which biomes appear at which height and temperature/latitude ranges.
	/// </summary>
	[Export]
	public Array<BiomeRange> BiomeRanges { get; set; } = new Array<BiomeRange>();

	[ExportGroup("Decoration Configuration")]
	/// <summary>
	/// Configuration for placing environmental decorations (trees, rocks, etc.) across the world.
	/// </summary>
	[Export]
	public Array<EnvironmentalDecorationPlacementConfig> DecorationConfigs { get; set; } = new Array<EnvironmentalDecorationPlacementConfig>();

	[ExportGroup("Tile Rendering Configuration")]
	/// <summary>
	/// Expansion factor for tile resolution. Each biome cell becomes NxN tiles on the final tilemap.
	/// Higher values create more detailed transitions but increase memory and generation time.
	/// </summary>
	[Export]
	public int TileExpansionFactor { get; set; } = 3;

	/// <summary>
	/// Radius for sampling neighboring biomes when calculating probabilities.
	/// Larger values create smoother, more gradual transitions.
	/// </summary>
	[Export]
	public int BiomeSampleRadius { get; set; } = 2;

	/// <summary>
	/// Optional separate TileMapLayer for environmental decorations (trees, rocks, etc.).
	/// If not set, decorations will be placed on the main layer.
	/// </summary>
	[Export]
	public TileMapLayer? DecorationLayer { get; set; }

	public override void _Ready()
	{
		// Don't auto-generate in editor mode
		if (Engine.IsEditorHint())
			return;

		GenerateWorldFromZero();
	}

	public void GenerateWorldFromZero()
	{
		Clear();
		DecorationLayer?.Clear();
		foreach (Node child in GetChildren())
		{
			child.QueueFree();
		}

		_worldMapBiomeTypes = new BiomeType[Width, Height];

		GlobalRandom.InitializeWithSeed(Seed == -1 ? (int)GD.Randi() : Seed);

		InitializeResources();
		GenerateWorldData();
		GD.Print($"World data generated with seed: {GlobalRandom.Seed}");

		RenderWorld();
	}

	public void GenerateWorldData()
	{
		if (WorldMapNoiseConfig is null)
		{
			WorldMapNoiseConfig = new NoiseLayerConfig();
		}

		var worldHeightMap = NoiseService.GenerateNoiseMap(Width, Height, GlobalRandom.Seed, WorldMapNoiseConfig);

		// Generate temperature/latitude noise for more organic biome boundaries
		if (TemperatureNoiseConfig is null)
		{
			TemperatureNoiseConfig = new NoiseLayerConfig() { Frequency = 0.03f, Octaves = 4 };
		}
		var temperatureNoiseMap = NoiseService.GenerateNoiseMap(Width, Height, GlobalRandom.Seed + TEMPERATURE_NOISE_SEED_OFFSET, TemperatureNoiseConfig);

		ApplyOceanOverlays(worldHeightMap);

		if (BiomeRanges is null || BiomeRanges.Count == 0)
		{
			BiomeRanges = GetDefaultBiomeRanges();
		}

		// Assign biomes to each tile based on the height map
		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				// Calculate base equator value (0.0 at equator, 1.0 at poles)
				var equatorValue = (float)Math.Abs(y - (Height / 2)) / (Height / 2);

				_worldMapBiomeTypes![x, y] = GetBiomeAt(worldHeightMap[x, y], temperatureNoiseMap[x, y], BiomeRanges, equatorValue);
			}
		}

		// Use the exported decoration configs, or create defaults if empty
		if (DecorationConfigs is null || DecorationConfigs.Count == 0)
		{
			DecorationConfigs = GetDefaultDecorationConfigs();
		}

		// Initialize resources if not already done
		if (_resources is null)
		{
			InitializeResources();
		}
	}

	public void RenderWorld()
	{
		const bool useWFC = false;
		System.Collections.Generic.List<int> protoTileIndices;

		// Calculate expanded dimensions
		int expandedWidth = Width * TileExpansionFactor;
		int expandedHeight = Height * TileExpansionFactor;

		// Hard coded proto tiles for now. In the future, load these from the source WFC image and configuration file
		System.Collections.Generic.List<(ProtoTile Tile, int X, int Y)> protoTileInfos =
		[
			(new()
			{
				// 0 - Ocean
				Id = "ocean",
				Weight = 20,
				NeighborIndices =
				[
					[ (int)BiomeType.Ocean, (int)BiomeType.Beach, (int)BiomeType.Swamp ], // up
					[ (int)BiomeType.Ocean, (int)BiomeType.Beach, (int)BiomeType.Swamp ], // right
					[ (int)BiomeType.Ocean, (int)BiomeType.Beach, (int)BiomeType.Swamp ], // down
					[ (int)BiomeType.Ocean, (int)BiomeType.Beach, (int)BiomeType.Swamp ],  // left
				]
			}, 10, 1),
			(new()
			{
				// 1 - Beach
				Id = "beach",
				Weight = 5,
				NeighborIndices =
				[
					[ (int)BiomeType.Ocean, (int)BiomeType.Beach, (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.Jungle, (int)BiomeType.Swamp ], // up
					[ (int)BiomeType.Ocean, (int)BiomeType.Beach, (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.Jungle, (int)BiomeType.Swamp ], // right
					[ (int)BiomeType.Ocean, (int)BiomeType.Beach, (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.Jungle, (int)BiomeType.Swamp ], // down
					[ (int)BiomeType.Ocean, (int)BiomeType.Beach, (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.Jungle, (int)BiomeType.Swamp ]  // left
				]
			}, 1, 1),
			(new()
			{
				// 2 - Plains
				Id = "plains",
				Weight = 15,
				NeighborIndices =
				[
					[ (int)BiomeType.Beach, (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.ForestDeciduous, (int)BiomeType.Mountain ], // up
					[ (int)BiomeType.Beach, (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.ForestDeciduous, (int)BiomeType.Mountain ], // right
					[ (int)BiomeType.Beach, (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.ForestDeciduous, (int)BiomeType.Mountain ], // down
					[ (int)BiomeType.Beach, (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.ForestDeciduous, (int)BiomeType.Mountain ]  // left
				]
			}, 4, 1),
			(new()
			{
				// 3 - Desert
				Id = "desert",
				Weight = 8,
				NeighborIndices =
				[
					[ (int)BiomeType.Beach, (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.Mountain ], // up
					[ (int)BiomeType.Beach, (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.Mountain ], // right
					[ (int)BiomeType.Beach, (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.Mountain ], // down
					[ (int)BiomeType.Beach, (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.Mountain ]  // left
				]
			}, 7, 1),
			(new()
			{
				// 4 - Jungle
				Id = "jungle",
				Weight = 8,
				NeighborIndices =
				[
					[ (int)BiomeType.Beach, (int)BiomeType.Jungle, (int)BiomeType.ForestDeciduous, (int)BiomeType.Swamp ], // up
					[ (int)BiomeType.Beach, (int)BiomeType.Jungle, (int)BiomeType.ForestDeciduous, (int)BiomeType.Swamp ], // right
					[ (int)BiomeType.Beach, (int)BiomeType.Jungle, (int)BiomeType.ForestDeciduous, (int)BiomeType.Swamp ], // down
					[ (int)BiomeType.Beach, (int)BiomeType.Jungle, (int)BiomeType.ForestDeciduous, (int)BiomeType.Swamp ]  // left
				]
			}, 1, 4),
			(new()
			{
				// 5 - ForestDeciduous
				Id = "forest_deciduous",
				Weight = 12,
				NeighborIndices =
				[
					[ (int)BiomeType.Plains, (int)BiomeType.Jungle, (int)BiomeType.ForestDeciduous, (int)BiomeType.ForestConiferous ], // up
					[ (int)BiomeType.Plains, (int)BiomeType.Jungle, (int)BiomeType.ForestDeciduous, (int)BiomeType.ForestConiferous ], // right
					[ (int)BiomeType.Plains, (int)BiomeType.Jungle, (int)BiomeType.ForestDeciduous, (int)BiomeType.ForestConiferous ], // down
					[ (int)BiomeType.Plains, (int)BiomeType.Jungle, (int)BiomeType.ForestDeciduous, (int)BiomeType.ForestConiferous ]  // left
				]
			}, 4, 4),
			(new()
			{
				// 6 - ForestConiferous
				Id = "forest_coniferous",
				Weight = 10,
				NeighborIndices =
				[
					[ (int)BiomeType.ForestDeciduous, (int)BiomeType.ForestConiferous, (int)BiomeType.Tundra, (int)BiomeType.Mountain ], // up
					[ (int)BiomeType.ForestDeciduous, (int)BiomeType.ForestConiferous, (int)BiomeType.Tundra, (int)BiomeType.Mountain ], // right
					[ (int)BiomeType.ForestDeciduous, (int)BiomeType.ForestConiferous, (int)BiomeType.Tundra, (int)BiomeType.Mountain ], // down
					[ (int)BiomeType.ForestDeciduous, (int)BiomeType.ForestConiferous, (int)BiomeType.Tundra, (int)BiomeType.Mountain ]  // left
				]
			}, 7, 4),
			(new()
			{
				// 7 - Tundra
				Id = "tundra",
				Weight = 6,
				NeighborIndices =
				[
					[ (int)BiomeType.ForestConiferous, (int)BiomeType.Tundra, (int)BiomeType.Mountain ], // up
					[ (int)BiomeType.ForestConiferous, (int)BiomeType.Tundra, (int)BiomeType.Mountain ], // right
					[ (int)BiomeType.ForestConiferous, (int)BiomeType.Tundra, (int)BiomeType.Mountain ], // down
					[ (int)BiomeType.ForestConiferous, (int)BiomeType.Tundra, (int)BiomeType.Mountain ]  // left
				]
			}, 1, 7),
			(new()
			{
				// 8 - Mountain
				Id = "mountain",
				Weight = 8,
				NeighborIndices =
				[
					[ (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.ForestConiferous, (int)BiomeType.Tundra, (int)BiomeType.Mountain ], // up
					[ (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.ForestConiferous, (int)BiomeType.Tundra, (int)BiomeType.Mountain ], // right
					[ (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.ForestConiferous, (int)BiomeType.Tundra, (int)BiomeType.Mountain ], // down
					[ (int)BiomeType.Plains, (int)BiomeType.Desert, (int)BiomeType.ForestConiferous, (int)BiomeType.Tundra, (int)BiomeType.Mountain ]  // left
				]
			}, 4, 7),
			(new()
			{
				// 9 - Swamp
				Id = "swamp",
				Weight = 5,
				NeighborIndices =
				[
					[ (int)BiomeType.Ocean, (int)BiomeType.Beach, (int)BiomeType.Jungle, (int)BiomeType.Swamp ], // up
					[ (int)BiomeType.Ocean, (int)BiomeType.Beach, (int)BiomeType.Jungle, (int)BiomeType.Swamp ], // right
					[ (int)BiomeType.Ocean, (int)BiomeType.Beach, (int)BiomeType.Jungle, (int)BiomeType.Swamp ], // down
					[ (int)BiomeType.Ocean, (int)BiomeType.Beach, (int)BiomeType.Jungle, (int)BiomeType.Swamp ]  // left
				]
			}, 7, 7)
		];

		// Use WFC (Wave Function Collapse) to generate the world's base tiles
#pragma warning disable CS0162 // Unreachable code detected
		if (useWFC)
		{
			var configuration = new Configuration(protoTileInfos.Select(x => x.Tile).ToList(), AdjacencyAlgorithmKind.ADJACENCY_2D);
			var output = new Output(configuration, width: expandedWidth, height: expandedHeight, depth: 1, getInitialValidProtoTilesForPosition: (x, y, z) =>
				{
					// Get biome probabilities at this expanded tile position
					var biomeProbabilities = GetBiomeProbabilities(x, y);

					// Allow proto tiles for all biomes with significant probability (>5%)
					var validTiles = new System.Collections.Generic.List<ProtoTile>();
					foreach (var kvp in biomeProbabilities)
					{
						if (kvp.Value > 0.05f)
						{
							var protoTileIndex = (int)kvp.Key;
							if (protoTileIndex >= 0 && protoTileIndex < protoTileInfos.Count)
							{
								// Weight the tile by its probability
								var tile = protoTileInfos[protoTileIndex].Tile;
								validTiles.Add(tile);
							}
						}
					}

					return validTiles.Count > 0 ? validTiles : protoTileInfos.Select(x => x.Tile).ToList();
				});
			var algorithm = new Algorithm(configuration, seed: GlobalRandom.Seed);
			algorithm.Run(output);

			// Get the proto tile indices from the WFC output
			protoTileIndices = output.ToSerializable().Tiles;
		}
#pragma warning restore CS0162
		else
		{
			// Simple mode: use biome probabilities to select tiles with weighted randomness
			protoTileIndices = new System.Collections.Generic.List<int>(expandedWidth * expandedHeight);
			for (int y = 0; y < expandedHeight; y++)
			{
				for (int x = 0; x < expandedWidth; x++)
				{
					var biomeProbabilities = GetBiomeProbabilities(x, y);

					// Select biome based on weighted random selection
					float randomValue = GlobalRandom.NextFloat(0f, 1f);
					float cumulativeProbability = 0f;
					BiomeType selectedBiome = BiomeType.Ocean;

					foreach (var kvp in biomeProbabilities.OrderByDescending(kv => kv.Value))
					{
						cumulativeProbability += kvp.Value;
						if (randomValue <= cumulativeProbability)
						{
							selectedBiome = kvp.Key;
							break;
						}
					}

					protoTileIndices.Add((int)selectedBiome);
				}
			}
		}

		// Generate noise maps for all decoration types at expanded resolution
		var decorationNoiseMaps = GenerateDecorationNoiseMaps(expandedWidth, expandedHeight);

		// Create a lookup for decoration tile atlas coordinates
		// TODO: Load these from configuration or tileset metadata
		var decorationTileCoords = new System.Collections.Generic.Dictionary<EnvironmentalDecorationType, Vector2I[]>
		{
			{ EnvironmentalDecorationType.Tree, new[] { new Vector2I(0, 10), new Vector2I(1, 10), new Vector2I(2, 10) } },
			{ EnvironmentalDecorationType.Rock, new[] { new Vector2I(3, 10), new Vector2I(4, 10) } },
			{ EnvironmentalDecorationType.Bush, new[] { new Vector2I(5, 10) } },
			{ EnvironmentalDecorationType.Flower, new[] { new Vector2I(6, 10) } },
			{ EnvironmentalDecorationType.Grass, new[] { new Vector2I(7, 10) } },
			{ EnvironmentalDecorationType.OreDeposit, new[] { new Vector2I(8, 10) } },
			{ EnvironmentalDecorationType.GasDeposit, new[] { new Vector2I(9, 10) } },
		};

		// Iterate through the world and set base tiles
		for (int x = 0; x < expandedWidth; x++)
		{
			for (int y = 0; y < expandedHeight; y++)
			{
				var protoTileIndex = protoTileIndices[x + y * expandedWidth];
				var protoTileInfo = protoTileInfos[protoTileIndex];
				SetCell(new Vector2I(x, y), sourceId: 0, atlasCoords: new Vector2I(protoTileInfo.X, protoTileInfo.Y));
			}
		}

		// Place decorations on the decoration layer at expanded resolution
		if (DecorationLayer != null && DecorationConfigs != null)
		{
			for (int x = 0; x < expandedWidth; x++)
			{
				for (int y = 0; y < expandedHeight; y++)
				{
					var biomeProbabilities = GetBiomeProbabilities(x, y);
					var dominantBiome = biomeProbabilities.OrderByDescending(kvp => kvp.Value).First().Key;

					foreach (var config in DecorationConfigs)
					{
						if (!decorationNoiseMaps.ContainsKey(config.DecorationType))
							continue;

						if (!decorationTileCoords.ContainsKey(config.DecorationType))
							continue;

						var noiseValue = decorationNoiseMaps[config.DecorationType][x, y];
						if (ShouldPlaceDecoration(dominantBiome, config, noiseValue))
						{
							var tileVariants = decorationTileCoords[config.DecorationType];
							var selectedTile = tileVariants[GlobalRandom.Next(0, tileVariants.Length)];

							DecorationLayer.SetCell(new Vector2I(x, y), sourceId: 0, atlasCoords: selectedTile);

							// Only place one decoration per tile
							// TODO: ensure we order decorations by priority
							break;
						}
					}
				}
			}
		}

		GD.Print($"World rendered: {expandedWidth}x{expandedHeight} tiles (from {Width}x{Height} biome map, {TileExpansionFactor}x expansion)");
	}

	private void ApplyOceanOverlays(float[,] heightMap)
	{
		float[,]? oceanContinentNoiseMap = null;
		if (EnableOceanContinentGeneration)
		{
			var oceanContinentNoiseConfig = new NoiseLayerConfig
			{
				Frequency = OceanContinentNoiseMapFrequency,
				Octaves = OceanContinentNoiseMapOctaves,
				Lacunarity = 2.0f,
				Persistence = 0.5f
			};
			oceanContinentNoiseMap = NoiseService.GenerateNoiseMap(Width, Height, GlobalRandom.Seed + OCEAN_CONTINENT_NOISE_SEED_OFFSET, oceanContinentNoiseConfig);
		}

		float[,]? edgeNoiseMap = null;
		var northFalloff = 0f;
		var southFalloff = 0f;
		var eastFalloff = 0f;
		var westFalloff = 0f;
		if (EdgesAreOcean)
		{
			northFalloff = EdgeFalloffDistanceMin + GlobalRandom.NextFloat(EdgeFalloffDistanceMin, EdgeFalloffDistanceMax);
			southFalloff = EdgeFalloffDistanceMin + GlobalRandom.NextFloat(EdgeFalloffDistanceMin, EdgeFalloffDistanceMax);
			eastFalloff = EdgeFalloffDistanceMin + GlobalRandom.NextFloat(EdgeFalloffDistanceMin, EdgeFalloffDistanceMax);
			westFalloff = EdgeFalloffDistanceMin + GlobalRandom.NextFloat(EdgeFalloffDistanceMin, EdgeFalloffDistanceMax);

			var edgeNoiseConfig = new NoiseLayerConfig
			{
				Frequency = OceanEdgeNoiseMapFrequency,
				Octaves = 4,
				Lacunarity = 2.0f,
				Persistence = 0.5f
			};
			edgeNoiseMap = NoiseService.GenerateNoiseMap(Width, Height, GlobalRandom.Seed + EDGE_NOISE_SEED_OFFSET, edgeNoiseConfig);
		}

		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				var finalFactor = 1.0f;

				// Apply continent/ocean noise to create large landmasses and oceans
				if (EnableOceanContinentGeneration && oceanContinentNoiseMap != null)
				{
					if (oceanContinentNoiseMap[x, y] < OceanContinentNoiseMapThreshold)
					{
						finalFactor *= 0.2f;
					}
				}

				// Apply edge ocean falloff if enabled
				if (EdgesAreOcean && edgeNoiseMap != null)
				{
					var distanceToNorth = y;
					var distanceToSouth = Height - 1 - y;
					var distanceToWest = x;
					var distanceToEast = Width - 1 - x;

					var minimumDistance = distanceToNorth;
					var falloffDistance = northFalloff;
					if (distanceToSouth < minimumDistance)
					{
						minimumDistance = distanceToSouth;
						falloffDistance = southFalloff;
					}
					if (distanceToWest < minimumDistance)
					{
						minimumDistance = distanceToWest;
						falloffDistance = westFalloff;
					}
					if (distanceToEast < minimumDistance)
					{
						minimumDistance = distanceToEast;
						falloffDistance = eastFalloff;
					}

					// Use noise to modulate the falloff distance, creating irregular coastlines
					// Remap noise from 0-1 to 0.5-1.5 to vary the effective falloff distance
					var noiseModulation = 0.5f + edgeNoiseMap[x, y];
					var modulatedFalloffDistance = falloffDistance * noiseModulation;

					// Calculate edge factor (0 at edge, 1 at falloff distance or beyond)
					var edgeFactor = Mathf.Clamp(minimumDistance / modulatedFalloffDistance, 0f, 1f);

					// Combine with continent factor (take minimum to ensure both constraints apply)
					finalFactor = Mathf.Min(finalFactor, edgeFactor);
				}

				// Apply the combined factor to modulate the height map
				heightMap[x, y] *= finalFactor;
			}
		}
	}

	private BiomeType GetBiomeAt(float height, float temperature, Array<BiomeRange> biomeRanges, float baseEquatorValue)
	{
		// Apply temperature noise to create organic boundaries
		// Remap noise from 0-1 to -influence to +influence
		var noiseOffset = (temperature - 0.5f) * 2.0f * TemperatureNoiseInfluence;
		var equatorValue = Mathf.Clamp(baseEquatorValue + noiseOffset, 0f, 1f);
		var validBiomeRanges = biomeRanges.Where(b => height >= b.MinimumHeight && height <= b.MaximumHeight && equatorValue >= b.MinimumEquatorValue && equatorValue <= b.MaximumEquatorValue).ToList();

		var selectedBiomeRange = validBiomeRanges.Count > 0 ? validBiomeRanges[(int)(temperature * validBiomeRanges.Count) % validBiomeRanges.Count] : null;
		return selectedBiomeRange is not null ? selectedBiomeRange.Biome : BiomeType.Ocean;
	}

	/// <summary>
	/// Samples the biome map around a tile position and returns weighted probabilities for each biome.
	/// This creates smooth transitions between biomes by blending nearby biome types.
	/// </summary>
	/// <param name="tileX">X position in the expanded tile space</param>
	/// <param name="tileY">Y position in the expanded tile space</param>
	/// <returns>Dictionary mapping each biome type to its probability (0.0 to 1.0)</returns>
	private System.Collections.Generic.Dictionary<BiomeType, float> GetBiomeProbabilities(int tileX, int tileY)
	{
		var probabilities = new System.Collections.Generic.Dictionary<BiomeType, float>();

		// Convert tile position back to world biome map coordinates
		float worldX = (float)tileX / TileExpansionFactor;
		float worldY = (float)tileY / TileExpansionFactor;

		float totalWeight = 0f;

		// Sample surrounding biomes within the radius
		for (int dx = -BiomeSampleRadius; dx <= BiomeSampleRadius; dx++)
		{
			for (int dy = -BiomeSampleRadius; dy <= BiomeSampleRadius; dy++)
			{
				int sampleX = Mathf.Clamp((int)worldX + dx, 0, Width - 1);
				int sampleY = Mathf.Clamp((int)worldY + dy, 0, Height - 1);

				var biome = _worldMapBiomeTypes![sampleX, sampleY];

				// Calculate distance-based weight (inverse square falloff)
				float distance = Mathf.Sqrt(dx * dx + dy * dy);
				float weight = 1.0f / (1.0f + distance * distance);

				if (!probabilities.ContainsKey(biome))
				{
					probabilities[biome] = 0f;
				}

				probabilities[biome] += weight;
				totalWeight += weight;
			}
		}

		// Normalize probabilities to sum to 1.0
		if (totalWeight > 0f)
		{
			var keys = probabilities.Keys.ToList();
			foreach (var biome in keys)
			{
				probabilities[biome] /= totalWeight;
			}
		}

		return probabilities;
	}

	private System.Collections.Generic.Dictionary<EnvironmentalDecorationType, float[,]> GenerateDecorationNoiseMaps(int width, int height)
	{
		var noiseMaps = new System.Collections.Generic.Dictionary<EnvironmentalDecorationType, float[,]>();

		foreach (var config in DecorationConfigs!)
		{
			var noiseMap = NoiseService.GenerateNoiseMap(
				width,
				height,
				GlobalRandom.Seed + (int)config.DecorationType, // Different seed per decoration
				config.NoiseConfig
			);
			noiseMaps[config.DecorationType] = noiseMap;
		}

		return noiseMaps;
	}

	private bool ShouldPlaceDecoration(BiomeType biomeType, EnvironmentalDecorationPlacementConfig config, float noiseValue)
	{
		// Check if noise value is in the valid range
		if (noiseValue < config.MinimumValue || noiseValue > config.MaximumValue)
			return false;

		// Check if the current biome is valid for this decoration
		return config.ValidBiomes.Contains(biomeType);
	}

	private Array<BiomeRange> GetDefaultBiomeRanges()
	{
		return new Array<BiomeRange>
		{
			new BiomeRange(BiomeType.Ocean, 0.0f, 0.2f),
			new BiomeRange(BiomeType.Swamp, 0.2f, 0.3f, 0.1f, 0.2f),
			new BiomeRange(BiomeType.Beach, 0.2f, 0.3f),
			new BiomeRange(BiomeType.Plains, 0.3f, 0.5f, 0.4f, 0.7f),
			new BiomeRange(BiomeType.Desert, 0.3f, 0.5f, 0.2f, 0.4f),
			new BiomeRange(BiomeType.Tundra, 0.5f, 0.7f, 0.7f, 1.0f),
			new BiomeRange(BiomeType.ForestDeciduous, 0.4f, 0.7f, 0.0f, 0.5f),
			new BiomeRange(BiomeType.ForestConiferous, 0.6f, 0.9f, 0.5f, 1.0f),
			new BiomeRange(BiomeType.Jungle, 0.3f, 0.4f, 0.0f, 0.2f),
			new BiomeRange(BiomeType.Mountain, 0.8f, 1.0f, 0.0f, 1.0f),
		};
	}

	private Array<EnvironmentalDecorationPlacementConfig> GetDefaultDecorationConfigs()
	{
		return new Array<EnvironmentalDecorationPlacementConfig>
		{
			new (EnvironmentalDecorationType.Tree,
				new NoiseLayerConfig() { Frequency = 0.1f, Octaves = 3, Lacunarity = 2.0f, Persistence = 0.5f },
				minimumValue: 0.1f,
				maximumValue: 0.5f,
				validBiomes: new Array<BiomeType>() { BiomeType.ForestDeciduous, BiomeType.ForestConiferous }
			),
			new (EnvironmentalDecorationType.Rock,
				new NoiseLayerConfig() { Frequency = 0.2f, Octaves = 2, Lacunarity = 2.0f, Persistence = 0.5f },
				minimumValue: 0.2f,
				maximumValue: 0.6f,
				validBiomes: new Array<BiomeType>() { BiomeType.Mountain, BiomeType.Plains }
			),
			new (EnvironmentalDecorationType.Bush,
				new NoiseLayerConfig() { Frequency = 0.3f, Octaves = 2, Lacunarity = 2.0f, Persistence = 0.5f },
				minimumValue: 0.1f,
				maximumValue: 0.4f,
				validBiomes: new Array<BiomeType>() { BiomeType.Plains, BiomeType.ForestDeciduous }
			),
			new (EnvironmentalDecorationType.Flower,
				new NoiseLayerConfig() { Frequency = 0.4f, Octaves = 2, Lacunarity = 2.0f, Persistence = 0.5f },
				minimumValue: 0.1f,
				maximumValue: 0.3f,
				validBiomes: new Array<BiomeType>() { BiomeType.Plains, BiomeType.ForestDeciduous }
			),
			new (EnvironmentalDecorationType.Grass,
				new NoiseLayerConfig() { Frequency = 0.5f, Octaves = 2, Lacunarity = 2.0f, Persistence = 0.5f },
				minimumValue: 0.1f,
				maximumValue: 0.3f,
				validBiomes: new Array<BiomeType>() { BiomeType.Plains, BiomeType.ForestDeciduous }
			),
			new (EnvironmentalDecorationType.OreDeposit,
				new NoiseLayerConfig() { Frequency = 0.6f, Octaves = 2, Lacunarity = 2.0f, Persistence = 0.5f },
				minimumValue: 0.2f,
				maximumValue: 0.5f,
				validBiomes: new Array<BiomeType>() { BiomeType.Mountain, BiomeType.Plains }
			),
			new (EnvironmentalDecorationType.GasDeposit,
				new NoiseLayerConfig() { Frequency = 0.7f, Octaves = 2, Lacunarity = 2.0f, Persistence = 0.5f },
				minimumValue: 0.2f,
				maximumValue: 0.5f,
				validBiomes: new Array<BiomeType>() { BiomeType.Mountain, BiomeType.Plains }
			)
		};
	}

	private void InitializeResources()
	{
		_resources = new Dictionary<ResourceKind, Array<RawResource>>
		{
			{ ResourceKind.Ore, new Array<RawResource>()
				{
					new RawResource("Iron", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>()
					{
						{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 8.0f) },
						{ ResourcePropertyType.Toughness, new ResourceProperty(ResourcePropertyType.Toughness, 6.0f) },
						{ ResourcePropertyType.Resistance, new ResourceProperty(ResourcePropertyType.Resistance, 7.0f) },
						{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 5.0f) },
						{ ResourcePropertyType.Reactivity, new ResourceProperty(ResourcePropertyType.Reactivity, 4.0f) },
					}),
					new RawResource("Copper", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>()
					{
						{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 6.0f) },
						{ ResourcePropertyType.Toughness, new ResourceProperty(ResourcePropertyType.Toughness, 5.0f) },
						{ ResourcePropertyType.Resistance, new ResourceProperty(ResourcePropertyType.Resistance, 4.0f) },
						{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 9.0f) },
						{ ResourcePropertyType.Reactivity, new ResourceProperty(ResourcePropertyType.Reactivity, 7.0f) },
					})
				}
			},
			{ ResourceKind.Wood, new Array<RawResource>()
				{
					new RawResource("Pine", ResourceKind.Wood, new Dictionary<ResourcePropertyType, ResourceProperty>()
					{
						{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 4.0f) },
						{ ResourcePropertyType.Toughness, new ResourceProperty(ResourcePropertyType.Toughness, 5.0f) },
						{ ResourcePropertyType.Resistance, new ResourceProperty(ResourcePropertyType.Resistance, 3.0f) },
						{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 6.0f) },
						{ ResourcePropertyType.Reactivity, new ResourceProperty(ResourcePropertyType.Reactivity, 7.0f) },
					}),
					new RawResource("Oak", ResourceKind.Wood, new Dictionary<ResourcePropertyType, ResourceProperty>()
					{
						{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 6.0f) },
						{ ResourcePropertyType.Toughness, new ResourceProperty(ResourcePropertyType.Toughness, 7.0f) },
						{ ResourcePropertyType.Resistance, new ResourceProperty(ResourcePropertyType.Resistance, 5.0f) },
						{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 4.0f) },
						{ ResourcePropertyType.Reactivity, new ResourceProperty(ResourcePropertyType.Reactivity, 6.0f) },
					})
				}
			},
			{ ResourceKind.Gas, new Array<RawResource>()
				{
					new RawResource("Hydrogen", ResourceKind.Gas, new Dictionary<ResourcePropertyType, ResourceProperty>()
					{
						{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 2.0f) },
						{ ResourcePropertyType.Toughness, new ResourceProperty(ResourcePropertyType.Toughness, 1.0f) },
						{ ResourcePropertyType.Resistance, new ResourceProperty(ResourcePropertyType.Resistance, 2.0f) },
						{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 9.0f) },
						{ ResourcePropertyType.Reactivity, new ResourceProperty(ResourcePropertyType.Reactivity, 8.0f) },
					}),
					new RawResource("Oxygen", ResourceKind.Gas, new Dictionary<ResourcePropertyType, ResourceProperty>()
					{
						{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 3.0f) },
						{ ResourcePropertyType.Toughness, new ResourceProperty(ResourcePropertyType.Toughness, 2.0f) },
						{ ResourcePropertyType.Resistance, new ResourceProperty(ResourcePropertyType.Resistance, 3.0f) },
						{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 8.0f) },
						{ ResourcePropertyType.Reactivity, new ResourceProperty(ResourcePropertyType.Reactivity, 9.0f) },
					})
				}
			},
		};
	}

	private const int TEMPERATURE_NOISE_SEED_OFFSET = 1000;
	private const int OCEAN_CONTINENT_NOISE_SEED_OFFSET = 2000;
	private const int EDGE_NOISE_SEED_OFFSET = 3000;

	private BiomeType[,]? _worldMapBiomeTypes;
	private Dictionary<ResourceKind, Array<RawResource>>? _resources;
}
