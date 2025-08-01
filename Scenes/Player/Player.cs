using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public const float Speed = 500.0f;

    public override void _Ready()
    {
        var viewport = GetViewportRect();
        int spriteHeight = 50; // Replace with your actual sprite height

        // Spawn at bottom center
        Position = new Vector2(viewport.Size.X / 2, viewport.Size.Y - spriteHeight);
    }

public override void _PhysicsProcess(double delta)
{
    Vector2 velocity = Velocity;

    float input = 0f;
    if (Input.IsActionPressed("ui_left"))
        input -= 1f;
    if (Input.IsActionPressed("ui_right"))
        input += 1f;

    velocity.X = input * Speed;

    int spriteWidth = 80; // Your sprite's width
    float minX = spriteWidth / 2f;
    float maxX = GetViewportRect().Size.X - spriteWidth / 2f;

    Vector2 newPosition = Position + velocity * (float)delta;
    newPosition.X = Mathf.Clamp(newPosition.X, minX, maxX);
    // Keep Y fixed at bottom
    newPosition.Y = Position.Y;

    Position = newPosition;
    Velocity = velocity;
}
}