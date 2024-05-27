using Godot;
using System;

public partial class Player_Controller : CharacterBody2D
{
    private float _speed = 100.0f;
    private float _jumpSpeed = -400.0f;
    private float _gravity;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;
        
        ApplyGravity(ref velocity, delta);
        HandleMovement(ref velocity);
        HandleJump(ref velocity);

        Velocity = velocity;  // applies updated velocity to the character
        MoveAndSlide();
    }

    private void ApplyGravity(ref Vector2 velocity, double delta)
    {
        velocity.Y += _gravity * (float)delta;
    }

    private void HandleMovement(ref Vector2 velocity)
    {
        if (Input.IsActionPressed("move_right"))
        {
            velocity.X = _speed;
        }
        else if (Input.IsActionPressed("move_left"))
        {
            velocity.X = -_speed;
        }
        else
        {
            velocity.X = 0;
        }
    }

    private void HandleJump(ref Vector2 velocity)
    {
        if (IsOnFloor() && Input.IsActionJustPressed("move_jump"))
        {
            velocity.Y = _jumpSpeed;
        }
    }
}