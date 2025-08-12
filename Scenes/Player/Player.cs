using Godot;
using GodotStateCharts; // IMPORTANT: plugin’s C# namespace

public partial class Player : CharacterBody2D
{
    private StateChart _sc;            // wrapper for the StateChart node
    private PlayerMovement _move;
    private PlayerInput _input;
    private float _axis;               // -1..1

    public override void _Ready()
    {
        _sc    = StateChart.Of(GetNode("StateChart"));
        _move  = GetNode<PlayerMovement>("PlayerMovement");
        _input = GetNode<PlayerInput>("PlayerInput");

        _move.SpawnAtBottom(this);

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
}
