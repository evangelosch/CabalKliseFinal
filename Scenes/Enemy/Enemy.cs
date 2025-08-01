using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    [Export] public float Speed = 200f;
    [Export] public int Direction = 1; // 1 = right, -1 = left

    public override void _PhysicsProcess(double delta)
    {
        Velocity = new Vector2(Direction * Speed, 0);
        MoveAndSlide();

        // Clamp enemy inside the viewport horizontally
        var viewport = GetViewportRect();
        int spriteWidth = 32; // Replace with your enemy sprite's width
        float minX = spriteWidth / 2f;
        float maxX = viewport.Size.X - spriteWidth / 2f;

        Vector2 clampedPosition = Position;
        clampedPosition.X = Mathf.Clamp(Position.X, minX, maxX);
        Position = clampedPosition;
    }
}