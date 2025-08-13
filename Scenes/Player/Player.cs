// Player.cs — only the changed/important parts shown
using Godot;
<<<<<<< HEAD
<<<<<<< Updated upstream
using GodotStateCharts; // IMPORTANT: plugin’s C# namespace
=======
using GodotStateCharts;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
using System;
>>>>>>> Stashed changes
=======
using System;
>>>>>>> Stashed changes

public partial class Player : CharacterBody2D
{
    [Export] public PlayerStats PlayerStats;
    [Export] public Weapon StartingWeapon;

    private StateChart _stateChart;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    private PlayerMovement _move;
<<<<<<< HEAD
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
=======
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
    private PlayerMovement _movement;
>>>>>>> Stashed changes
=======
    private PlayerMovement _movement;
>>>>>>> Stashed changes
    private PlayerDash _dash;
    private PlayerShoot _shoot;
    private PlayerHealth _health;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
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
=======
=======
    private bool _dashRequested;
>>>>>>> Stashed changes
    private float _axis;

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

<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // if (StartingWeapon != null)
        //     _shoot.Equip(StartingWeapon);
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea

        if (PlayerStats != null)
        {
            _movement.Speed = PlayerStats.MoveSpeed;
            _dash.SetStats(PlayerStats);
        }

<<<<<<< HEAD
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
=======
        // Init chart expression properties
        _stateChart.SetExpressionProperty("move_input", 0f);
        _stateChart.SetExpressionProperty("move_abs", 0f);
        _stateChart.SetExpressionProperty("allow_dash_shoot", true);
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea

        // Set from dash charges (guard uses this)
        _stateChart.SetExpressionProperty("can_dash", _dash.CanStartDash());

        // Expose shooting cooldown for Aim -> Shoot guard (shoot_cd <= 0)
=======
        if (StartingWeapon != null)
            _shoot.Equip(StartingWeapon);

        _movement.SpawnAtBottom(this);

        _stateChart.SetExpressionProperty("movement_input", 0f);
        _stateChart.SetExpressionProperty("movement_abs", 0f);
        _stateChart.SetExpressionProperty("allow_dash_shoot", true);
        _stateChart.SetExpressionProperty("can_dash", _dash.Charges > 0);
>>>>>>> Stashed changes
=======
        if (StartingWeapon != null)
            _shoot.Equip(StartingWeapon);

        _movement.SpawnAtBottom(this);

        _stateChart.SetExpressionProperty("movement_input", 0f);
        _stateChart.SetExpressionProperty("movement_abs", 0f);
        _stateChart.SetExpressionProperty("allow_dash_shoot", true);
        _stateChart.SetExpressionProperty("can_dash", _dash.Charges > 0);
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< HEAD
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

=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    private void OnFirePressed()  => _stateChart.SendEvent(EVT_FIRE_PRESS);
    private void OnFireReleased() => _stateChart.SendEvent(EVT_FIRE_REL);

    private void OnDashChargesChanged(int current, int max)
        => _stateChart.SetExpressionProperty("can_dash", current > 0);

    // -------- Movement callbacks (state logic)
    private void MvEnterIdle()                   => _movement.Stop(this);
    private void MvUpdateIdle(double dt)         { /* noop */ }

    private void MvEnterRun()                    { /* start run anim if you want */ }
    private void MvUpdateRun(double dt)          => _movement.Run(this, _axis, dt);
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream

    // -------- Combat callbacks
=======
    private void MvEnterRun()                    { /* start run anim if desired */ }
    private void MvUpdateRun(double dt)          => _move.Run(this, _axis, dt); // sets Velocity only
=======
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
    // ------- Combat callbacks (cooldown-aware) -------
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
    // -------- Combat callbacks
>>>>>>> Stashed changes
=======

    // -------- Combat callbacks
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< HEAD
=======
        // FireReleased drives exit to Aim.
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    }

    private void CbExitShooting() => _shoot.EndBurst();

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< HEAD
    // -------- helper
=======
    // ------- Helper to connect a state node directly -------
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
    // -------- helper
>>>>>>> Stashed changes
=======
    // -------- helper
>>>>>>> Stashed changes
    private void ConnectState(string nodePath, System.Action enter = null,
                              System.Action<double> physics = null,
                              System.Action exit = null)
    {
        var st = StateChartState.Of(GetNode(nodePath));
        if (enter   != null) st.Connect(StateChartState.SignalName.StateEntered,           Callable.From(enter));
        if (physics != null) st.Connect(StateChartState.SignalName.StatePhysicsProcessing, Callable.From<double>(physics));
        if (exit    != null) st.Connect(StateChartState.SignalName.StateExited,            Callable.From(exit));
    }
<<<<<<< HEAD
>>>>>>> Stashed changes
=======
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
}
