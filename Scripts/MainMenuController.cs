using Godot;
using System;

public partial class MainMenuController : Node
{
	private PackedScene _mainScene;

	public override void _Ready()
	{
		_mainScene = (PackedScene)ResourceLoader.Load("res://Scenes/Main.tscn");

	}

	private void _on_start_game_button_pressed()
	{
		var mainSceneInstance = _mainScene.Instantiate();
		
		GetTree().Root.AddChild(mainSceneInstance);
		GetTree().CurrentScene.QueueFree();
		GetTree().CurrentScene = mainSceneInstance;
	}

	private void _on_exit_game_button_pressed()
	{
		GetTree().Quit();
	}
}
