using Godot;

public partial class WorldGenerator : TileMapLayer
{
	public override void _Ready()
	{
		_tileMapLayer = this;
		GD.Print($"WorldGenerator ready: {_tileMapLayer}, {_tileMapLayer?.Name}");
	}

	private TileMapLayer? _tileMapLayer;
}
