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

        GenerateInitialTiles();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void UpdateTiles()
    {
        var playerY = (int)_player.GlobalPosition.Y;

        if (playerY > _highestYGenerated - _tileBuffer)
        {
            var newHighestY = _highestYGenerated + _tileBuffer;
            GenerateBackground(0, 20, _highestYGenerated, newHighestY);
            GenerateLeftWall(_highestYGenerated, newHighestY);
            GenerateRightWall(_highestYGenerated, newHighestY);
            _highestYGenerated = newHighestY;
        }

        if (playerY < _highestYGenerated + _tileBuffer)
        {
            var newLowestY = _lowestYGenerated - _tileBuffer;
            GenerateBackground(0, 20, newLowestY, _lowestYGenerated);
            GenerateLeftWall(newLowestY, _lowestYGenerated);
            GenerateRightWall(newLowestY, _lowestYGenerated);
            _lowestYGenerated = newLowestY;
        }

        RemoveTilesAbove(playerY + _tileBuffer * 2);
        RemoveTilesBelow(playerY - _tileBuffer * 2);
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
        for (var x = 0; x < 20; x++)
        {
            for (var y = aboveY; y < _highestYGenerated; y++)
            {
                _backgroundTileMap.SetCell(BackgroundLayer, new Vector2I(x, y), -1);
                _wallTileMap.SetCell(WallLayer, new Vector2I(x, y), -1);
            }
        }

        _highestYGenerated = aboveY;
    }

    private void RemoveTilesBelow(int belowY)
    {
        for (var x = 0; x < 20; x++)
        {
            for (var y = _lowestYGenerated; y < belowY; y++)
            {
                _backgroundTileMap.SetCell(BackgroundLayer, new Vector2I(x, y), -1);
                _wallTileMap.SetCell(WallLayer, new Vector2I(x, y), -1);
            }
        }

        _lowestYGenerated = belowY;
    }
}