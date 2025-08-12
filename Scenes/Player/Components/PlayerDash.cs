using Godot;
using System;

public partial class PlayerDash : Node
{
    private bool _active;
    private float _left;
    private Vector2 _dir;

    [Export] public float DashDistance = 120f;
    [Export] public float DashSpeed = 1200f;
    [Export] public float Cooldown = 0.8f;

    public void StartDash(CharacterBody2D body, float inputAxis, Action onFinished)
    {
        if (_active) return;
        _active = true;
        _left = DashDistance;
        _dir = new Vector2(Mathf.Sign(Mathf.IsZeroApprox(inputAxis) ? 1f : inputAxis), 0f);

        body.Velocity = _dir * DashSpeed;
        _onFinished = onFinished;
    }

    public void TickDash(CharacterBody2D body, double delta)
    {
        if (!_active) return;
        float step = DashSpeed * (float)delta;
        float moved = Mathf.Min(step, _left);
        _left -= moved;

        body.Velocity = _dir * DashSpeed;
        if (_left <= 0f)
        {
            EndDash(body);
            _onFinished?.Invoke();
        }
    }

    public void EndDash(CharacterBody2D body)
    {
        _active = false;
        body.Velocity = Vector2.Zero;
    }

    public void BeginCooldown(Action onReady)
    {
        var t = GetTree().CreateTimer(Mathf.Max(Cooldown, 0.01f));
        t.Timeout += () => onReady?.Invoke();
    }

    private Action _onFinished;
}
