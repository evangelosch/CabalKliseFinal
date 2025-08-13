using Godot;

public partial class InputBus : Node
{
    [Signal] public delegate void MoveAxisEventHandler(float axis);
    [Signal] public delegate void DashPressedEventHandler();
    [Signal] public delegate void FirePressedEventHandler();
    [Signal] public delegate void FireReleasedEventHandler();

    private float _axis;

    public override void _PhysicsProcess(double delta)
    {
        float axis = 0f;
        if (Input.IsActionPressed("move_left"))  axis -= 1f;
        if (Input.IsActionPressed("move_right")) axis += 1f;
        axis = Mathf.Clamp(axis, -1f, 1f);

        if (!Mathf.IsEqualApprox(axis, _axis))
        {
            _axis = axis;
            EmitSignal(SignalName.MoveAxis, _axis);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("dash"))
        {
            GD.Print("[InputBus] Dash pressed");
            EmitSignal(SignalName.DashPressed);
        }
        
        if (@event.IsActionPressed("fire")) EmitSignal(SignalName.FirePressed);
        if (@event.IsActionReleased("fire")) EmitSignal(SignalName.FireReleased);
    }
}
