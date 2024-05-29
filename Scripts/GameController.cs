using Godot;
using System;
using System.Collections.Generic;

public partial class GameController : Node
{
    private TileMap _backgroundTileMap;
    private TileMap _wallTileMap;
    private RandomNumberGenerator _rng = new RandomNumberGenerator();
    private CharacterBody2D _player;
    

    private int _tileBuffer = 20;
    private int _lowestYGenerated = 0;
    private int _highestYGenerated = 40;

    private const int BackgroundLayer = -1;
    private const int WallLayer = 0;

    private readonly List<Vector2I> _tileCoords = new List<Vector2I>
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


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _backgroundTileMap = GetNode<TileMap>("BackgroundTileMap");
        _wallTileMap = GetNode<TileMap>("WallTileMap");
        _player = GetNode<CharacterBody2D>("Player");

        _backgroundTileMap.ZIndex = BackgroundLayer;
        _wallTileMap.ZIndex = WallLayer;

        GenerateInitialTiles();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        UpdateTiles();
    }

    private void UpdateTiles()
    {
       
    }

    private void GenerateInitialTiles()
    {
        GenerateBackground(0, 20, _lowestYGenerated, _highestYGenerated);
        GenerateLeftWall(_lowestYGenerated, _highestYGenerated);
        GenerateRightWall(_lowestYGenerated, _highestYGenerated);
    }

    private void GenerateBackground(int startX, int endX, int startY, int endY)
    {
        for (var x = startX; x < endX; x++)
        {
            for (var y = startY; y < endY; y++)
            {
                var randomAtlasCoords = GetRandomAtlasCoords();
                _backgroundTileMap.SetCell(BackgroundLayer, new Vector2I(x, y), 0, randomAtlasCoords, 0);
            }
        }
    }

    private void GenerateLeftWall(int startY, int endY)
    {
        for (var y = startY; y < endY; y++)
        {
            _wallTileMap.SetCell(WallLayer, new Vector2I(0, y), 0, _leftWallAtlasCoords);
        }
    }

    private void GenerateRightWall(int startY, int endY)
    {
        for (var y = startY; y < endY; y++)
        {
            _wallTileMap.SetCell(WallLayer, new Vector2I(19, y), 0, _rightWallAtlasCoords);
        }
    }

    private Vector2I GetRandomAtlasCoords()
    {
        var randomIndex = _rng.RandiRange(0, _tileCoords.Count - 1);
        return _tileCoords[randomIndex];
    }

    private void RemoveTilesAbove(int aboveY)
    {

        
      
    }

    private void RemoveTilesBelow(int belowY)
    {

        
    }
}