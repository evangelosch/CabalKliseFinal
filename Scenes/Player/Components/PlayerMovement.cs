// PlayerMovement.cs (replace with this)
using Godot;

public partial class PlayerMovement : Node
{
    [Export] public int SpriteWidth = 80;
    [Export] public int SpriteHeight = 50;

    public float Speed { get; set; } = 220f;

    public void SpawnAtBottom(CharacterBody2D body)
    {
        var viewport = body.GetViewportRect();
        body.Position = new Vector2(viewport.Size.X / 2f, viewport.Size.Y - SpriteHeight);
    }

    public void Stop(CharacterBody2D body)
    {
        var v = body.Velocity;
        v.X = 0;
        body.Velocity = v;
    }

    public void Run(CharacterBody2D body, float input, double delta)
    {
        var v = body.Velocity;
        v.X = input * Speed;
        body.Velocity = v;
    }

    public void ClampToScreen(CharacterBody2D body)
    {
        float half = SpriteWidth / 2f;
        float minX = half;
        float maxX = body.GetViewportRect().Size.X - half;
        var p = body.Position;
        p.X = Mathf.Clamp(p.X, minX, maxX);
        body.Position = p;
    }
}
