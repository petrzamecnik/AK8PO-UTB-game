using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameController : Node
{
    [Signal]
    public delegate void KillPlayerEventHandler();

    private const int TileSize = 16;
    private const int WallHeightInPixels = 960;
    private const int BackgroundHeightInPixels = 1600;
    private const int PlatformLeftBoundary = -100;
    private const int PlatformRightBoundary = 400;
    private const int MinPlatformDistance = 30;
    private const int MaxPlatformDistance = 100;
    private const int LookAheadDistance = 1000;
    private const int PlatformBuffer = 3;

    private CharacterBody2D _player;
    private Random _rnd = new Random();
    private float _highestPlayerPosition;
    private Node2D _lastPlatform;
    private List<Node2D> _platforms = new List<Node2D>();
    private float _lowestPlatformY;
    private bool _playerCanDie;

    private PackedScene _leftWallScene, _rightWallScene, _backGroundScene, _platform1Scene, _platform2Scene;
    private Node2D[] _leftWallInstances = new Node2D[3];
    private Node2D[] _rightWallInstances = new Node2D[3];
    private Node2D[] _backgroundInstances = new Node2D[2];

    public override void _EnterTree() => AddUserSignal(nameof(KillPlayerEventHandler));

    public override void _Ready()
    {
        InitializePlayer();
        LoadScenes();
        InitializeGame();
    }

    public override void _Process(double delta)
    {
        DebugInfo();
        UpdateGameElements();
        CheckPlayerStatus();
    }

    private void InitializePlayer()
    {
        _player = GetNode<CharacterBody2D>("Player");
        _highestPlayerPosition = _player.GlobalPosition.Y;
    }

    private void LoadScenes()
    {
        _leftWallScene = ResourceLoader.Load("res://prefabs/left_wall.tscn") as PackedScene;
        _rightWallScene = ResourceLoader.Load("res://prefabs/right_wall.tscn") as PackedScene;
        _backGroundScene = ResourceLoader.Load("res://prefabs/background_tile_map.tscn") as PackedScene;
        _platform1Scene = ResourceLoader.Load("res://Prefabs/platform_1.tscn") as PackedScene;
        _platform2Scene = ResourceLoader.Load("res://Prefabs/platform_2.tscn") as PackedScene;
    }

    private void InitializeGame()
    {
        _lastPlatform = SpawnPlatform(150, 500);
        _lowestPlatformY = _lastPlatform.GlobalPosition.Y;
        InitializeWalls();
        InitializeBackground();
    }

    private void DebugInfo()
    {
        if (Input.IsActionJustPressed("debug_key"))
        {
            GD.Print("Player Global Position:", _player.GlobalPosition);
            GD.Print("Highest player pos: ", _highestPlayerPosition);
        }
    }

    private void UpdateGameElements()
    {
        UpdateWalls();
        UpdateBackground();
        GeneratePlatforms();
    }

    private void CheckPlayerStatus()
    {
        PlayerCanDieToggle();
        KillPlayerIfTooLow();
    }

    private void GeneratePlatforms()
    {
        var spawnTriggerY = _player.GlobalPosition.Y - LookAheadDistance;

        while (_lastPlatform.GlobalPosition.Y > spawnTriggerY || _platforms.Count < PlatformBuffer)
        {
            var newPlatformY = _lastPlatform.GlobalPosition.Y - _rnd.Next(MinPlatformDistance, MaxPlatformDistance + 1);
            var minX = Mathf.Max((int)_lastPlatform.GlobalPosition.X - 100, PlatformLeftBoundary);
            var maxX = Mathf.Min((int)_lastPlatform.GlobalPosition.X + 100, PlatformRightBoundary);
            float newPlatformX = _rnd.Next(minX, maxX + 1);

            _lastPlatform = SpawnPlatform(newPlatformX, newPlatformY);
        }

        RemoveOldPlatforms();
    }

    private void RemoveOldPlatforms()
    {
        while (_platforms.Count > 0 && _platforms[0].GlobalPosition.Y > _player.GlobalPosition.Y + 500)
        {
            _platforms[0].QueueFree();
            _platforms.RemoveAt(0);
        }
    }

    private Node2D SpawnPlatform(float x, float y)
    {
        var chosenPlatform = _rnd.Next() % 2 == 0 ? _platform1Scene : _platform2Scene;
        var platformToSpawn = chosenPlatform.Instantiate() as Node2D;
        platformToSpawn!.GlobalPosition = new Vector2(x, y);
        AddChild(platformToSpawn);
        _platforms.Add(platformToSpawn);
        return platformToSpawn;
    }

    private void InitializeBackground()
    {
        for (var i = 0; i < 2; i++)
        {
            _backgroundInstances[i] = _backGroundScene.Instantiate() as Node2D;
            _backgroundInstances[i]!.GlobalPosition = new Vector2I(0, -i * BackgroundHeightInPixels);
            AddChild(_backgroundInstances[i]);
        }
    }

    private void UpdateBackground()
    {
        UpdateBackgroundPosition(_backgroundInstances[0], _backgroundInstances[1]);
        UpdateBackgroundPosition(_backgroundInstances[1], _backgroundInstances[0]);
    }

    private void UpdateBackgroundPosition(Node2D background1, Node2D background2)
    {
        const int offset = 200;

        if (_player.GlobalPosition.Y < background2.GlobalPosition.Y - offset)
        {
            background1.GlobalPosition = new Vector2(background1.GlobalPosition.X, background2.GlobalPosition.Y - BackgroundHeightInPixels);
        }
        else if (_player.GlobalPosition.Y > background2.GlobalPosition.Y + offset)
        {
            background1.GlobalPosition = new Vector2(background1.GlobalPosition.X, background2.GlobalPosition.Y + BackgroundHeightInPixels);
        }
    }

    private void InitializeWalls()
    {
        for (var i = 0; i < 3; i++)
        {
            _leftWallInstances[i] = _leftWallScene.Instantiate() as Node2D;
            _rightWallInstances[i] = _rightWallScene.Instantiate() as Node2D;

            _leftWallInstances[i]!.GlobalPosition = new Vector2I(-8 * TileSize, -i * WallHeightInPixels);
            _rightWallInstances[i]!.GlobalPosition = new Vector2I(31 * TileSize, -i * WallHeightInPixels);

            AddChild(_leftWallInstances[i]);
            AddChild(_rightWallInstances[i]);
        }
    }

    private void UpdateWalls()
    {
        UpdateWallSide(_leftWallInstances);
        UpdateWallSide(_rightWallInstances);
    }

    private void UpdateWallSide(Node2D[] walls)
    {
        var wallPositions = new List<float> { walls[0].GlobalPosition.Y, walls[1].GlobalPosition.Y, walls[2].GlobalPosition.Y };
        wallPositions.Sort();

        var highestWallY = wallPositions[0];
        var lowestWallY = wallPositions[2];

        if (_player.GlobalPosition.Y < highestWallY + WallHeightInPixels / 2)
        {
            MoveWall(walls[0], walls[1], walls[2], lowestWallY, highestWallY, true);
        }
        else if (_player.GlobalPosition.Y > lowestWallY - WallHeightInPixels / 2)
        {
            MoveWall(walls[0], walls[1], walls[2], lowestWallY, highestWallY, false);
        }
    }

    private void MoveWall(Node2D wall0, Node2D wall1, Node2D wall2, float lowestWallY, float highestWallY, bool moveAbove)
    {
        var newYPosition = moveAbove ? highestWallY - WallHeightInPixels : lowestWallY + WallHeightInPixels;
        var targetY = moveAbove ? lowestWallY : highestWallY;

        var wallToMove = new[] { wall0, wall1, wall2 }
            .FirstOrDefault(w => Math.Abs(w.GlobalPosition.Y - targetY) < 10);

        if (wallToMove != null)
        {
            wallToMove.GlobalPosition = new Vector2(wallToMove.GlobalPosition.X, newYPosition);
        }
    }

    private void PlayerCanDieToggle()
    {
        if (_player.GlobalPosition.Y < 0)
        {
            _playerCanDie = true;
        }
    }

    private void KillPlayerIfTooLow()
    {
        if (_player.IsOnFloor() && _player.GlobalPosition.Y > _lowestPlatformY && _playerCanDie)
        {
            EmitSignal(nameof(KillPlayerEventHandler));
        }
    }
}