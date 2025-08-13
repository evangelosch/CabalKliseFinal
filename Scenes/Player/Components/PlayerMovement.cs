using Godot;

public partial class PlayerMovement : Node
{
    [Export] public int SpriteWidth = 80;
    [Export] public int SpriteHeight = 50;

    // This will be set from PlayerStats via Player.cs
    public float Speed { get; set; }

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
        var velocity = body.Velocity;
        velocity.X = input * Speed;

        float minX = SpriteWidth / 2f;
        float maxX = body.GetViewportRect().Size.X - SpriteWidth / 2f;

        Vector2 newPos = body.Position + velocity * (float)delta;
        newPos.X = Mathf.Clamp(newPos.X, minX, maxX);
        newPos.Y = body.Position.Y;

        body.Position = newPos;
        body.Velocity = velocity;
    }
}
