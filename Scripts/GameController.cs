using Godot;
using System;
using System.Collections.Generic;


public partial class GameController : Node
{
    private const int BackgroundLayer = -1;
    private const int WallLayer = 0;
    private const int TileSize = 16;
    private const int WallHeightInPixels = 960;
    private const int StartingPlatformWidthInPixels = 640;
    private const int StartingPlatformWidthInTiles = 40;


    private readonly List<Vector2I> _backgroundTileCoords = new List<Vector2I>
    {
        new Vector2I(6, 6),
        new Vector2I(6, 7),
        new Vector2I(7, 6),
        new Vector2I(7, 7),
        new Vector2I(8, 6),
        new Vector2I(8, 7),
        new Vector2I(9, 7),
        new Vector2I(9, 6),
        new Vector2I(10, 6),
        new Vector2I(10, 7),
        new Vector2I(11, 6),
        new Vector2I(11, 7),
        new Vector2I(12, 1),
        new Vector2I(12, 2),
        new Vector2I(12, 3),
        new Vector2I(12, 4),
        new Vector2I(12, 5),
        new Vector2I(12, 6),
        new Vector2I(12, 7),
        new Vector2I(13, 1),
        new Vector2I(13, 2),
        new Vector2I(13, 3)
    };


    private TileMap _backgroundTileMap;
    private TileMap _wallTileMap;

    private RandomNumberGenerator _rng = new RandomNumberGenerator();
    private CharacterBody2D _player;

    private Viewport _viewport;

    // scenes
    private PackedScene _leftWallScene;
    private PackedScene _rightWallScene;

    // instances
    private Node2D _leftWallInstance0;
    private Node2D _leftWallInstance1;
    private Node2D _leftWallInstance2;
    private Node2D _rightWallInstance0;
    private Node2D _rightWallInstance1;
    private Node2D _rightWallInstance2;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _player = GetNode<CharacterBody2D>("Player");
        _viewport = GetViewport();
        _backgroundTileMap = GetNode<TileMap>("BackgroundTileMap");
        _wallTileMap = GetNode<TileMap>("WallTileMap");

        _leftWallScene = (PackedScene)ResourceLoader.Load("res://prefabs/left_wall.tscn");
        _rightWallScene = (PackedScene)ResourceLoader.Load("res://prefabs/right_wall.tscn");

        _backgroundTileMap.ZIndex = BackgroundLayer;
        _wallTileMap.ZIndex = WallLayer;


        InitializeWalls();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var playerGlobalPosition = _player.GlobalPosition;

        if (Input.IsActionJustPressed("debug_key"))
        {
            GD.Print("Player Global Position:", playerGlobalPosition);
            GD.Print("**************************************************");
            GD.Print("LeftWall0 Y Position: ", _leftWallInstance0.GlobalPosition.Y);
            GD.Print("LeftWall1 Y Position: ", _leftWallInstance1.GlobalPosition.Y);
            GD.Print("LeftWall2 Y Position: ", _leftWallInstance2.GlobalPosition.Y);
            GD.Print("**************************************************");
            GD.Print("RightWall0 Y Position: ", _rightWallInstance0.GlobalPosition.Y);
            GD.Print("RightWall1 Y Position: ", _rightWallInstance1.GlobalPosition.Y);
            GD.Print("RightWall2 Y Position: ", _rightWallInstance2.GlobalPosition.Y);
        }

        UpdateWalls();
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

        float highestLeftWallY = leftWallPositions[0];
        float lowestLeftWallY = leftWallPositions[2];
        float highestRightWallY = rightWallPositions[0];
        float lowestRightWallY = rightWallPositions[2];

        // Handle left walls
        if (_player.GlobalPosition.Y < highestLeftWallY + WallHeightInPixels / 2)
        {
            MoveWallAbove(ref _leftWallInstance0, ref _leftWallInstance1, ref _leftWallInstance2, lowestLeftWallY, highestLeftWallY);
        }
        else if (_player.GlobalPosition.Y > lowestLeftWallY - WallHeightInPixels / 2)
        {
            MoveWallBelow(ref _leftWallInstance0, ref _leftWallInstance1, ref _leftWallInstance2, lowestLeftWallY, highestLeftWallY);
        }

        // Handle right walls
        if (_player.GlobalPosition.Y < highestRightWallY + WallHeightInPixels / 2)
        {
            MoveWallAbove(ref _rightWallInstance0, ref _rightWallInstance1, ref _rightWallInstance2, lowestRightWallY, highestRightWallY);
        }
        else if (_player.GlobalPosition.Y > lowestRightWallY - WallHeightInPixels / 2)
        {
            MoveWallBelow(ref _rightWallInstance0, ref _rightWallInstance1, ref _rightWallInstance2, lowestRightWallY, highestRightWallY);
        }
    }
    
    private void MoveWallAbove(ref Node2D wall0, ref Node2D wall1, ref Node2D wall2, float lowestWallY, float highestWallY)
    {
        if (wall0.GlobalPosition.Y == lowestWallY)
            wall0.GlobalPosition = new Vector2(wall0.GlobalPosition.X, highestWallY - WallHeightInPixels);
        else if (wall1.GlobalPosition.Y == lowestWallY)
            wall1.GlobalPosition = new Vector2(wall1.GlobalPosition.X, highestWallY - WallHeightInPixels);
        else if (wall2.GlobalPosition.Y == lowestWallY)
            wall2.GlobalPosition = new Vector2(wall2.GlobalPosition.X, highestWallY - WallHeightInPixels);
    }

    private void MoveWallBelow(ref Node2D wall0, ref Node2D wall1, ref Node2D wall2, float lowestWallY, float highestWallY)
    {
        if (wall0.GlobalPosition.Y == highestWallY)
            wall0.GlobalPosition = new Vector2(wall0.GlobalPosition.X, lowestWallY + WallHeightInPixels);
        else if (wall1.GlobalPosition.Y == highestWallY)
            wall1.GlobalPosition = new Vector2(wall1.GlobalPosition.X, lowestWallY + WallHeightInPixels);
        else if (wall2.GlobalPosition.Y == highestWallY)
            wall2.GlobalPosition = new Vector2(wall2.GlobalPosition.X, lowestWallY + WallHeightInPixels);
    }

    private Vector2I GetRandomAtlasCoords()
    {
        var randomIndex = _rng.RandiRange(0, _backgroundTileCoords.Count - 1);
        return _backgroundTileCoords[randomIndex];
    }


    private int CalculateTilesFromPixels(int pixels)
    {
        return pixels / TileSize;
    }

    private int CalculatePixelsFromTiles(int tiles)
    {
        return tiles * TileSize;
    }
}