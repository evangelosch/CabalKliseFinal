using Godot;
using GodotStateCharts;

public partial class Player : CharacterBody2D
{
    [Export] public PlayerStats PlayerStats;
    [Export] public Weapon StartingWeapon;

    private StateChart _stateChart;
    private PlayerMovement _move;
    private PlayerDash _dash;
    private PlayerShoot _shoot;
    private PlayerHealth _health;

    private float _axis;

    public override void _Ready()
    {
        _stateChart = StateChart.Of(GetNode("StateChart"));
        _move   = GetNode<PlayerMovement>("Components/PlayerMovement");
        _dash   = GetNode<PlayerDash>("Components/PlayerDash");
        _shoot  = GetNode<PlayerShoot>("Components/PlayerShoot");
        _health = GetNode<PlayerHealth>("Components/PlayerHealth");

        // Inject data from PlayerStats
        if (PlayerStats != null)
        {
            _move.Speed        = PlayerStats.MoveSpeed;
            _dash.DashDistance = PlayerStats.DashDistance;
            _dash.DashSpeed    = PlayerStats.DashSpeed;
        }

        // if (StartingWeapon != null)
        //     _shoot.Equip(StartingWeapon);

        _move.SpawnAtBottom(this);

        // Init chart expression properties
        _stateChart.SetExpressionProperty("move_input", 0f);
        _stateChart.SetExpressionProperty("move_abs", 0f);
        _stateChart.SetExpressionProperty("allow_dash_shoot", true);

        // Set from dash charges (guard uses this)
        _stateChart.SetExpressionProperty("can_dash", _dash.CanStartDash());

        // Expose shooting cooldown for Aim -> Shoot guard (shoot_cd <= 0)
        _stateChart.SetExpressionProperty("shoot_cd", 0f);

        // --- InputBus hookups ---
        var inputBus = GetNode<InputBus>("/root/InputBus");
        inputBus.Connect(InputBus.SignalName.MoveAxis,     Callable.From<float>(OnMoveAxis));
        inputBus.Connect(InputBus.SignalName.DashPressed,  Callable.From(() => _stateChart.SendEvent("dash_press")));
        inputBus.Connect(InputBus.SignalName.FirePressed,  Callable.From(() => _stateChart.SendEvent("fire_press")));
        inputBus.Connect(InputBus.SignalName.FireReleased, Callable.From(() => _stateChart.SendEvent("fire_release")));

        // Keep can_dash in sync with charges
        _dash.Connect(PlayerDash.SignalName.ChargesChanged,
            Callable.From<int, int>((current, max) =>
            {
                _stateChart.SetExpressionProperty("can_dash", current > 0);
            }));

        // --- State connections ---
        // Movement
        ConnectState("StateChart/Root/Movement/Grounded/Idle", enter: MvEnterIdle, physics: MvUpdateIdle);
        ConnectState("StateChart/Root/Movement/Grounded/Run",  enter: MvEnterRun,  physics: MvUpdateRun);

        // Dash: we start dash on Enter, but physics tick is handled centrally in _PhysicsProcess.
        ConnectState("StateChart/Root/Movement/Dash", enter: MvEnterDash, physics: MvUpdateDash, exit: MvExitDash);

        // Combat
        ConnectState("StateChart/Root/Combat/Aim",   physics: CbUpdateAim);
        ConnectState("StateChart/Root/Combat/Shoot", enter: CbEnterShooting, physics: CbUpdateShooting, exit: CbExitShooting);
    }

   public override void _PhysicsProcess(double delta)
{
    if (_dash.IsDashing)
    {
        _dash.TickDash(this, delta);  // sets velocity
        MoveAndSlide();               // do the move once
        _dash.AfterSlide(this);       // consume actual pixels moved / detect wall
        return;
    }

    // normal movement:
    MoveAndSlide();
}


    // ------- Input → chart -------
    private void OnMoveAxis(float axis)
    {
        _axis = axis;
        _stateChart.SetExpressionProperty("move_input", _axis);
        _stateChart.SetExpressionProperty("move_abs", Mathf.Abs(_axis));
        _stateChart.SendEvent("move_update");
    }

    // ------- Movement callbacks -------
    private void MvEnterIdle()                   => _move.Stop(this);
    private void MvUpdateIdle(double dt)         { /* idle keeps velocity zeroed by _move.Stop */ }

    private void MvEnterRun()                    { /* start run anim if desired */ }
    private void MvUpdateRun(double dt)          => _move.Run(this, _axis, dt); // sets Velocity only

    private void MvEnterDash()
    {
        // StartDash consumes a charge and will emit ChargesChanged (updating can_dash)
        GD.Print("[Player] Starting dash with axis: ", _axis);
        _dash.StartDash(this, _axis, () => _stateChart.SendEvent("dash_done"));
    }

    // Important: leave dash physics empty — _PhysicsProcess drives dash each frame.
    private void MvUpdateDash(double dt) { }

    private void MvExitDash()
    {
        _dash.EndDash(this);
        // No per-dash cooldown — PlayerDash recharges continuously.
        // can_dash flips back via ChargesChanged when a charge refills.
    }

    // ------- Combat callbacks (cooldown-aware) -------
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
        // FireReleased drives exit to Aim.
    }

    private void CbExitShooting() => _shoot.EndBurst();

    // ------- Helper to connect a state node directly -------
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
