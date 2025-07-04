using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	public int JumpImpulse { get; set; } = 20;

	[Export]
	public int BounceImpulse { get; set; } = 16;

	[Export]
	public int Speed { get; set; } = 14;

	[Export]
	public int FallAcceleration { get; set; } = 75;

	private Vector3 _targetVelocity = Vector3.Zero;

	public override void _PhysicsProcess(double delta)
	{
		var direction = Vector3.Zero;

		if (Input.IsActionPressed("move_right"))
		{
			direction.X += 1.0f;
		}

		if (Input.IsActionPressed("move_left"))
		{
			direction.X -= 1.0f;
		}

		if (Input.IsActionPressed("move_forward"))
		{
			direction.Z -= 1.0f;
		}

		if (Input.IsActionPressed("move_back"))
		{
			direction.Z += 1.0f;
		}

		if (IsOnFloor() && Input.IsActionJustPressed("jump"))
		{
			_targetVelocity.Y = JumpImpulse;
		}

		for(int index = 0; index < GetSlideCollisionCount(); index++)
		{
			KinematicCollision3D collision = GetSlideCollision(index);

			if(collision.GetCollider() is Mob mob)
			{
				if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
				{
					mob.Squash();
					_targetVelocity.Y = BounceImpulse;
					break;
				}
			}
		}

		if(direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			GetNode<Node3D>("Pivot").Basis = Basis.LookingAt(direction);
		}

		_targetVelocity.X = direction.X * Speed;
		_targetVelocity.Z = direction.Z * Speed;

		if (!IsOnFloor())
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}

		Velocity = _targetVelocity;
		MoveAndSlide();
	}

}
