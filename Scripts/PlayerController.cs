using Godot;
using System;

// ReSharper disable once CheckNamespace
public partial class PlayerController : CharacterBody2D
{
    private float _speed = 100.0f;
    private float _jumpSpeed = -400.0f;
    private float _gravity;
    private bool _facingRight = true;
    private bool _canDoubleJump = true;

    private AnimatedSprite2D _animatedSprite;
    private CollisionShape2D _collisionShape;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
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

        HandleAnimation(velocity);

        Velocity = velocity; // applies updated velocity to the character
        MoveAndSlide();

        // Reset double jump if on the floor
        if (IsOnFloor())
        {
            _canDoubleJump = true;
        }
    }

    private void HandleAnimation(Vector2 velocity)
    {
        if (!IsOnFloor())
        {
            _animatedSprite.Play("Jump");
        }
        else if (velocity.X == 0)
        {
            _animatedSprite.Play("Idle");
        }
        else
        {
            _animatedSprite.Play("Run");
        }
    }

    private void ApplyGravity(ref Vector2 velocity, double delta)
    {
        velocity.Y += _gravity * (float)delta;
    }

    private void HandleMovement(ref Vector2 velocity)
    {
        bool wasFacingRight = _facingRight;

        if (Input.IsActionPressed("move_right"))
        {
            velocity.X = _speed;
            _facingRight = true;
        }
        else if (Input.IsActionPressed("move_left"))
        {
            velocity.X = -_speed;
            _facingRight = false;
        }
        else
        {
            velocity.X = 0;
        }

        // If facing direction changed, update facing direction
        if (_facingRight != wasFacingRight)
        {
            _animatedSprite.FlipH = !_facingRight;
        }
    }

    private void HandleJump(ref Vector2 velocity)
    {
        if (IsOnFloor())
        {
            if (Input.IsActionJustPressed("move_jump"))
            {
                velocity.Y = _jumpSpeed;
            }
        }
        else if (_canDoubleJump && Input.IsActionJustPressed("move_jump"))
        {
            velocity.Y = _jumpSpeed;
            _canDoubleJump = false;
        }
    }
}