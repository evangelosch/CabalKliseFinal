// Player.cs — only the changed/important parts shown
using Godot;
<<<<<<< Updated upstream
using GodotStateCharts; // IMPORTANT: plugin’s C# namespace

public partial class Player : CharacterBody2D
{
    private StateChart _sc;            // wrapper for the StateChart node
    private PlayerMovement _move;
    private PlayerInput _input;
    private float _axis;               // -1..1
=======
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

    private bool _dashRequested;
    private float _axis;
>>>>>>> Stashed changes

    private const string EVT_MOVE_UPDATE = "movement_update";
    private const string EVT_DASH_PRESS  = "dash_pressed";
    private const string EVT_DASH_DONE   = "dash_done";
    private const string EVT_FIRE_PRESS  = "fire_pressed";
    private const string EVT_FIRE_REL    = "fire_released";

    private const string EVT_MOVE_UPDATE = "movement_update";
    private const string EVT_DASH_PRESS  = "dash_pressed";
    private const string EVT_DASH_DONE   = "dash_done";
    private const string EVT_FIRE_PRESS  = "fire_pressed";
    private const string EVT_FIRE_REL    = "fire_released";

    private const string EVT_MOVE_UPDATE = "movement_update";
    private const string EVT_DASH_PRESS  = "dash_pressed";
    private const string EVT_DASH_DONE   = "dash_done";
    private const string EVT_FIRE_PRESS  = "fire_pressed";
    private const string EVT_FIRE_REL    = "fire_released";

    public override void _Ready()
    {
<<<<<<< Updated upstream
        _sc    = StateChart.Of(GetNode("StateChart"));
        _move  = GetNode<PlayerMovement>("PlayerMovement");
        _input = GetNode<PlayerInput>("PlayerInput");
=======
        _stateChart = StateChart.Of(GetNode("StateChart"));
        _movement   = GetNode<PlayerMovement>("Components/PlayerMovement");
        _dash       = GetNode<PlayerDash>("Components/PlayerDash");
        _shoot      = GetNode<PlayerShoot>("Components/PlayerShoot");
        _health     = GetNode<PlayerHealth>("Components/PlayerHealth");

        if (PlayerStats != null)
        {
            _movement.Speed = PlayerStats.MoveSpeed;
            _dash.SetStats(PlayerStats);
        }

        if (StartingWeapon != null)
            _shoot.Equip(StartingWeapon);
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes

        _movement.SpawnAtBottom(this);

<<<<<<< Updated upstream
        // Init expression properties (used in guards)
        _sc.SetExpressionProperty("move_input", 0f);
        _sc.SetExpressionProperty("move_abs",   0f);

        // Hook input → chart vars/events
        _input.MoveAxis += OnMoveAxis;

        // Hook state callbacks via signals (so we don’t need Editor connections)
        // Idle callbacks
        var stIdle = StateChartState.Of(GetNode("StateChart/Root/Movement/Grounded/Idle"));
        stIdle.Connect(StateChartState.SignalName.StateEntered,            Callable.From(OnEnterIdle));
        stIdle.Connect(StateChartState.SignalName.StatePhysicsProcessing,  Callable.From<double>(OnUpdateIdle));
        stIdle.Connect(StateChartState.SignalName.StateExited,             Callable.From(OnExitIdle));

        // Run callbacks
        var stRun = StateChartState.Of(GetNode("StateChart/Root/Movement/Grounded/Run"));
        stRun.Connect(StateChartState.SignalName.StateEntered,            Callable.From(OnEnterRun));
        stRun.Connect(StateChartState.SignalName.StatePhysicsProcessing,  Callable.From<double>(OnUpdateRun));
        stRun.Connect(StateChartState.SignalName.StateExited,             Callable.From(OnExitRun));
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide(); // movement is applied inside the state callbacks
    }

    // ===== Input → chart =====
    private void OnMoveAxis(float axis)
    {
        _axis = axis;
        _sc.SetExpressionProperty("move_input", _axis);
        _sc.SetExpressionProperty("move_abs", Mathf.Abs(_axis));
        _sc.SendEvent("move_update"); // drives the Idle<->Run transitions
    }

    // ===== State callbacks =====
    private void OnEnterIdle() { _move.Stop(this); }
    private void OnUpdateIdle(double dt) { /* no-op */ }
    private void OnExitIdle() { }

    private void OnEnterRun() { /* start run anim if needed */ }
    private void OnUpdateRun(double dt) => _move.Run(this, _axis, dt);
    private void OnExitRun()  { /* stop run anim if needed */ }
=======
=======

        _movement.SpawnAtBottom(this);

>>>>>>> Stashed changes
=======

        _movement.SpawnAtBottom(this);

>>>>>>> Stashed changes
        _stateChart.SetExpressionProperty("movement_input", 0f);
        _stateChart.SetExpressionProperty("movement_abs", 0f);
        _stateChart.SetExpressionProperty("allow_dash_shoot", true);
        _stateChart.SetExpressionProperty("can_dash", _dash.Charges > 0);
        _stateChart.SetExpressionProperty("shoot_cd", 0f);

        var inputBus = GetNode<InputBus>("/root/InputBus");
        inputBus.MoveAxis     += OnMoveAxis;
        inputBus.DashPressed  += OnDashPressed;
        inputBus.FirePressed  += OnFirePressed;
        inputBus.FireReleased += OnFireReleased;

        _dash.ChargesChanged += OnDashChargesChanged;

        // Movement states
        ConnectState("StateChart/Root/Movement/Grounded/Idle", enter: MvEnterIdle, physics: MvUpdateIdle);
        ConnectState("StateChart/Root/Movement/Grounded/Run",  enter: MvEnterRun,  physics: MvUpdateRun);

        // Dash state
        ConnectState("StateChart/Root/Movement/Dash",
            enter:   MvEnterDash,
            physics: MvUpdateDash,
            exit:    MvExitDash);

        // Combat states
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
        _stateChart.SetExpressionProperty("can_dash", _dash.Charges > 0);

        if (_dash.IsDashing)
        {
            var prev = GlobalPosition;       // capture BEFORE moving
            _dash.TickDashPre(this);         // set dash velocity
            MoveAndSlide();                  // now we actually move
            _dash.TickDashPost(this, prev);  // measure AFTER moving, decide end
            return;
        }

        MoveAndSlide();
        _movement.ClampToScreen(this);
    }

    // -------- Input handlers
    private void OnMoveAxis(float axis)
    {
        _axis = axis;
        _stateChart.SetExpressionProperty("movement_input", _axis);
        _stateChart.SetExpressionProperty("movement_abs", Mathf.Abs(_axis));
        _stateChart.SendEvent(EVT_MOVE_UPDATE);
    }

    private void OnDashPressed()
    {
        _axis = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        GD.Print($"[InputBus] Dash pressed, axis={_axis}");
        _dashRequested = true;
        _stateChart.SendEvent(EVT_DASH_PRESS);
    }

    private void OnFirePressed()  => _stateChart.SendEvent(EVT_FIRE_PRESS);
    private void OnFireReleased() => _stateChart.SendEvent(EVT_FIRE_REL);

    private void OnDashChargesChanged(int current, int max)
        => _stateChart.SetExpressionProperty("can_dash", current > 0);

    // -------- Movement callbacks (state logic)
    private void MvEnterIdle()                   => _movement.Stop(this);
    private void MvUpdateIdle(double dt)         { /* noop */ }

    private void MvEnterRun()                    { /* start run anim if you want */ }
    private void MvUpdateRun(double dt)          => _movement.Run(this, _axis, dt);

    private void MvEnterDash()
    {
         GD.Print("[MvEnterDash] entered Dash state");
        if (!_dashRequested || !_dash.CanStartDash())
        {
            GD.Print("[Player] Dash ignored — not requested or cannot start");
            _stateChart.SendEvent(EVT_DASH_DONE);
            return;
        }

        _dashRequested = false;
        GD.Print("[Player] Starting dash with axis: ", _axis);
        _dash.StartDash(this, _axis, () => _stateChart.SendEvent(EVT_DASH_DONE));

        if (!_dash.IsDashing)
        {
            GD.Print("[Player] Dash didn't start (no direction/charges) — bouncing");
            _stateChart.SendEvent(EVT_DASH_DONE);
        }
    }

    private void MvUpdateDash(double dt) { /* dash moved centrally in _PhysicsProcess */ }

    private void MvExitDash() => _dash.EndDash(this);

    // -------- Combat callbacks
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

    // -------- helper
    private void ConnectState(string nodePath, System.Action enter = null,
                              System.Action<double> physics = null,
                              System.Action exit = null)
    {
        var st = StateChartState.Of(GetNode(nodePath));
        if (enter   != null) st.Connect(StateChartState.SignalName.StateEntered,           Callable.From(enter));
        if (physics != null) st.Connect(StateChartState.SignalName.StatePhysicsProcessing, Callable.From<double>(physics));
        if (exit    != null) st.Connect(StateChartState.SignalName.StateExited,            Callable.From(exit));
    }
>>>>>>> Stashed changes
}
