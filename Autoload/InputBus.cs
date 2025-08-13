<<<<<<< HEAD
// InputBus.cs
=======
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
using Godot;

public partial class InputBus : Node
{
    [Signal] public delegate void MoveAxisEventHandler(float axis);
    [Signal] public delegate void DashPressedEventHandler();
    [Signal] public delegate void FirePressedEventHandler();
    [Signal] public delegate void FireReleasedEventHandler();

<<<<<<< HEAD
    private float _lastAxis = 0f;

    public override void _PhysicsProcess(double delta)
    {
        // Change these actions to match your InputMap (Project Settings → Input Map)
        float axis = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        if (!Mathf.IsEqualApprox(axis, _lastAxis))
        {
            _lastAxis = axis;
            EmitSignal(SignalName.MoveAxis, axis);
        }

        if (Input.IsActionJustPressed("dash"))
            EmitSignal(SignalName.DashPressed);

        if (Input.IsActionJustPressed("fire"))
            EmitSignal(SignalName.FirePressed);

        if (Input.IsActionJustReleased("fire"))
            EmitSignal(SignalName.FireReleased);
=======
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
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
    }
}
