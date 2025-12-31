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

		// For now, hardcode the resources but in the future, these will be loaded from a configuration file or procedurally generated.
		var environmentalDecorations = Enum.GetValues(typeof(EnvironmentalDecorationType));
		Array<EnvironmentalDecorationPlacementConfig> environmentalDecorationPlacementConfigs =
		[
			new (EnvironmentalDecorationType.Tree,
				new NoiseLayerConfig() { Frequency = 0.1f, Octaves = 3, Lacunarity = 2.0f, Persistence = 0.5f },
				minimumValue: 0.1f,
				maximumValue: 0.5f,
				validBiomes: new Array<BiomeType>() { BiomeType.ForestDeciduous, BiomeType.ForestConiferous }
				),
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
				// 0 - Plains
				Id = "plains",
				Weight = 10,
				NeighborIndices =
				[
					[ 0, 1 ], // up
					[ 0, 1 ], // right
					[ 0, 1 ], // down
					[ 0, 1 ]  // left
				]
			},
			new()
			{
				// 1 - Water
				Id = "water",
				Weight = 5,
				NeighborIndices =
				[
					[ 1 ], // up
					[ 1 ], // right
					[ 1 ], // down
					[ 1 ], // left
				]
			}
		];

		// Map proto tile indices to their corresponding tile set coordinates using hard coded values for now
		System.Collections.Generic.Dictionary<int, (int X, int Y)> protoTileIndexToTileSetCoords = new()
		{
			{ 0, (0, 0) },
			{ 1, (1, 0) },
		};

		var configuration = new Configuration(protoTiles, AdjacencyAlgorithmKind.ADJACENCY_2D);
		var output = new Output(configuration, width: WORLD_SIZE, height: WORLD_SIZE, depth: 1, getInitialValidProtoTilesForPosition: (x, y, z) =>
			{
				// for now, return all proto tiles as valid for any position
				return protoTiles;
			});
		var algorithm = new Algorithm(configuration, seed: GlobalRandom.Seed);
		algorithm.Run(output);

		// Iterate through the WFC output and set the corresponding tiles in the world
		var protoTileIndices = output.ToSerializable().Tiles;
		// Spawn resources and environmental decorations in each valid tile that matches the environmental decoration placement config

		/*
		for (int x = 0; x < WORLD_SIZE; x++)
		{
			for (int y = 0; y < WORLD_SIZE; y++)
			{
				var protoTileIndex = protoTileIndices[x + y * WORLD_SIZE];
				var atlasCoords = protoTileIndexToTileSetCoords[protoTileIndex];
				_tileMapLayer?.SetCell(new Vector2I(x, y), atlasCoords: new Vector2I(atlasCoords.X, atlasCoords.Y));
			}
		}

		// Spawn resources and environmental decorations in each valid tile that matches the environmental decoration placement config

		/*
		for (int x = 0; x < WORLD_SIZE; x++)
		{
			for (int y = 0; y < WORLD_SIZE; y++)
			{
				var biomeType = _worldMapBiomeTypes[x, y];

				// SetCell parameters: layer, coords, source_id, atlas_coords
				// Using source_id 0 and atlas coords based on tile type
				SetCell(new Vector2I(x, y), biomeType, new Vector2I(1, 1));

				// Randomly place resource nodes on certain tiles with 10% chance
				if (GD.Randf() < 0.1f)
				{
					ResourceNode resourceNode;
					if (biomeType == 1)
					{
						resourceNode = _goldOreScene!.Instantiate<ResourceNode>();
						resourceNode.ResourceData = _resources![ResourceKind.Ore][GlobalRandom.Next(0, _resources[ResourceKind.Ore].Count)].Duplicate() as RawResource;
					}
					else
					{
						resourceNode = _pineTreeScene!.Instantiate<ResourceNode>();
						resourceNode.ResourceData = _resources![ResourceKind.Wood][GlobalRandom.Next(0, _resources[ResourceKind.Wood].Count)].Duplicate() as RawResource;
					}

					// Convert grid position to world position
					resourceNode.Position = new Vector2((x + 0.5f) * TileSet.TileSize.X, (y + 0.5f) * TileSet.TileSize.Y);
					resourceNode.ZIndex = (int) Math.Round(resourceNode.Position.Y);

					AddChild(resourceNode);
				}
			}
		}
		*/

		GD.Print($"World rendered: {WORLD_SIZE}x{WORLD_SIZE} tiles");
	}

	private BiomeType GetBiomeAt(float height, Array<BiomeRange> biomeRanges, float equatorValue)
	{
		var validBiomeRanges = biomeRanges.FirstOrDefault(b => height >= b.MinimumHeight && height <= b.MaximumHeight && equatorValue >= b.MinimumEquatorValue && equatorValue <= b.MaximumEquatorValue);
		return validBiomeRanges is not null ? validBiomeRanges.Biome : BiomeType.Default;
	}

	private const int WORLD_SIZE = 48;
	private BiomeType[,] _worldMapBiomeTypes = new BiomeType[WORLD_SIZE, WORLD_SIZE];
	private Array<EnvironmentalDecorationPlacementConfig>? _environmentalDecorationPlacementConfigs;
	private TileMapLayer? _tileMapLayer;
	private Dictionary<ResourceKind, Array<RawResource>>? _resources;
	private PackedScene? _pineTreeScene;
	private PackedScene? _goldOreScene;
}
