using Godot;
using GodotStateCharts;
using System;

public partial class Player : CharacterBody2D
{
    [Export] public PlayerStats PlayerStats;
    [Export] public Weapon StartingWeapon;

    private StateChart _stateChart;
    private PlayerMovement _movement;
    private PlayerDash _dash;
    private PlayerShoot _shoot;
    private PlayerHealth _health;

    // Gate so Dash state can't trigger without a real button press
    private bool _dashRequested;
    private float _axis;

    // Events (MUST match chart)
    private const string EVT_MOVE_UPDATE = "movement_update";
    private const string EVT_DASH_PRESS  = "dash_pressed";
    private const string EVT_DASH_DONE   = "dash_done";
    private const string EVT_FIRE_PRESS  = "fire_pressed";
    private const string EVT_FIRE_REL    = "fire_released";

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Hidden;
        _stateChart = StateChart.Of(GetNode("StateChart"));
        _movement   = GetNode<PlayerMovement>("Components/PlayerMovement");
        _dash       = GetNode<PlayerDash>("Components/PlayerDash");
        _shoot      = GetNode<PlayerShoot>("Components/PlayerShoot");
        _health     = GetNode<PlayerHealth>("Components/PlayerHealth");

        if (PlayerStats != null)
        {
            _movement.Speed = PlayerStats.MoveSpeed;
            _dash.SetStats(PlayerStats); // dash pulls distance/speed/cooldown from stats
        }

        if (StartingWeapon != null)
            _shoot.Equip(StartingWeapon);

        _movement.SpawnAtBottom(this);

        // Init chart expressions
        _stateChart.SetExpressionProperty("movement_input", 0f);
        _stateChart.SetExpressionProperty("movement_abs", 0f);
        _stateChart.SetExpressionProperty("allow_dash_shoot", true);
        _stateChart.SetExpressionProperty("can_dash", _dash.Charges > 0);
        _stateChart.SetExpressionProperty("shoot_cd", 0f);

        // --- InputBus subscriptions ---
        var inputBus = GetNode<InputBus>("/root/InputBus");
        inputBus.MoveAxis     += OnMoveAxis;
        inputBus.DashPressed  += OnDashPressed;
        inputBus.FirePressed  += OnFirePressed;
        inputBus.FireReleased += OnFireReleased;

        // Keep can_dash in sync via signal (useful for UI/guard)
        _dash.ChargesChanged += OnDashChargesChanged;

        // --- State connections ---
        // Movement
        ConnectState("StateChart/Root/Movement/Grounded/Idle", enter: MvEnterIdle, physics: MvUpdateIdle);
        ConnectState("StateChart/Root/Movement/Grounded/Run",  enter: MvEnterRun,  physics: MvUpdateRun);

        // Dash (start on Enter, tick from _PhysicsProcess pre/post)
        ConnectState("StateChart/Root/Movement/Dash",
            enter:   MvEnterDash,
            physics: MvUpdateDash,
            exit:    MvExitDash);

        // Combat
        ConnectState("StateChart/Root/Combat/Aim",   physics: CbUpdateAim);
        ConnectState("StateChart/Root/Combat/Shoot", enter: CbEnterShooting, physics: CbUpdateShooting, exit: CbExitShooting);
    }

    public override void _ExitTree()
    {
        var inputBus = GetNodeOrNull<InputBus>("/root/InputBus");
        if (inputBus != null)
        {
            inputBus.MoveAxis     -= OnMoveAxis;
            inputBus.DashPressed  -= OnDashPressed;
            inputBus.FirePressed  -= OnFirePressed;
            inputBus.FireReleased -= OnFireReleased;
        }

        if (IsInstanceValid(_dash))
            _dash.ChargesChanged -= OnDashChargesChanged;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Keep chart guard fresh
        _stateChart.SetExpressionProperty("can_dash", _dash.Charges > 0);

        if (_dash.IsDashing)
        {
            // IMPORTANT: measure AFTER moving
            var prev = GlobalPosition;
            _dash.TickDashPre(this);   // sets dash velocity for this frame
            MoveAndSlide();            // perform the move
            _dash.TickDashPost(this, prev); // measure travelled & possibly end dash
            return; // skip normal movement during dash
        }

        // normal movement
        MoveAndSlide();
        _movement.ClampToScreen(this);
    }

    // ------- InputBus handlers -------
    private void OnMoveAxis(float axis)
    {
        _axis = axis;
        _stateChart.SetExpressionProperty("movement_input", _axis);
        _stateChart.SetExpressionProperty("movement_abs", Mathf.Abs(_axis));
        _stateChart.SendEvent(EVT_MOVE_UPDATE);
    }

    private void OnDashPressed()
    {
        // Sample live axis once at press
        _axis = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        _dashRequested = true;
        _stateChart.SendEvent(EVT_DASH_PRESS);
    }

    private void OnFirePressed()  => _stateChart.SendEvent(EVT_FIRE_PRESS);
    private void OnFireReleased() => _stateChart.SendEvent(EVT_FIRE_REL);

    private void OnDashChargesChanged(int current, int max)
        => _stateChart.SetExpressionProperty("can_dash", current > 0);

    // ------- Movement callbacks -------
    private void MvEnterIdle()           => _movement.Stop(this);
    private void MvUpdateIdle(double dt) { /* noop */ }

    private void MvEnterRun()            { /* start run anim if desired */ }
    private void MvUpdateRun(double dt)  => _movement.Run(this, _axis, dt); // sets Velocity.X

    // ------- Dash callbacks -------
    private void MvEnterDash()
    {
        // hard gate so accidental startup transitions can't consume a dash
        if (!_dashRequested || !_dash.CanStartDash())
        {
            _stateChart.SendEvent(EVT_DASH_DONE); // bounce back immediately
            return;
        }

        _dashRequested = false; // consume request

        _dash.StartDash(this, _axis, () => _stateChart.SendEvent(EVT_DASH_DONE));

        // If StartDash refused (e.g., axis == 0 / no charges), ensure we exit the Dash state
        if (!_dash.IsDashing)
            _stateChart.SendEvent(EVT_DASH_DONE);
    }

    private void MvUpdateDash(double dt) { /* dash moved centrally in _PhysicsProcess */ }

    private void MvExitDash() => _dash.EndDash(this); // idempotent

    // ------- Combat callbacks -------
    private void CbUpdateAim(double dt)
    {
        _shoot.TickAim(dt);
        _shoot.TickCooldown(dt);
        _stateChart.SetExpressionProperty("shoot_cd", _shoot.Cooldown);
    }

    private void CbEnterShooting()
    {
        _shoot.BeginBurst();
        _stateChart.SetExpressionProperty("shoot_cd", _shoot.Cooldown);
    }

    private void CbUpdateShooting(double dt)
    {
        _shoot.TickShooting(dt);
        _stateChart.SetExpressionProperty("shoot_cd", _shoot.Cooldown);
    }

    private void CbExitShooting() => _shoot.EndBurst();

    // ------- Helper -------
    private void ConnectState(string nodePath, System.Action enter = null,
                              System.Action<double> physics = null,
                              System.Action exit = null)
    {
        var st = StateChartState.Of(GetNode(nodePath));
        if (enter   != null) st.Connect(StateChartState.SignalName.StateEntered,           Callable.From(enter));
        if (physics != null) st.Connect(StateChartState.SignalName.StatePhysicsProcessing, Callable.From<double>(physics));
        if (exit    != null) st.Connect(StateChartState.SignalName.StateExited,            Callable.From(exit));
    }
}
