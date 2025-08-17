using Godot;
using GodotStateCharts;
using System;
using System.Collections.Generic;

public partial class Enemy : CharacterBody2D
{
    [Export] public float DefaultDashSpeed = 900f;
    [Export] public float ScreenMargin = 24f;

    private StateChart _chart;
    private EnemyMovement _move;
    private EnemyShoot _shoot;

    // Command queue / script
    private readonly Queue<EnemyCommand> _queue = new();
    private readonly List<EnemyCommand> _scriptBackup = new();
    private EnemyCommand _current;
    private bool _loop;

    // Walk bookkeeping
    private float _targetX;

    // Dash bookkeeping
    private bool _dashing;
    private float _dashSpeed;
    private float _dashDistLeft;
    private float _dashDir; // -1 / +1

    public override void _Ready()
    {
        _chart = StateChart.Of(GetNode("StateChart"));
        _move  = GetNode<EnemyMovement>("Components/EnemyMovement");
        _shoot = GetNode<EnemyShoot>("Components/EnemyShoot");

        ConnectState("StateChart/Root/AI/Idle",  enter: AiEnterIdle);
        ConnectState("StateChart/Root/AI/Walk",  enter: AiEnterWalk,  physics: AiUpdateWalk);
        ConnectState("StateChart/Root/AI/Wait",  enter: AiEnterWait,  physics: AiUpdateWait);
        ConnectState("StateChart/Root/AI/Shoot", enter: AiEnterShoot, physics: AiUpdateShoot);
        ConnectState("StateChart/Root/AI/Dash",  enter: AiEnterDash,  physics: AiUpdateDash);

        // start idle; you’ll call StartScript() after enqueueing
    }

    public override void _PhysicsProcess(double dt)
    {
        MoveAndSlide();
        _move.ClampToScreenX(this, GetSpriteWidthPx());
    }

    // ------- Public scripting API (fluent-ish) -------
    public Enemy SpawnLeftTop(float y, float margin = 8f)
    {
        var vr = GetViewport().GetVisibleRect();
        var pos = new Vector2(vr.Position.X + margin, y);
        _queue.Enqueue(EnemyCommand.SetPos(pos));
        _scriptBackup.Add(EnemyCommand.SetPos(pos));
        return this;
    }

    public Enemy SpawnRightTop(float y, float margin = 8f)
    {
        var vr = GetViewport().GetVisibleRect();
        var pos = new Vector2(vr.Position.X + vr.Size.X - margin, y);
        _queue.Enqueue(EnemyCommand.SetPos(pos));
        _scriptBackup.Add(EnemyCommand.SetPos(pos));
        return this;
    }

    public Enemy WalkLeft(float distance)  => Enq(EnemyCommand.Walk(-1f, distance));
    public Enemy WalkRight(float distance) => Enq(EnemyCommand.Walk(+1f, distance));
    public Enemy StopFor(float seconds)    => Enq(EnemyCommand.Wait(Mathf.Max(0f, seconds)));
    public Enemy ShootBurst(int shots, float fireRate = 6f) => Enq(EnemyCommand.Shoot(shots, fireRate));
    public Enemy DashLeft(float distance, float? speed = null)  => Enq(EnemyCommand.Dash(-1f, distance, speed ?? DefaultDashSpeed));
    public Enemy DashRight(float distance, float? speed = null) => Enq(EnemyCommand.Dash(+1f, distance, speed ?? DefaultDashSpeed));

    public Enemy Loop(bool loop) { _loop = loop; return this; }

    public void StartScript()
    {
        // kick the machine
        _chart.SendEvent("to_wait"); // trivial nudge to ensure Idle runs enter once
        // immediately go back to Idle
        _chart.SendEvent("wait_done");
    }

    // Helper to enqueue both in queue & backup for looping
    private Enemy Enq(EnemyCommand c)
    {
        _queue.Enqueue(c);
        _scriptBackup.Add(c);
        return this;
    }

    // ------- State callbacks -------
    private void AiEnterIdle()
    {
        // If queue empty and looping, refill
        if (_queue.Count == 0 && _loop)
            foreach (var c in _scriptBackup) _queue.Enqueue(c);

        if (_queue.Count == 0) return; // nothing to do

        _current = _queue.Dequeue();

        switch (_current.Type)
        {
            case EnemyCmdType.SetPos:
                GlobalPosition = new Vector2(_current.F1, _current.F2);
                // instantly complete and pull next
                AiEnterIdle(); 
                break;

            case EnemyCmdType.Walk:
                _chart.SendEvent("to_walk");
                break;

            case EnemyCmdType.Wait:
                _chart.SendEvent("to_wait");
                break;

            case EnemyCmdType.Shoot:
                _chart.SendEvent("to_shoot");
                break;

            case EnemyCmdType.Dash:
                _chart.SendEvent("to_dash");
                break;
        }
    }

    // --- Walk ---
    private void AiEnterWalk()
    {
        float dir = Mathf.Sign(_current.F1);
        float distance = Mathf.Max(0f, _current.F2);
        _targetX = ComputeReachableTargetX(dir, distance);
    }

    private void AiUpdateWalk(double dt)
    {
        if (_move.RunToX(this, _targetX, dt))
            _chart.SendEvent("walk_done");
    }

    // --- Wait ---
    private float _waitLeft;
    private void AiEnterWait() => _waitLeft = _current.F1;
    private void AiUpdateWait(double dt)
    {
        _waitLeft -= (float)dt;
        if (_waitLeft <= 0f) _chart.SendEvent("wait_done");
    }

    // --- Shoot ---
    private void AiEnterShoot()
    {
        int shots = Math.Max(1, _current.I1);
        float rate = (_current.F1 > 0f) ? _current.F1 : 6f;
        _move.Stop(this);
        _shoot.BeginBurst(shots, rate);
    }

    private void AiUpdateShoot(double dt)
    {
        if (_shoot.TickBurst(this, dt))
            _chart.SendEvent("shoot_done");
    }

    // --- Dash (horizontal blink-ish move using velocity over distance) ---
    private void AiEnterDash()
    {
        _dashing = true;
        _dashDir = Mathf.Sign(_current.F1);
        _dashDistLeft = Mathf.Max(0f, _current.F2);
        _dashSpeed = (_current.F3 > 0f) ? _current.F3 : DefaultDashSpeed;
        Velocity = new Vector2(_dashDir * _dashSpeed, 0f);
    }

    private void AiUpdateDash(double dt)
    {
        // distance traveled this frame (after MoveAndSlide in _PhysicsProcess)
        float step = MathF.Abs(Velocity.X) * (float)dt;
        _dashDistLeft -= step;
        Velocity = new Vector2(_dashDir * _dashSpeed, 0f);

        if (_dashDistLeft <= 0f || Mathf.IsZeroApprox(step))
        {
            _dashing = false;
            Velocity = Vector2.Zero;
            _chart.SendEvent("dash_done");
        }
    }

    // ------- Helpers -------
    private float GetSpriteWidthPx()
    {
        var s = GetNodeOrNull<Sprite2D>("Sprite2D");
        if (s?.Texture != null) return s.Texture.GetWidth() * s.Scale.X;
        return 64f;
    }

    private float ComputeReachableTargetX(float dir, float distance)
    {
        // clamp target so we can actually reach it (respect sprite half width)
        float half = GetSpriteWidthPx() * 0.5f;
        var vr = GetViewportRect().Size;
        float minX = half + ScreenMargin;
        float maxX = vr.X - half - ScreenMargin;

        float raw = GlobalPosition.X + dir * distance;
        return Mathf.Clamp(raw, minX, maxX);
    }

    private void ConnectState(string nodePath, Action enter = null, Action<double> physics = null, Action exit = null)
    {
        var st = StateChartState.Of(GetNode(nodePath));
        if (enter   != null) st.Connect(StateChartState.SignalName.StateEntered,           Callable.From(enter));
        if (physics != null) st.Connect(StateChartState.SignalName.StatePhysicsProcessing, Callable.From<double>(physics));
        if (exit    != null) st.Connect(StateChartState.SignalName.StateExited,            Callable.From(exit));
    }
}
