using System;
using Godot;
using Wayblazer;

public partial class PlayerController : CharacterBody2D
{
	[Export]
	public float Speed = 300.0f;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (_animatedSprite is null)
			throw new InvalidOperationException($"AnimatedSprite2D node not found for {Name} ({nameof(PlayerController)})");

		HandleAnimations();
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		var direction = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		if (direction != Vector2.Zero)
		{
			var speed = Speed;
			if (Input.IsActionPressed("player_run"))
			{
				speed = Speed * 2;
			}
			velocity = direction * speed;

			// if the player is moving left, flip the sprite
			if (direction.X < 0)
			{
				_animatedSprite!.FlipH = true;
			}
			else if (direction.X > 0)
			{
				_animatedSprite!.FlipH = false;
			}

			if (_state != PlayerState.Harvesting)
			{
				_state = Input.IsActionPressed("player_run") ? PlayerState.Running : PlayerState.Walking;
			}
		}
		else
		{
			if (_state != PlayerState.Harvesting)
			{
				_state = PlayerState.Idle;
			}

			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		ZIndex = (int)GlobalPosition.Y;

		CheckForResourceNode();
		HandleHarvestInput();

		HandleAnimations();
	}

	private void HandleAnimations()
	{
		if (_animatedSprite is null)
			return;

		var animationToPlay = _state switch
		{
			PlayerState.Idle => "idle",
			PlayerState.Walking => "walk",
			PlayerState.Running => "run",
			PlayerState.Harvesting => "harvest",
			_ => null
		};

		if (animationToPlay is not null && (_animatedSprite.Animation != animationToPlay || !_animatedSprite.IsPlaying()))
		{
			if (_state == PlayerState.Harvesting && _nearbyResourceNode is not null)
			{
				if (_nearbyResourceNode.ResourceData?.ResourceKind == ResourceKind.Wood)
				{
					_animatedSprite?.Play("harvest_wood");
				}
				else if (_nearbyResourceNode.ResourceData?.ResourceKind == ResourceKind.Ore)
				{
					_animatedSprite?.Play("harvest_ore");
				}
			}
			else
			{
				_animatedSprite?.Play(animationToPlay);
			}
		}
	}

	enum PlayerState
	{
		Idle,
		Walking,
		Running,
		Harvesting
	}

	private void CheckForResourceNode()
	{
		_nearbyResourceNode = null;

		foreach (var node in GetTree().GetNodesInGroup(Constants.NodeGroups.RESOURCES))
		{
			if (node is ResourceNode resourceNode)
			{
				if (GlobalPosition.DistanceTo(resourceNode.GlobalPosition) < Constants.INTERACTION_RANGE)
				{
					_nearbyResourceNode = resourceNode;
					break;
				}
			}
		}
	}

	private void HandleHarvestInput()
	{
		if (_nearbyResourceNode is null)
			return;

		if (Input.IsActionJustPressed("player_interact"))
		{
			_state = PlayerState.Harvesting;

			// kick off a timer for harvesting duration after which the resource node will be harvested
			var harvestTimer = new Timer();
			harvestTimer.WaitTime = 1.0f;
			harvestTimer.OneShot = true;
			harvestTimer.Timeout += () =>
			{
				_nearbyResourceNode.Harvest();
				GD.Print("Harvested resource node: " + _nearbyResourceNode.ResourceData!.Name);

				_state = PlayerState.Idle;
			};
			AddChild(harvestTimer);
			harvestTimer.Start();
		}
	}

	private ResourceNode? _nearbyResourceNode;
	private AnimatedSprite2D? _animatedSprite;
	private PlayerState _state;
}
