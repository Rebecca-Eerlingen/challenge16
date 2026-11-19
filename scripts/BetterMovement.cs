using Godot;

public partial class BetterMovement : CharacterBody2D
{
	public const float Speed = 300.0f;

	private AnimatedSprite2D _animatedSprite;

	public override void _Ready()
	{
		// haalt het animatedsprite node op wanneer de scene geladen wordt
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

	
		Vector2 direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;

			
			_animatedSprite.Play("WalkingRight");

			
			if (direction.X != 0)
			{
				_animatedSprite.FlipH = direction.X < 0;
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);

			// Als je stopt met bewegen, stop dan de animatie
			_animatedSprite.Stop();
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
