<<<<<<< HEAD
// PlayerDash.cs (drop-in replacement of your class)
=======
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
using Godot;
using System;

public partial class PlayerDash : Node
{
<<<<<<< HEAD
    [Export] public PlayerStats Stats;
    [Export] public int MaxCharges = 2;

    [Signal] public delegate void ChargesChangedEventHandler(int current, int max);

    private float _dashDistance;
    private float _dashSpeed;
    private float _rechargeTime;

    private bool _active;
    private Vector2 _dir;       // (-1,0) or (1,0)
    private float _travelled;   // px
    private Vector2 _lastPos;

    private int _charges;
    private float _rechargeTimer;
    private Action _onFinished;

    public bool IsDashing => _active;
    public int  Charges   => _charges;

    public override void _Ready()
    {
        ApplyStats();
        _charges = MaxCharges;
=======
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
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
        _rechargeTimer = 0f;
        EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);
    }

<<<<<<< HEAD
    public void SetStats(PlayerStats stats)
    {
        Stats = stats;
        ApplyStats();
    }

    private void ApplyStats()
    {
        if (Stats == null)
        {
            GD.PushWarning("[PlayerDash] Stats is null; using defaults.");
            _dashDistance = 160f;
            _dashSpeed    = 900f;
            _rechargeTime = 0.35f;
            return;
        }

        _dashDistance = Stats.DashDistance;
        _dashSpeed    = Stats.DashSpeed;
        _rechargeTime = Mathf.Max(0.01f, Stats.DashCooldown);
    }

    public override void _PhysicsProcess(double delta)
    {
        // Recharge
        if (_charges < MaxCharges)
        {
            _rechargeTimer += (float)delta;
            while (_charges < MaxCharges && _rechargeTimer >= _rechargeTime)
            {
                _rechargeTimer -= _rechargeTime;
=======
    public override void _PhysicsProcess(double delta)
    {
        if (_charges < MaxCharges)
        {
            _rechargeTimer += (float)delta;
            while (_charges < MaxCharges && _rechargeTimer >= RechargeTime)
            {
                _rechargeTimer -= RechargeTime;
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
                _charges++;
                EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);
            }
        }
    }

    public bool CanStartDash() => !_active && _charges > 0;

<<<<<<< HEAD
    public void StartDash(CharacterBody2D body, float axis, Action onFinished)
    {
        GD.Print($"[StartDash] pre: active={_active}, charges={_charges}, axis={axis}");
        if (!CanStartDash()) return;

        float sign = Mathf.Sign(axis);
        if (Mathf.IsZeroApprox(sign)) return; // need direction

        _charges--;
        _rechargeTimer = 0f;
        EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);

        _active = true;
        _travelled = 0f;
        _dir = new Vector2(sign, 0f);
        _onFinished = onFinished;

        _lastPos = body.GlobalPosition;
        body.Velocity = _dir * _dashSpeed;
    }

    public void TickDashPre(CharacterBody2D body)
{
    if (!_active) return;
    body.Velocity = _dir * _dashSpeed;
}

public void TickDashPost(CharacterBody2D body, Vector2 prevPos)
{
    if (!_active) return;

    float step = (body.GlobalPosition - prevPos).Length();
    _travelled += step;

    bool blocked = step < 0.5f;
    if (_travelled >= _dashDistance || blocked)
    {
        EndDash(body);
        _onFinished?.Invoke();
        _onFinished = null;
    }
}
=======
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
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea

    public void EndDash(CharacterBody2D body)
    {
        if (!_active) return;
<<<<<<< HEAD
        _active = false;
        body.Velocity = Vector2.Zero;
=======

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
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
    }
}
