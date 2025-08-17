using Godot;

public partial class EnemyMovement : Node
{
    [Export] public float Speed { get; set; } = 160f;

    public void Stop(CharacterBody2D body)
    {
        var v = body.Velocity;
        v.X = 0;
        body.Velocity = v;
    }

    // Move until we reach targetX (±eps). Returns true when arrived.
    public bool RunToX(CharacterBody2D body, float targetX, double dt, float arriveEps = 2f)
    {
        float dx = targetX - body.GlobalPosition.X;
        if (Mathf.Abs(dx) <= arriveEps)
        {
            Stop(body);
            return true;
        }
        float dir = Mathf.Sign(dx);
        var v = body.Velocity;
        v.X = dir * Speed;
        body.Velocity = v;
        return false;
    }

    public void ClampToScreenX(CharacterBody2D body, float spriteWidthPx)
    {
        float half = spriteWidthPx * 0.5f;
        float minX = half;
        float maxX = body.GetViewportRect().Size.X - half;
        var p = body.Position;
        p.X = Mathf.Clamp(p.X, minX, maxX);
        body.Position = p;
    }
}
