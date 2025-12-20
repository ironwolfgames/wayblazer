using Godot;

namespace Wayblazer;

public partial class ResourceNode : Area2D
{
	[Export]
	public RawResource? ResourceData { get; set; }

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AddToGroup(Constants.NodeGroups.RESOURCES);

		if (ResourceData is not null && _animatedSprite is not null)
		{
			_animatedSprite.SpriteFrames = ResourceData.ResourceKind switch
			{
				ResourceKind.Ore => GD.Load<SpriteFrames>(Constants.Sprites.RESOURCE_ORE),
				ResourceKind.Wood => GD.Load<SpriteFrames>(Constants.Sprites.RESOURCE_WOOD),
			};

			_animatedSprite.Frame = GlobalRandom.Next(0, _animatedSprite.SpriteFrames.GetFrameCount(_animatedSprite.Animation));
			_animatedSprite.Play();

			GD.Print($"Resource node created: {ResourceData.Name}");
		}
	}

	public void Harvest()
	{
		GD.Print($"Harvesting {ResourceData?.Name}");

		// Play sound effect, spawn particles, etc.
		QueueFree(); // Remove this node from the scene
	}

	private AnimatedSprite2D? _animatedSprite;
}
