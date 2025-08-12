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
        float a = 0f;
        if (Input.IsActionPressed("move_left"))  a -= 1f;
        if (Input.IsActionPressed("move_right")) a += 1f;
        a = Mathf.Clamp(a, -1f, 1f);

        if (!Mathf.IsEqualApprox(a, _axis))
        {
            _axis = a;
            EmitSignal(SignalName.MoveAxis, _axis);
        }
    }

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("dash"))  EmitSignal(SignalName.DashPressed);
        if (e.IsActionPressed("fire"))  EmitSignal(SignalName.FirePressed);
        if (e.IsActionReleased("fire")) EmitSignal(SignalName.FireReleased);
    }
}
