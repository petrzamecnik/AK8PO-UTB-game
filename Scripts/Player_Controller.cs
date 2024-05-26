using Godot;
using System;

public partial class Player_Controller : CharacterBody2D
{
	private float _speed = 100.0f;
	private float _jumpSpeed = -400.0f;

	public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Velocity;
		velocity.Y += 9.8f * (float)delta;
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;
		
		velocity.Y += 9.8f * (float)delta;

	}
}
