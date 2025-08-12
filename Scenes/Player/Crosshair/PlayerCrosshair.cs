using Godot;

public partial class PlayerCrosshair : Node
{
    private Area2D _area;

    public override void _Ready()
    {
        // Your scene should be:
        // PlayerCrosshair (Node)
        // └── Area2D
        //     ├── CollisionShape2D
        //     └── Sprite2D (optional)
        _area = GetNode<Area2D>("Area2D");

        // Bring above most 2D
        _area.ZIndex = 100;

        // Hide OS cursor if you show a custom crosshair sprite
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    public override void _Process(double delta)
    {
        // If your camera updates in Physics, move this to _PhysicsProcess
        _area.GlobalPosition = GetViewport().GetMousePosition();
    }

    public Godot.Collections.Array<Node2D> GetEnemiesUnderCrosshair()
    {
        // Area2D.GetOverlappingBodies() returns PhysicsBody2D, so cast to Node2D where possible
        var result = new Godot.Collections.Array<Node2D>();
        foreach (var body in _area.GetOverlappingBodies())
        {
            if (body is Node2D n2d) result.Add(n2d);
        }
        return result;
    }
}
