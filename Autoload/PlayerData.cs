using Godot;

public partial class PlayerData : Node
{
    public static PlayerData Instance { get; private set; }

    [Export] public int MaxHpDefault = 6;

    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
    }

    public void NewRun()
    {
        MaxHp = MaxHpDefault;
        CurrentHp = MaxHp;
    }

    public void Damage(int amount)
    {
        CurrentHp = Mathf.Clamp(CurrentHp - amount, 0, MaxHp);
    }
}
