using Godot;
using GodotStateCharts;

public partial class Player : CharacterBody2D
{
    [Export] public PlayerStats PlayerStats;
    [Export] public Weapon StartingWeapon;

    private StateChart _sc;
    private PlayerMovement _move;
    private PlayerDash _dash;
    private PlayerShoot _shoot;
    private PlayerHealth _health;

    private float _axis;

    public override void _Ready()
    {
        _sc    = StateChart.Of(GetNode("StateChart"));
        _move  = GetNode<PlayerMovement>("Components/PlayerMovement");
        _dash  = GetNode<PlayerDash>("Components/PlayerDash");
        _shoot = GetNode<PlayerShoot>("Components/PlayerShoot");
        _health= GetNode<PlayerHealth>("Components/PlayerHealth");

        // Inject data
        if (PlayerStats != null)
        {
            _move.Speed        = PlayerStats.MoveSpeed;
            _dash.DashDistance = PlayerStats.DashDistance;
            _dash.DashSpeed    = PlayerStats.DashSpeed;
            _dash.Cooldown     = PlayerStats.DashCooldown;
        }

        _move.SpawnAtBottom(this);

        // Init chart props
        _sc.SetExpressionProperty("move_input", 0f);
        _sc.SetExpressionProperty("move_abs", 0f);
        _sc.SetExpressionProperty("can_dash", true);
        _sc.SetExpressionProperty("allow_dash_shoot", true);

        // --- InputBus hookups (no ambiguity; use Connect with generated names) ---
        var ib = GetNode<InputBus>("/root/InputBus");
        ib.Connect(InputBus.SignalName.MoveAxis,     Callable.From<float>(OnMoveAxis));
        ib.Connect(InputBus.SignalName.DashPressed,  Callable.From(() => _sc.SendEvent("dash_press")));
        ib.Connect(InputBus.SignalName.FirePressed,  Callable.From(() => _sc.SendEvent("fire_press")));
        ib.Connect(InputBus.SignalName.FireReleased, Callable.From(() => _sc.SendEvent("fire_release")));

        // --- Hardcoded state paths ---
        // Movement
        ConnectState("StateChart/Root/Movement/Grounded/Idle", enter: MvEnterIdle, physics: MvUpdateIdle);
        ConnectState("StateChart/Root/Movement/Grounded/Run",  enter: MvEnterRun,  physics: MvUpdateRun);
        ConnectState("StateChart/Root/Movement/Dash",          enter: MvEnterDash, physics: MvUpdateDash, exit: MvExitDash);

        // Combat (make sure your node is named 'Shooting' in the chart)
        ConnectState("StateChart/Root/Combat/Aim",             physics: CbUpdateAim);
        ConnectState("StateChart/Root/Combat/Shoot",        enter: CbEnterShooting, physics: CbUpdateShooting, exit: CbExitShooting);
    }

    public override void _PhysicsProcess(double delta)
    {
        // movement happens in state callbacks; we just slide the body here
        MoveAndSlide();
    }

    // ------- Input → chart -------
    private void OnMoveAxis(float axis)
    {
        _axis = axis;
        _sc.SetExpressionProperty("move_input", _axis);
        _sc.SetExpressionProperty("move_abs", Mathf.Abs(_axis));
        _sc.SendEvent("move_update");
    }

    // ------- Movement callbacks -------
    private void MvEnterIdle()                   => _move.Stop(this);
    private void MvUpdateIdle(double dt)         { }

    private void MvEnterRun()                    { /* start run anim if desired */ }
    private void MvUpdateRun(double dt)          => _move.Run(this, _axis, dt);

    private void MvEnterDash()
    {
        _dash.StartDash(this, _axis, () => _sc.SendEvent("dash_done"));
        _sc.SetExpressionProperty("can_dash", false);
    }
    private void MvUpdateDash(double dt)         => _dash.TickDash(this, dt);
    private void MvExitDash()
    {
        _dash.EndDash(this);
        _dash.BeginCooldown(() => _sc.SetExpressionProperty("can_dash", true));
    }

    // ------- Combat callbacks -------
    private void CbUpdateAim(double dt)          => _shoot.TickAim(dt);
    private void CbEnterShooting()               => _shoot.BeginBurst();
    private void CbUpdateShooting(double dt)     => _shoot.TickShooting(dt); // no shoot_done here; release drives exit
    private void CbExitShooting()                => _shoot.EndBurst();

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
