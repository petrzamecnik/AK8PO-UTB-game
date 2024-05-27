using Godot;
using System;
using System.Collections.Generic;

public partial class GameController : Node
{
	private TileMap _tileMap;
	private RandomNumberGenerator _rng = new RandomNumberGenerator();

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
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GenerateBackground();
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void GenerateBackground()
	{
		_tileMap = GetNode<TileMap>("Terrain/TerrainTileSet");
		_tileMap.ZIndex = -9;
		
		for (var x = 0; x < 20; x++) 
		{
			for (var y = 0; y < 40; y++)
			{
				var randomAtlasCoords = GetRandomAtlasCoords();
				_tileMap.SetCell(0, new Vector2I(x, y), 0, randomAtlasCoords, 0);
			}
		}
	}
	
	private Vector2I GetRandomAtlasCoords()
	{
		var randomIndex = _rng.RandiRange(0, _tileCoords.Count - 1);
		return _tileCoords[randomIndex];
	}
}
