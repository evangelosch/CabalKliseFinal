using Godot;

public partial class PlayerHealth : Node
{
    [Export] public int MaxHp = 3;
    public int Hp { get; private set; }

    public override void _Ready() => Hp = MaxHp;

    public void Damage(int amount)
    {
        Hp = Mathf.Max(0, Hp - amount);
        if (Hp == 0) GD.Print("[PlayerHealth] Dead");
    }
}
