<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< HEAD
// InputBus.cs
=======
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
// InputBus.cs
>>>>>>> Stashed changes
=======
// InputBus.cs
>>>>>>> Stashed changes
using Godot;

public partial class InputBus : Node
{
    [Signal] public delegate void MoveAxisEventHandler(float axis);
    [Signal] public delegate void DashPressedEventHandler();
    [Signal] public delegate void FirePressedEventHandler();
    [Signal] public delegate void FireReleasedEventHandler();

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
    private float _axis;
=======
    private float _lastAxis = 0f;
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
        }
        
        if (@event.IsActionPressed("fire")) EmitSignal(SignalName.FirePressed);
        if (@event.IsActionReleased("fire")) EmitSignal(SignalName.FireReleased);
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======

        if (Input.IsActionJustPressed("fire"))
            EmitSignal(SignalName.FirePressed);

        if (Input.IsActionJustReleased("fire"))
            EmitSignal(SignalName.FireReleased);
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    }
}
