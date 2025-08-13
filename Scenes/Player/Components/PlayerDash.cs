using Godot;
using System;

public partial class PlayerDash : Node
{
    // ---- Dash motion ----
    [Export] public float DashDistance = 300f;
    [Export] public float DashSpeed = 1200f;
    [Export] public float MaxDashTime = 0.30f;           // safety cap
    [Export] public bool PreserveYVelocity = true;       // keep gravity/jump Y while dashing
    [Export] public bool StopOnWallHit = true;           // stop if we barely move

    // ---- Charges & recharge ----
    [Export] public int   MaxCharges   = 2;
    [Export] public float RechargeTime = 1.0f;

    [Export] public bool RequireInputDirection = false;

    [Signal] public delegate void ChargesChangedEventHandler(int current, int max);
    [Signal] public delegate void DashStartedEventHandler();
    [Signal] public delegate void DashEndedEventHandler();

    private bool _active;
    private float _left;              // distance left to travel
    private Vector2 _direction;       // normalized (±1, 0)

    private int _charges;
    private float _rechargeTimer;

    private float _dashTimer;         // safety cap
    private Vector2 _lastPos;         // to measure actual distance moved

    private Action _onFinished;

    public bool IsDashing => _active;
    public int Charges => _charges;

    public override void _Ready()
    {
        _charges = Mathf.Max(1, MaxCharges);
        _rechargeTimer = 0f;
        EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_charges < MaxCharges)
        {
            _rechargeTimer += (float)delta;
            while (_charges < MaxCharges && _rechargeTimer >= RechargeTime)
            {
                _rechargeTimer -= RechargeTime;
                _charges++;
                EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);
            }
        }
    }

    public bool CanStartDash() => !_active && _charges > 0;

    public void StartDash(CharacterBody2D body, float inputAxis, Action onFinished)
    {
        if (!CanStartDash()) return;
        if (RequireInputDirection && Mathf.IsZeroApprox(inputAxis)) return;

        _charges = Mathf.Max(0, _charges - 1);
        GD.Print($"[Dash] start; charges={_charges}/{MaxCharges}");
        EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);

        _active = true;
        _onFinished = onFinished;

        _left = Mathf.Max(0f, DashDistance);
        float sign = Mathf.Sign(Mathf.IsZeroApprox(inputAxis) ? 1f : inputAxis);
        _direction = new Vector2(sign, 0f);

        _dashTimer = 0f;
        _lastPos = body.GlobalPosition;

        // set initial velocity (MoveAndSlide happens in Player)
        body.Velocity = PreserveYVelocity
            ? new Vector2(_direction.X * DashSpeed, body.Velocity.Y)
            : _direction * DashSpeed;

        EmitSignal(SignalName.DashStarted);
    }

    /// Call BEFORE MoveAndSlide() each physics frame while dashing.
    public void TickDash(CharacterBody2D body, double delta)
    {
        if (!_active) return;

        _dashTimer += (float)delta;
        if (_dashTimer >= MaxDashTime)
        {
            EndDash(body);
            _onFinished?.Invoke();
            _onFinished = null;
            return;
        }

        // keep applying dash velocity; Player will MoveAndSlide() once
        body.Velocity = PreserveYVelocity
            ? new Vector2(_direction.X * DashSpeed, body.Velocity.Y)
            : _direction * DashSpeed;
    }

    /// Call AFTER MoveAndSlide() each physics frame while dashing.
    public void AfterSlide(CharacterBody2D body)
    {
        if (!_active) return;

        float moved = body.GlobalPosition.DistanceTo(_lastPos);
        _lastPos = body.GlobalPosition;

        // if we bounced into a wall, movement might be tiny
        if (StopOnWallHit && moved < 0.5f)
        {
            EndDash(body);
            _onFinished?.Invoke();
            _onFinished = null;
            return;
        }

        _left -= moved;
        if (_left <= 0f)
        {
            EndDash(body);
            _onFinished?.Invoke();
            _onFinished = null;
        }
    }

    public void EndDash(CharacterBody2D body)
    {
        if (!_active) return;

        _active = false;
        _left = 0f;
        body.Velocity = PreserveYVelocity
            ? new Vector2(0f, body.Velocity.Y)
            : Vector2.Zero;

        EmitSignal(SignalName.DashEnded);
        GD.Print("[Dash] end");
    }

    // helpers
    public void RefillCharges()
    {
        _charges = MaxCharges;
        _rechargeTimer = 0f;
        EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);
    }

    public void SetCharges(int count)
    {
        int clamped = Mathf.Clamp(count, 0, MaxCharges);
        if (clamped == _charges) return;
        _charges = clamped;
        EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);
    }
}
