<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< HEAD
// PlayerDash.cs (drop-in replacement of your class)
=======
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
// PlayerDash.cs (drop-in replacement of your class)
>>>>>>> Stashed changes
=======
// PlayerDash.cs (drop-in replacement of your class)
>>>>>>> Stashed changes
using Godot;
using System;

public partial class PlayerDash : Node
{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
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
=======
    [Export] public PlayerStats Stats;
    [Export] public int MaxCharges = 2;
>>>>>>> Stashed changes
=======
    [Export] public PlayerStats Stats;
    [Export] public int MaxCharges = 2;
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        _charges = Mathf.Max(1, MaxCharges);
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
        ApplyStats();
        _charges = MaxCharges;
>>>>>>> Stashed changes
=======
        ApplyStats();
        _charges = MaxCharges;
>>>>>>> Stashed changes
        _rechargeTimer = 0f;
        EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);
    }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
    public override void _PhysicsProcess(double delta)
    {
        // Recharge
        if (_charges < MaxCharges)
        {
            _rechargeTimer += (float)delta;
            while (_charges < MaxCharges && _rechargeTimer >= _rechargeTime)
            {
                _rechargeTimer -= _rechargeTime;
<<<<<<< Updated upstream
=======
=======
>>>>>>> Stashed changes
    public override void _PhysicsProcess(double delta)
    {
        // Recharge
        if (_charges < MaxCharges)
        {
            _rechargeTimer += (float)delta;
            while (_charges < MaxCharges && _rechargeTimer >= _rechargeTime)
            {
<<<<<<< Updated upstream
                _rechargeTimer -= RechargeTime;
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
                _rechargeTimer -= _rechargeTime;
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
                _charges++;
                EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);
            }
        }
    }

    public bool CanStartDash() => !_active && _charges > 0;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    }
}
=======
    public void StartDash(CharacterBody2D body, float inputAxis, Action onFinished)
=======
    public void StartDash(CharacterBody2D body, float axis, Action onFinished)
>>>>>>> Stashed changes
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
=======
>>>>>>> Stashed changes
    }
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
<<<<<<< Updated upstream
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
}
>>>>>>> Stashed changes

    public void EndDash(CharacterBody2D body)
    {
        if (!_active) return;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
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
=======
        _active = false;
        body.Velocity = Vector2.Zero;
>>>>>>> Stashed changes
=======
        _active = false;
        body.Velocity = Vector2.Zero;
>>>>>>> Stashed changes
    }
}
