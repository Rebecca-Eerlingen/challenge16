using Godot;

public partial class BetterMovement : CharacterBody2D
{
	public const float Speed = 300.0f;

	private AnimatedSprite2D _animatedSprite;
	private Vector2 _facing = Vector2.Down;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		// Het game-venster moet zichtbaar blijven boven de editor om toetsen te kunnen ontvangen.
		if (OS.GetName() == "Windows")
		{
			DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.AlwaysOnTop, true, (int)DisplayServer.MainWindowId);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		// Lees WASD en pijltjes direct, onafhankelijk van de InputMap-acties.
		Vector2 direction = new Vector2(
			(Input.IsKeyPressed(Key.D) || Input.IsKeyPressed(Key.Right) ? 1f : 0f)
				- (Input.IsKeyPressed(Key.A) || Input.IsKeyPressed(Key.Left) ? 1f : 0f),
			(Input.IsKeyPressed(Key.S) || Input.IsKeyPressed(Key.Down) ? 1f : 0f)
				- (Input.IsKeyPressed(Key.W) || Input.IsKeyPressed(Key.Up) ? 1f : 0f)
		).Normalized();

		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;

			// Links en rechts delen dezelfde rij op de sheet; alleen het spiegelvlak verschilt.
			if (Mathf.Abs(direction.X) >= Mathf.Abs(direction.Y))
			{
				_facing = direction.X < 0 ? Vector2.Left : Vector2.Right;
				_animatedSprite.FlipH = direction.X < 0;
			}
			else
			{
				_facing = direction.Y < 0 ? Vector2.Up : Vector2.Down;
				_animatedSprite.FlipH = false;
			}

			_animatedSprite.Play(AnimationFor(_facing, walking: true));
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);

			_animatedSprite.Play(AnimationFor(_facing, walking: false));
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private static string AnimationFor(Vector2 facing, bool walking)
	{
		if (facing.X != 0)
		{
			return walking ? "WalkingRight" : "IdleRight";
		}

		return walking
			? (facing.Y < 0 ? "WalkingBack" : "WalkingFront")
			: (facing.Y < 0 ? "IdleBack" : "IdleFront");
	}
}
