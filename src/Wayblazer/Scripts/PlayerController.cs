using Godot;

public partial class PlayerController : CharacterBody2D
{
	[Export]
	public float Speed = 300.0f;

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		var direction = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		if (direction != Vector2.Zero)
		{
			velocity = direction * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		ZIndex = (int)GlobalPosition.Y;
	}
}
