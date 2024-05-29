using Godot;
using System;
using System.Collections.Generic;

public partial class GameController : Node
{
    private const int BackgroundLayer = -1;
    private const int WallLayer = 0;
    private const int CellSize = 16;
    private const int StartingPlatformHeight = 3;

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

    private readonly Vector2I _leftWallAtlasCoords = new Vector2I(3, 6);
    private readonly Vector2I _rightWallAtlasCoords = new Vector2I(1, 6);
    
    private readonly Vector2I _groundTileAtlasCoord = new Vector2I(2, 1);
    private readonly Vector2I _groundTileAtlasCoord2 = new Vector2I(2, 5);
    private readonly Vector2I _groundTileAtlasCoord3 = new Vector2I(2, 2);


    private TileMap _backgroundTileMap;
    private TileMap _wallTileMap;
    private RandomNumberGenerator _rng = new RandomNumberGenerator();
    private CharacterBody2D _player;

    private int _leftBoundary = -20;
    private int _rightBoundary = 20;
    private int _topBoundary = 20;
    private int _bottomBoundary = 0;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _backgroundTileMap = GetNode<TileMap>("BackgroundTileMap");
        _wallTileMap = GetNode<TileMap>("WallTileMap");
        _player = GetNode<CharacterBody2D>("Player");

        _backgroundTileMap.ZIndex = BackgroundLayer;
        _wallTileMap.ZIndex = WallLayer;
        

        GenerateStartingPlatform();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var playerPosition = _player.GlobalPosition;
    }


    private void GenerateStartingPlatform()
    {
        var playerPosition = _player.GlobalPosition;

        var startX = (int)playerPosition.X / CellSize + _leftBoundary;
        var startY = (int)playerPosition.Y / CellSize + 1;

        for (var x = startX; x < startX + (_rightBoundary - _leftBoundary); x++)
        {
            for (var y = 0; y < StartingPlatformHeight; y++)
            {
                var tileCoord = (y == 0) ? _groundTileAtlasCoord : _groundTileAtlasCoord3;
                _wallTileMap.SetCell(WallLayer, new Vector2I(x, startY + y), 0, tileCoord, 0);
            }
        }
    }    private Vector2I GetRandomAtlasCoords()
    {
        var randomIndex = _rng.RandiRange(0, _backgroundTileCoords.Count - 1);
        return _backgroundTileCoords[randomIndex];
    }
}