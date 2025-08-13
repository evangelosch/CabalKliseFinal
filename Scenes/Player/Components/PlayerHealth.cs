using Godot;

public partial class PlayerHealth : Node
{
    [Export] public PlayerStats Stats;
    public int Current { get; private set; }

    [Signal] public delegate void DiedEventHandler();

    public override void _Ready()
    {
        if (Stats == null) GD.PushWarning("PlayerHealth: Stats not set.");
        Current = Stats?.MaxHealth ?? 5;
    }

    public void Damage(int amount)
    {
        Current = Mathf.Max(Current - amount, 0);
        if (Current <= 0) EmitSignal(SignalName.Died);
    }

    public void Heal(int amount)
    {
        Current = Mathf.Min(Current + amount, Stats?.MaxHealth ?? 5);
    }
}
