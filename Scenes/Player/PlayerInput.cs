// PlayerInput.cs
using Godot;

public partial class PlayerInput : Node
{
    [Signal] public delegate void MoveAxisEventHandler(float axis);

    public override void _PhysicsProcess(double delta)
    {
        float a = 0f;
        if (Input.IsActionPressed("move_left"))  a -= 1f;
        if (Input.IsActionPressed("move_right")) a += 1f;
        EmitSignal(SignalName.MoveAxis, Mathf.Clamp(a, -1f, 1f));
    }
}
