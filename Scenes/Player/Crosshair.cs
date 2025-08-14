// Crosshair.cs
using Godot;

public partial class Crosshair : Node2D
{
    [Export] public bool ClampToViewport = true;
    [Export] public Vector2 Margin = new(8, 8);
    [Export(PropertyHint.Range, "0,20,0.1")] public float Smooth = 0f; // 0 = instant

    public override void _Process(double dt)
    {
        Vector2 target = GetGlobalMousePosition();

        if (ClampToViewport)
        {
            var vp = GetViewportRect().Size;
            target.X = Mathf.Clamp(target.X, Margin.X, vp.X - Margin.X);
            target.Y = Mathf.Clamp(target.Y, Margin.Y, vp.Y - Margin.Y);
        }

        if (Smooth <= 0f) GlobalPosition = target;
        else
        {
            float t = 1f - Mathf.Exp(-Smooth * (float)dt);
            GlobalPosition = GlobalPosition.Lerp(target, t);
        }
    }
}
