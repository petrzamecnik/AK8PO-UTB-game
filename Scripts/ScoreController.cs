using Godot;

namespace AK8POUTBgame.Scripts;

public partial class ScoreController : Node
{

	private Label _scoreLabel;
	private CharacterBody2D _player;
	private int _startingPosition;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_scoreLabel = GetNode<Label>("PanelContainer/GridContainer/ScoreLabel");
		_player = GetParent().GetNode<CharacterBody2D>("Player");
		_startingPosition = (int)_player.GlobalPosition.Y;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var score = (int)((_startingPosition - _player.GlobalPosition.Y) / 100);
		_scoreLabel.Text = $" Score: {score} ";
	}
}