using Godot;
using System;
using System.Collections.Generic;


public partial class GameController : Node
{
    private const int TileSize = 16;
    private const int WallHeightInPixels = 960;
    private const int BackgroundHeightInPixels = 1600;

    private CharacterBody2D _player;

    // scenes
    private PackedScene _leftWallScene;
    private PackedScene _rightWallScene;
    private PackedScene _backGroundScene;

    // instances
    private Node2D _leftWallInstance0;
    private Node2D _leftWallInstance1;
    private Node2D _leftWallInstance2;
    private Node2D _rightWallInstance0;
    private Node2D _rightWallInstance1;
    private Node2D _rightWallInstance2;
    private Node2D _backgroundInstance0;
    private Node2D _backgroundInstance1;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _player = GetNode<CharacterBody2D>("Player");

        _leftWallScene = (PackedScene)ResourceLoader.Load("res://prefabs/left_wall.tscn");
        _rightWallScene = (PackedScene)ResourceLoader.Load("res://prefabs/right_wall.tscn");
        _backGroundScene = (PackedScene)ResourceLoader.Load("res://prefabs/background_tile_map.tscn");

        InitializeWalls();
        InitializeBackground();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var playerGlobalPosition = _player.GlobalPosition;

        if (Input.IsActionJustPressed("debug_key"))
        {
            GD.Print("Player Global Position:", playerGlobalPosition);
        }

        UpdateWalls();
        UpdateBackground();
    }

    private void InitializeBackground()
    {
        _backgroundInstance0 = _backGroundScene.Instantiate() as Node2D;
        _backgroundInstance1 = _backGroundScene.Instantiate() as Node2D;

        _backgroundInstance0!.GlobalPosition = new Vector2I(0, 0);
        _backgroundInstance1!.GlobalPosition = new Vector2I(0, 0 - BackgroundHeightInPixels);

        AddChild(_backgroundInstance0);
        AddChild(_backgroundInstance1);
    }

    private void UpdateBackground()
    {
        UpdateBackgroundPosition(_backgroundInstance0, _backgroundInstance1);
        UpdateBackgroundPosition(_backgroundInstance1, _backgroundInstance0);
    }

    private void UpdateBackgroundPosition(Node2D background1, Node2D background2)
    {
        const int offset = 200;

        if (_player.GlobalPosition.Y < background2.GlobalPosition.Y - offset)
        {
            background1.GlobalPosition = new Vector2(background1.GlobalPosition.X,
                background2.GlobalPosition.Y - BackgroundHeightInPixels);
        }
        else if (_player.GlobalPosition.Y > background2.GlobalPosition.Y + offset)
        {
            background1.GlobalPosition = new Vector2(background1.GlobalPosition.X,
                background2.GlobalPosition.Y + BackgroundHeightInPixels);
        }
    }

    private void InitializeWalls()
    {
        _leftWallInstance0 = _leftWallScene.Instantiate() as Node2D;
        _leftWallInstance1 = _leftWallScene.Instantiate() as Node2D;
        _leftWallInstance2 = _leftWallScene.Instantiate() as Node2D;
        _rightWallInstance0 = _rightWallScene.Instantiate() as Node2D;
        _rightWallInstance1 = _rightWallScene.Instantiate() as Node2D;
        _rightWallInstance2 = _rightWallScene.Instantiate() as Node2D;

        _leftWallInstance0!.GlobalPosition = new Vector2I(-8 * TileSize, 0);
        _leftWallInstance1!.GlobalPosition = new Vector2I(-8 * TileSize, 0 - WallHeightInPixels);
        _leftWallInstance2!.GlobalPosition = new Vector2I(-8 * TileSize, 0 - WallHeightInPixels * 2);
        _rightWallInstance0!.GlobalPosition = new Vector2I(31 * TileSize, 0);
        _rightWallInstance1!.GlobalPosition = new Vector2I(31 * TileSize, 0 - WallHeightInPixels);
        _rightWallInstance2!.GlobalPosition = new Vector2I(31 * TileSize, 0 - WallHeightInPixels * 2);

        AddChild(_leftWallInstance0);
        AddChild(_leftWallInstance1);
        AddChild(_leftWallInstance2);
        AddChild(_rightWallInstance0);
        AddChild(_rightWallInstance1);
        AddChild(_rightWallInstance2);
    }

    private void UpdateWalls()
    {
        var leftWallPositions = new List<float>
        {
            _leftWallInstance0.GlobalPosition.Y,
            _leftWallInstance1.GlobalPosition.Y,
            _leftWallInstance2.GlobalPosition.Y
        };

        var rightWallPositions = new List<float>
        {
            _rightWallInstance0.GlobalPosition.Y,
            _rightWallInstance1.GlobalPosition.Y,
            _rightWallInstance2.GlobalPosition.Y
        };

        leftWallPositions.Sort();
        rightWallPositions.Sort();

        var highestLeftWallY = leftWallPositions[0];
        var lowestLeftWallY = leftWallPositions[2];
        var highestRightWallY = rightWallPositions[0];
        var lowestRightWallY = rightWallPositions[2];

        // Handle left walls
        if (_player.GlobalPosition.Y < highestLeftWallY + WallHeightInPixels / 2)
        {
            MoveWall(_leftWallInstance0, _leftWallInstance1, _leftWallInstance2, lowestRightWallY, highestRightWallY,
                true);
        }
        else if (_player.GlobalPosition.Y > lowestLeftWallY - WallHeightInPixels / 2)
        {
            MoveWall(_leftWallInstance0, _leftWallInstance1, _leftWallInstance2, lowestLeftWallY,
                highestLeftWallY, false);
        }

        // Handle right walls
        if (_player.GlobalPosition.Y < highestRightWallY + WallHeightInPixels / 2)
        {
            MoveWall(_rightWallInstance0, _rightWallInstance1, _rightWallInstance2, lowestRightWallY,
                highestRightWallY, true);
        }
        else if (_player.GlobalPosition.Y > lowestRightWallY - WallHeightInPixels / 2)
        {
            MoveWall(_rightWallInstance0, _rightWallInstance1, _rightWallInstance2, lowestRightWallY,
                highestRightWallY, false);
        }
    }

    private void MoveWall(Node2D wall0, Node2D wall1, Node2D wall2, float lowestWallY, float highestWallY,
        bool moveAbove)
    {
        var newYPosition = moveAbove ? highestWallY - WallHeightInPixels : lowestWallY + WallHeightInPixels;

        if (Math.Abs(wall0.GlobalPosition.Y - (moveAbove ? lowestWallY : highestWallY)) < 10)
            wall0.GlobalPosition = new Vector2(wall0.GlobalPosition.X, newYPosition);
        else if (Math.Abs(wall1.GlobalPosition.Y - (moveAbove ? lowestWallY : highestWallY)) < 10)
            wall1.GlobalPosition = new Vector2(wall1.GlobalPosition.X, newYPosition);
        else if (Math.Abs(wall2.GlobalPosition.Y - (moveAbove ? lowestWallY : highestWallY)) < 10)
            wall2.GlobalPosition = new Vector2(wall2.GlobalPosition.X, newYPosition);
    }

    private int CalculateTilesFromPixels(int pixels)
    {
        return pixels / TileSize;
    }

    private int CalculatePixelsFromTiles(int tiles)
    {
        return tiles * TileSize;
    }

    private (double, double) GetPlayerPositionInTiles()
    {
        return (Math.Ceiling(_player.GlobalPosition.X / TileSize), Math.Ceiling(_player.GlobalPosition.Y / TileSize));
    }
}