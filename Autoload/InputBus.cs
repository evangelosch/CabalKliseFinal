// InputBus.cs
using Godot;

public partial class InputBus : Node
{
    [Signal] public delegate void MoveAxisEventHandler(float axis);
    [Signal] public delegate void DashPressedEventHandler();
    [Signal] public delegate void FirePressedEventHandler();
    [Signal] public delegate void FireReleasedEventHandler();

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
    }
}
