using System;
using System.Linq;
using Godot;
using Godot.Collections;
using WFC.Core;

namespace Wayblazer;

public partial class WorldGenerator : TileMapLayer
{
	public override void _Ready()
	{
		_pineTreeScene = GD.Load<PackedScene>(Constants.Scenes.PINE_TREE);
		_goldOreScene = GD.Load<PackedScene>(Constants.Scenes.GOLD_ORE);

		GlobalRandom.InitializeWithSeed((int)GD.Randi());

		GenerateWorldData();
		GD.Print($"World data generated with seed: {GlobalRandom.Seed}");

		RenderWorld();
	}

	public void GenerateWorldData()
	{
		var worldBiomeMapNoiseConfig = new NoiseLayerConfig();
		var worldHeightMap = NoiseService.GenerateNoiseMap(WORLD_SIZE, WORLD_SIZE, GlobalRandom.Seed, worldBiomeMapNoiseConfig);

		// For now, hardcode the biome ranges but in the future, these will be loaded from a configuration file or generated procedurally.
		Array<BiomeRange> biomeRanges =
		[
			new BiomeRange(BiomeType.Ocean, 0.0f, 0.2f),
			// new BiomeRange(BiomeType.River, 0.2f, 0.4f),
			// new BiomeRange(BiomeType.Lake, 0.3f, 0.6f),
			new BiomeRange(BiomeType.Swamp, 0.2f, 0.3f, 0.1f, 0.2f),
			new BiomeRange(BiomeType.Beach, 0.2f, 0.3f),
			new BiomeRange(BiomeType.Plains, 0.3f, 0.5f, 0.4f, 0.7f),
			new BiomeRange(BiomeType.Desert, 0.3f, 0.5f, 0.2f, 0.4f),
			new BiomeRange(BiomeType.Tundra, 0.5f, 0.7f, 0.7f, 1.0f),
			new BiomeRange(BiomeType.ForestDeciduous, 0.4f, 0.7f, 0.0f, 0.5f),
			new BiomeRange(BiomeType.ForestConiferous, 0.6f, 0.9f, 0.5f, 1.0f),
			new BiomeRange(BiomeType.Jungle, 0.3f, 0.4f, 0.0f, 0.2f),
			new BiomeRange(BiomeType.Mountain, 0.8f, 1.0f, 0.0f, 1.0f),
		];

		// Assign biomes to each tile based on the height map
		for (var x = 0; x < WORLD_SIZE; x++)
		{
			for (var y = 0; y < WORLD_SIZE; y++)
			{
				// calculate equator value in the range of 0.0 to 1.0 for the current y where 0.0 is the equator and 1.0 is either pole.
				var equatorValue = (float)Math.Abs(y - (WORLD_SIZE / 2)) / (WORLD_SIZE / 2);
				_worldMapBiomeTypes[x, y] = GetBiomeAt(worldHeightMap[x, y], biomeRanges, equatorValue);
			}
		}

		// For now, hardcode the environmental decoration placement configs but in the future, these
		// will be loaded from a configuration file or generated procedurally.
		_environmentalDecorationPlacementConfigs =
		[
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
		];

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

	public void RenderWorld()
	{
		// Use WFC (Wave Function Collapse) to generate the world's base tiles

		// Hard coded proto tiles for now. In the future, load these from the source WFC image and configuration file
		System.Collections.Generic.List<ProtoTile> protoTiles =
		[
			new()
			{
				// 0 - Ocean
				Id = "ocean",
				Weight = 20,
				NeighborIndices =
				[
					[ 0, 1, 9 ], // up
					[ 0, 1, 9 ], // right
					[ 0, 1, 9 ], // down
					[ 0, 1, 9 ]  // left
				]
			},
			new()
			{
				// 1 - Beach
				Id = "beach",
				Weight = 5,
				NeighborIndices =
				[
					[ 0, 1, 2, 3, 4, 9 ], // up
					[ 0, 1, 2, 3, 4, 9 ], // right
					[ 0, 1, 2, 3, 4, 9 ], // down
					[ 0, 1, 2, 3, 4, 9 ]  // left
				]
			},
			new()
			{
				// 2 - Plains
				Id = "plains",
				Weight = 15,
				NeighborIndices =
				[
					[ 1, 2, 3, 5, 8 ], // up
					[ 1, 2, 3, 5, 8 ], // right
					[ 1, 2, 3, 5, 8 ], // down
					[ 1, 2, 3, 5, 8 ]  // left
				]
			},
			new()
			{
				// 3 - Desert
				Id = "desert",
				Weight = 8,
				NeighborIndices =
				[
					[ 1, 2, 3, 8 ], // up
					[ 1, 2, 3, 8 ], // right
					[ 1, 2, 3, 8 ], // down
					[ 1, 2, 3, 8 ]  // left
				]
			},
			new()
			{
				// 4 - Jungle
				Id = "jungle",
				Weight = 8,
				NeighborIndices =
				[
					[ 1, 4, 5, 9 ], // up
					[ 1, 4, 5, 9 ], // right
					[ 1, 4, 5, 9 ], // down
					[ 1, 4, 5, 9 ]  // left
				]
			},
			new()
			{
				// 5 - ForestDeciduous
				Id = "forest_deciduous",
				Weight = 12,
				NeighborIndices =
				[
					[ 2, 4, 5, 6 ], // up
					[ 2, 4, 5, 6 ], // right
					[ 2, 4, 5, 6 ], // down
					[ 2, 4, 5, 6 ]  // left
				]
			},
			new()
			{
				// 6 - ForestConiferous
				Id = "forest_coniferous",
				Weight = 10,
				NeighborIndices =
				[
					[ 5, 6, 7, 8 ], // up
					[ 5, 6, 7, 8 ], // right
					[ 5, 6, 7, 8 ], // down
					[ 5, 6, 7, 8 ]  // left
				]
			},
			new()
			{
				// 7 - Tundra
				Id = "tundra",
				Weight = 6,
				NeighborIndices =
				[
					[ 6, 7, 8 ], // up
					[ 6, 7, 8 ], // right
					[ 6, 7, 8 ], // down
					[ 6, 7, 8 ]  // left
				]
			},
			new()
			{
				// 8 - Mountain
				Id = "mountain",
				Weight = 8,
				NeighborIndices =
				[
					[ 2, 3, 6, 7, 8 ], // up
					[ 2, 3, 6, 7, 8 ], // right
					[ 2, 3, 6, 7, 8 ], // down
					[ 2, 3, 6, 7, 8 ]  // left
				]
			},
			new()
			{
				// 9 - Swamp
				Id = "swamp",
				Weight = 5,
				NeighborIndices =
				[
					[ 0, 1, 4, 9 ], // up
					[ 0, 1, 4, 9 ], // right
					[ 0, 1, 4, 9 ], // down
					[ 0, 1, 4, 9 ]  // left
				]
			}
		];

		// Map proto tile indices to their corresponding tile set coordinates using hard coded values for now
		System.Collections.Generic.Dictionary<int, (int X, int Y)> protoTileIndexToTileSetCoords = new()
		{
			{ 0, (0, 0) },
			{ 1, (1, 0) },
			{ 2, (2, 0) },
			{ 3, (3, 0) },
			{ 4, (4, 0) },
			{ 5, (5, 0) },
			{ 6, (6, 0) },
			{ 7, (7, 0) },
			{ 8, (8, 0) },
			{ 9, (9, 0) },
		};

		var configuration = new Configuration(protoTiles, AdjacencyAlgorithmKind.ADJACENCY_2D);
		var output = new Output(configuration, width: WORLD_SIZE, height: WORLD_SIZE, depth: 1, getInitialValidProtoTilesForPosition: (x, y, z) =>
			{
				// Get the biome type at this position
				var biomeType = _worldMapBiomeTypes[x, y];

				// only allow the proto tiles that matches this biome
				var protoTileIndex = (int)biomeType;
				if (protoTileIndex >= 0 && protoTileIndex < protoTiles.Count)
				{
					return new System.Collections.Generic.List<ProtoTile> { protoTiles[protoTileIndex] };
				}

				// fallback to returning all proto tiles as valid
				return protoTiles;
			});
		var algorithm = new Algorithm(configuration, seed: GlobalRandom.Seed);
		algorithm.Run(output);

		// Get the proto tile indices from the WFC output
		var protoTileIndices = output.ToSerializable().Tiles;

		// Generate noise maps for all decoration types
		var decorationNoiseMaps = GenerateDecorationNoiseMaps();

		// Create a lookup for scene paths (you'll need to add these to Constants.Scenes)
		var decorationScenes = new Dictionary<EnvironmentalDecorationType, PackedScene>
		{
			{ EnvironmentalDecorationType.Tree, _pineTreeScene! },
			{ EnvironmentalDecorationType.OreDeposit, _goldOreScene! },
			// Add more as you create the scenes
		};

		// Iterate through the world, set the tiles in the world according to the WFC output and place environmental decorations
		for (int x = 0; x < WORLD_SIZE; x++)
		{
			for (int y = 0; y < WORLD_SIZE; y++)
			{
				var protoTileIndex = protoTileIndices[x + y * WORLD_SIZE];
				var atlasCoords = protoTileIndexToTileSetCoords[protoTileIndex];
				_tileMapLayer?.SetCell(new Vector2I(x, y), atlasCoords: new Vector2I(atlasCoords.X, atlasCoords.Y));

				foreach (var config in _environmentalDecorationPlacementConfigs!)
				{
					if (!decorationNoiseMaps.ContainsKey(config.DecorationType) || !decorationScenes.ContainsKey(config.DecorationType))
						continue;

					var noiseValue = decorationNoiseMaps[config.DecorationType][x, y];
					if (ShouldPlaceDecoration(x, y, config, noiseValue))
					{
						var scene = decorationScenes[config.DecorationType];
						var instance = scene.Instantiate<ResourceNode>();

						// Set resource data based on decoration type
						if (config.DecorationType == EnvironmentalDecorationType.OreDeposit)
						{
							instance.ResourceData = _resources![ResourceKind.Ore][GlobalRandom.Next(0, _resources[ResourceKind.Ore].Count)].Duplicate() as RawResource;
						}
						else if (config.DecorationType == EnvironmentalDecorationType.Tree)
						{
							instance.ResourceData = _resources![ResourceKind.Wood][GlobalRandom.Next(0, _resources[ResourceKind.Wood].Count)].Duplicate() as RawResource;
						}

						// Convert grid position to world position
						instance.Position = new Vector2((x + 0.5f) * TileSet.TileSize.X, (y + 0.5f) * TileSet.TileSize.Y);
						instance.ZIndex = (int)Math.Round(instance.Position.Y);

						AddChild(instance);

						// Only place one decoration per tile
						// TODO: ensure we order decorations by priority
						break;
					}
				}
			}
		}

		GD.Print($"World rendered: {WORLD_SIZE}x{WORLD_SIZE} tiles");
	}

	private BiomeType GetBiomeAt(float height, Array<BiomeRange> biomeRanges, float equatorValue)
	{
		var validBiomeRanges = biomeRanges.FirstOrDefault(b => height >= b.MinimumHeight && height <= b.MaximumHeight && equatorValue >= b.MinimumEquatorValue && equatorValue <= b.MaximumEquatorValue);
		return validBiomeRanges is not null ? validBiomeRanges.Biome : BiomeType.Default;
	}

	private System.Collections.Generic.Dictionary<EnvironmentalDecorationType, float[,]> GenerateDecorationNoiseMaps()
	{
		var noiseMaps = new System.Collections.Generic.Dictionary<EnvironmentalDecorationType, float[,]>();

		foreach (var config in _environmentalDecorationPlacementConfigs!)
		{
			var noiseMap = NoiseService.GenerateNoiseMap(
				WORLD_SIZE,
				WORLD_SIZE,
				GlobalRandom.Seed + (int)config.DecorationType, // Different seed per decoration
				config.NoiseConfig
			);
			noiseMaps[config.DecorationType] = noiseMap;
		}

		return noiseMaps;
	}

	private bool ShouldPlaceDecoration(int x, int y, EnvironmentalDecorationPlacementConfig config, float noiseValue)
	{
		var biomeType = _worldMapBiomeTypes[x, y];

		// Check if noise value is in the valid range
		if (noiseValue < config.MinimumValue || noiseValue > config.MaximumValue)
			return false;

		// Check if the current biome is valid for this decoration
		return config.ValidBiomes.Contains(biomeType);
	}

	private const int WORLD_SIZE = 48;
	private BiomeType[,] _worldMapBiomeTypes = new BiomeType[WORLD_SIZE, WORLD_SIZE];
	private Array<EnvironmentalDecorationPlacementConfig>? _environmentalDecorationPlacementConfigs;
	private TileMapLayer? _tileMapLayer;
	private Dictionary<ResourceKind, Array<RawResource>>? _resources;
	private PackedScene? _pineTreeScene;
	private PackedScene? _goldOreScene;
}
