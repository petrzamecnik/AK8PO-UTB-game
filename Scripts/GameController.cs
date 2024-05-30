using Godot;
using System;
using System.Collections.Generic;

public partial class GameController : Node
{
    private const int BackgroundLayer = -1;
    private const int WallLayer = 0;
    private const int CellSize = 16;

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
    private StaticBody2D _leftWall;
    private StaticBody2D _rightWall;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _backgroundTileMap = GetNode<TileMap>("BackgroundTileMap");
        _wallTileMap = GetNode<TileMap>("WallTileMap");
        _player = GetNode<CharacterBody2D>("Player");
        _leftWall = GetNode<StaticBody2D>("LeftWall");
        _rightWall = GetNode<StaticBody2D>("RightWall");
        
        _backgroundTileMap.ZIndex = BackgroundLayer;
        _wallTileMap.ZIndex = WallLayer;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var playerGlobalPosition = _player.GlobalPosition;

        if (Input.IsActionJustPressed("debug_key"))
        {
            GD.Print("Player Global Position:", playerGlobalPosition);
            GD.Print("Player X position tiles:", CalculateTilesFromPixels((int)playerGlobalPosition.X));
            GD.Print("Player Y position tiles:", CalculateTilesFromPixels((int)playerGlobalPosition.Y));
        }
        
        UpdateWallPosition(playerGlobalPosition);
    }

    private void UpdateWallPosition(Vector2 playerGlobalPosition)
    {
        _leftWall.GlobalPosition = _leftWall.GlobalPosition with { Y = playerGlobalPosition.Y };
        _rightWall.GlobalPosition = _rightWall.GlobalPosition with { Y = playerGlobalPosition.Y };
    }


    private Vector2I GetRandomAtlasCoords()
    {
        var randomIndex = _rng.RandiRange(0, _backgroundTileCoords.Count - 1);
        return _backgroundTileCoords[randomIndex];
    }


    private int CalculateTilesFromPixels(int pixels)
    {
        return pixels / CellSize;
    }
}