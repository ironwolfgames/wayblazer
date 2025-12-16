using Godot;
using System;

public partial class WorldGenerator : TileMapLayer
{
	public override void _Ready()
	{
		_tileMapLayer = this;
		GD.Print($"WorldGenerator ready: {_tileMapLayer}, {_tileMapLayer?.Name}");

		GenerateWorldData(42);
		RenderWorld();
	}

	/// <summary>
	/// Generates world data by randomly filling the array with environment types.
	/// </summary>
	/// <param name="seed">Random seed for generation</param>
	public void GenerateWorldData(int seed)
	{
		var random = new Random(seed);
		for (var x = 0; x < WORLD_SIZE; x++)
		{
			for (var y = 0; y < WORLD_SIZE; y++)
			{
				// Randomly assign an environment type (0-1 for now, can be expanded)
				_worldData[x, y] = random.Next(0, 2);
			}
		}

		GD.Print($"World data generated with seed: {seed}");
	}

	public void RenderWorld()
	{
		if (_tileMapLayer == null)
		{
			GD.PrintErr("TileMapLayer is not initialized");
			return;
		}

		for (int x = 0; x < WORLD_SIZE; x++)
		{
			for (int y = 0; y < WORLD_SIZE; y++)
			{
				int tileType = _worldData[x, y];

				// SetCell parameters: layer, coords, source_id, atlas_coords
				// Using source_id 0 and atlas coords based on tile type
				_tileMapLayer.SetCell(new Vector2I(x, y), tileType, new Vector2I(1, 1));
			}
		}

		GD.Print($"World rendered: {WORLD_SIZE}x{WORLD_SIZE} tiles");
	}

	private const int WORLD_SIZE = 48;
	private int[,] _worldData = new int[WORLD_SIZE, WORLD_SIZE];
	private TileMapLayer? _tileMapLayer;
}
