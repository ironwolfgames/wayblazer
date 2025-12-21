using Godot;

namespace Wayblazer;

[GlobalClass]
public partial class ResourceNode : Node2D
{
	[Export]
	public RawResource? ResourceData { get; set; }

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AddToGroup(Constants.NodeGroups.RESOURCES);

		if (ResourceData is not null && _animatedSprite is not null)
		{
			// set a timer to repeatedly run for a random number of seconds and then play the animated sprite
			_timer.WaitTime = GlobalRandom.NextFloat(0.5f, 7.0f);
			_timer.OneShot = true;
			_timer.Timeout += () =>
			{
				// _animatedSprite.Frame = GlobalRandom.Next(0, _animatedSprite.SpriteFrames.GetFrameCount(_animatedSprite.Animation));
				_animatedSprite.Play();

				_timer.WaitTime = GlobalRandom.NextFloat(0.5f, 7.0f);
				_timer.Start();
			};
			AddChild(_timer);
			_timer.Start();

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
	private Timer _timer = new Timer() { OneShot = true };
}
