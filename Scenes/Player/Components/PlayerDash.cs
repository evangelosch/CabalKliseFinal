using Godot;
using System;

public partial class PlayerDash : Node
{
    [Export] public PlayerStats Stats;
    [Export] public int MaxCharges = 2;

    [Signal] public delegate void ChargesChangedEventHandler(int current, int max);

    // Runtime from Stats
    private float _dashDistance;   // pixels
    private float _dashSpeed;      // px/s
    private float _rechargeTime;   // seconds per charge

    // Dash runtime
    private bool _active;
    private Vector2 _dir;          // (-1,0) or (1,0)
    private float _travelled;      // px

    // Track movement across frames
    private Vector2 _lastPos;

    // Charges
    private int _charges;
    private float _rechargeTimer;

    private Action _onFinished;    // notify statechart "dash_done"

    public bool IsDashing => _active;
    public int  Charges   => _charges;

    public override void _Ready()
    {
        ApplyStats();
        _charges = MaxCharges;        // start full
        _rechargeTimer = 0f;
        EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);
    }

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
            _rechargeTime = 1.0f;
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
                _charges++;
                EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);
            }
        }
    }

    public bool CanStartDash() => !_active && _charges > 0;

    // Called from Player.MvEnterDash()
    public void StartDash(CharacterBody2D body, float axis, Action onFinished)
    {
        if (!CanStartDash()) return;

        float sign = Mathf.Sign(axis);
        if (Mathf.IsZeroApprox(sign)) return; // need left/right direction

        _charges--;
        _rechargeTimer = 0f;
        EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);

        _active = true;
        _travelled = 0f;
        _dir = new Vector2(sign, 0f);

        _onFinished = onFinished;
        _lastPos = body.GlobalPosition;

        // Set dash velocity now; Player will MoveAndSlide() once per frame
        body.Velocity = _dir * _dashSpeed;
    }

    // NEW: split tick so distance is measured AFTER movement
    public void TickDashPre(CharacterBody2D body)
    {
        if (!_active) return;
        body.Velocity = _dir * _dashSpeed;
    }

    public void TickDashPost(CharacterBody2D body, Vector2 prevPos)
    {
        if (!_active) return;

        // distance actually moved since MoveAndSlide
        float step = (body.GlobalPosition - prevPos).Length();
        _travelled += step;

        // Stop if we reached target distance OR we got blocked (no movement)
        bool blocked = step < 0.5f;
        if (_travelled >= _dashDistance || blocked)
        {
            EndDash(body);
            _onFinished?.Invoke();
            _onFinished = null;
        }

        _lastPos = body.GlobalPosition;
    }

    public void EndDash(CharacterBody2D body)
    {
        if (!_active) return;
        _active = false;
        _travelled = 0f;
        body.Velocity = Vector2.Zero;
    }
}
