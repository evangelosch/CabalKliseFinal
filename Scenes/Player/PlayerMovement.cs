// PlayerMovement.cs (replace with this)
using Godot;

public partial class PlayerMovement : Node
{
    [Export] public float Speed = 500f;
    [Export] public int SpriteWidth = 80;
    [Export] public int SpriteHeight = 50;

<<<<<<< Updated upstream:Scenes/Player/PlayerMovement.cs
    // Call once if you want spawn-at-bottom logic
=======
    public float Speed { get; set; } = 220f;

>>>>>>> Stashed changes:Scenes/Player/Components/PlayerMovement.cs
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
<<<<<<< Updated upstream:Scenes/Player/PlayerMovement.cs

        float minX = SpriteWidth / 2f;
        float maxX = body.GetViewportRect().Size.X - SpriteWidth / 2f;

        Vector2 newPos = body.Position + v * (float)delta;
        newPos.X = Mathf.Clamp(newPos.X, minX, maxX);
        newPos.Y = body.Position.Y; // keep Y fixed

        body.Position = newPos;
        body.Velocity = v;
=======
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
>>>>>>> Stashed changes:Scenes/Player/Components/PlayerMovement.cs
    }
}
