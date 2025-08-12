using Godot;
using System;

public partial class PlayerCrosshair : Node2D
{
    public Area2D Area;

    public override void _Ready()
    {
        ZIndex = 100;
		Input.MouseMode = Input.MouseModeEnum.Hidden;
        Area = GetNode<Area2D>("Area2D");
    }

    public override void _Process(double delta)
    {
    GlobalPosition = GetViewport().GetMousePosition();
    }

    public Godot.Collections.Array<Node2D> GetEnemiesUnderCrosshair()
    {
        return Area.GetOverlappingBodies();
    }
}