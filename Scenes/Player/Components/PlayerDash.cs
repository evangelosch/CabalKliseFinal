// PlayerDash.cs (drop-in replacement of your class)
using Godot;
using System;

public partial class PlayerDash : Node
{
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
                _charges++;
                EmitSignal(SignalName.ChargesChanged, _charges, MaxCharges);
            }
        }
    }

    public bool CanStartDash() => !_active && _charges > 0;

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

    public void EndDash(CharacterBody2D body)
    {
        if (!_active) return;
        _active = false;
        body.Velocity = Vector2.Zero;
    }
}
