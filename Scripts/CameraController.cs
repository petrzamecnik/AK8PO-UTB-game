using Godot;

public partial class CameraController : Camera2D
{
    private const int CellSize = 16;

    private CharacterBody2D _player;
    private Vector2 _viewportRect;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
        
        _player = GetParent<CharacterBody2D>();
        _viewportRect = GetViewportRect().Size;

        PositionSmoothingEnabled = true;
        PositionSmoothingSpeed = 10;
        
        LimitBottom = (int)_viewportRect.Y;
        LimitLeft = - (CellSize * 11);
        LimitRight = (int)(CellSize * 11 + _viewportRect.X);
        
        GD.Print(_viewportRect.X);
        GD.Print(_viewportRect.Y);



    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var playerPosition = _player.GlobalPosition;

        GlobalPosition = new Vector2(playerPosition.X, playerPosition.Y - 160);
    }
}