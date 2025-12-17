using Godot;
using Godot.Collections;
using System;

namespace Wayblazer;

public partial class WorldGenerator : TileMapLayer
{
	public override void _Ready()
	{
		_resourceNodeScene = GD.Load<PackedScene>(Constants.Scenes.RESOURCE_NODE);

		int seed = (int)GD.Randi();
		_random = new Random(seed);

		GenerateWorldData();
		GD.Print($"World data generated with seed: {_random}");

		RenderWorld();
	}

	/// <summary>
	/// Generates world data by randomly filling the array with environment types.
	/// </summary>
	/// <param name="seed">Random seed for generation</param>
	public void GenerateWorldData()
	{
		for (var x = 0; x < WORLD_SIZE; x++)
		{
			for (var y = 0; y < WORLD_SIZE; y++)
			{
				// Randomly assign an environment type (0-1 for now, can be expanded)
				_worldData[x, y] = _random.Next(0, 2);
			}
		}

		// Generate all available resources
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
		for (int x = 0; x < WORLD_SIZE; x++)
		{
			for (int y = 0; y < WORLD_SIZE; y++)
			{
				int tileType = _worldData[x, y];

				// SetCell parameters: layer, coords, source_id, atlas_coords
				// Using source_id 0 and atlas coords based on tile type
				SetCell(new Vector2I(x, y), tileType, new Vector2I(1, 1));

				if (_resourceNodeScene is null || _resources is null)
					continue;

				// Randomly place resource nodes on certain tiles with 10% chance
				if (GD.Randf() < 0.1f)
				{
					var resourceNode = _resourceNodeScene.Instantiate<ResourceNode>();

					// Convert grid position to world position
					resourceNode.Position = new Vector2((x + 0.5f) * TileSet.TileSize.X, (y + 0.5f) * TileSet.TileSize.Y);

					if (tileType == 1)
					{
						resourceNode.ResourceData = _resources[ResourceKind.Ore][_random.Next(0, _resources[ResourceKind.Ore].Count)].Duplicate() as RawResource;
					}
					else
					{
						resourceNode.ResourceData = _resources[ResourceKind.Wood][_random.Next(0, _resources[ResourceKind.Wood].Count)].Duplicate() as RawResource;
					}

					AddChild(resourceNode);
				}
			}
		}

		GD.Print($"World rendered: {WORLD_SIZE}x{WORLD_SIZE} tiles");
	}

	private const int WORLD_SIZE = 48;
	private int[,] _worldData = new int[WORLD_SIZE, WORLD_SIZE];
	private TileMapLayer? _tileMapLayer;
	private PackedScene? _resourceNodeScene;
	private Random _random;
	private Dictionary<ResourceKind, Array<RawResource>>? _resources;
}
