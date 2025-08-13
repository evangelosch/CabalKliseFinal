// PlayerMovement.cs (replace with this)
using Godot;

public partial class PlayerMovement : Node
{
    [Export] public int SpriteWidth = 80;
    [Export] public int SpriteHeight = 50;

<<<<<<< Updated upstream
<<<<<<< HEAD
    public float Speed { get; set; } = 220f;

=======
<<<<<<<< HEAD:Scenes/Player/PlayerMovement.cs
<<<<<<< Updated upstream:Scenes/Player/PlayerMovement.cs
    // Call once if you want spawn-at-bottom logic
=======
    public float Speed { get; set; } = 220f;

>>>>>>> Stashed changes:Scenes/Player/Components/PlayerMovement.cs
========
    // This will be set from PlayerStats via Player.cs
    public float Speed { get; set; }
=======
    public float Speed { get; set; } = 220f;
>>>>>>> Stashed changes

>>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea:Scenes/Player/Components/PlayerMovement.cs
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
    public void SpawnAtBottom(CharacterBody2D body)
    {
        var viewport = body.GetViewportRect();
        body.Position = new Vector2(viewport.Size.X / 2f, viewport.Size.Y - SpriteHeight);
    }

    public void Stop(CharacterBody2D body)
    {
<<<<<<< Updated upstream
<<<<<<< HEAD
        var v = body.Velocity;
        v.X = 0;
=======
        var v = body.Velocity; 
        v.X = 0; 
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
        var v = body.Velocity;
        v.X = 0;
>>>>>>> Stashed changes
        body.Velocity = v;
    }

    public void Run(CharacterBody2D body, float input, double delta)
    {
<<<<<<< Updated upstream
<<<<<<< HEAD
        var v = body.Velocity;
        v.X = input * Speed;
=======
<<<<<<<< HEAD:Scenes/Player/PlayerMovement.cs
        var v = body.Velocity;
        v.X = input * Speed;
<<<<<<< Updated upstream:Scenes/Player/PlayerMovement.cs
========
        var velocity = body.Velocity;
        velocity.X = input * Speed;
>>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea:Scenes/Player/Components/PlayerMovement.cs

        float minX = SpriteWidth / 2f;
        float maxX = body.GetViewportRect().Size.X - SpriteWidth / 2f;

        Vector2 newPos = body.Position + velocity * (float)delta;
        newPos.X = Mathf.Clamp(newPos.X, minX, maxX);
        newPos.Y = body.Position.Y;

        body.Position = newPos;
<<<<<<<< HEAD:Scenes/Player/PlayerMovement.cs
        body.Velocity = v;
=======
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
        body.Velocity = v;
    }

=======
        var v = body.Velocity;
        v.X = input * Speed;
        body.Velocity = v;
    }

>>>>>>> Stashed changes
    public void ClampToScreen(CharacterBody2D body)
    {
        float half = SpriteWidth / 2f;
        float minX = half;
        float maxX = body.GetViewportRect().Size.X - half;
        var p = body.Position;
        p.X = Mathf.Clamp(p.X, minX, maxX);
        body.Position = p;
<<<<<<< Updated upstream
<<<<<<< HEAD
=======
>>>>>>> Stashed changes:Scenes/Player/Components/PlayerMovement.cs
========
        body.Velocity = velocity;
>>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea:Scenes/Player/Components/PlayerMovement.cs
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
>>>>>>> Stashed changes
    }
}
