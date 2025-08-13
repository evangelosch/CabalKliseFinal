<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< HEAD
// PlayerHealth.cs
=======
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
// PlayerHealth.cs
>>>>>>> Stashed changes
=======
// PlayerHealth.cs
>>>>>>> Stashed changes
using Godot;

public partial class PlayerHealth : Node
{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
    [Export] public int MaxHp = 3;
    public int Hp { get; private set; }

    public override void _Ready() => Hp = MaxHp;

    public void Damage(int amount)
    {
        Hp = Mathf.Max(0, Hp - amount);
        if (Hp == 0) GD.Print("[PlayerHealth] Dead");
<<<<<<< Updated upstream
=======
    [Export] public PlayerStats Stats;
    public int Current { get; private set; }
=======
    [Export] public int MaxHp = 3;
    public int Hp { get; private set; }
>>>>>>> Stashed changes

    public override void _Ready() => Hp = MaxHp;

    public void Damage(int amount)
    {
<<<<<<< Updated upstream
        Current = Mathf.Max(Current - amount, 0);
        if (Current <= 0) EmitSignal(SignalName.Died);
    }

    public void Heal(int amount)
    {
        Current = Mathf.Min(Current + amount, Stats?.MaxHealth ?? 5);
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
        Hp = Mathf.Max(0, Hp - amount);
        if (Hp == 0) GD.Print("[PlayerHealth] Dead");
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    }
}
