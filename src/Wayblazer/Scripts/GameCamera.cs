using Godot;

namespace Wayblazer;

public partial class GameCamera : Camera2D
{
	[Export]
	public WorldGenerator? WorldMap;

	[Export]
	public CharacterBody2D? PlayerCharacter;

	public override void _Ready()
	{
		if (WorldMap is null)
		{
			GD.PrintErr("WorldMapBaseLayer is not assigned in the GameCamera.");
			return;
		}

		if (PlayerCharacter is null)
		{
			GD.PrintErr("PlayerCharacter is not assigned in the GameCamera.");
			return;
		}

		// Set camera limits based on the world map size
		var edgeBuffer = 48;
		LimitLeft = -edgeBuffer;
		LimitTop = -edgeBuffer;

		LimitRight = WorldMap.Width * WorldMap.TileSet.TileSize.X + edgeBuffer;
		LimitBottom = WorldMap.Height * WorldMap.TileSet.TileSize.Y + edgeBuffer;
	}

	public override void _Process(double delta)
	{
		// Follow the player character with smoothing
		if (PlayerCharacter is not null)
		{
			GlobalPosition = GlobalPosition.Lerp(PlayerCharacter.GlobalPosition, 0.1f);
		}
	}
}
