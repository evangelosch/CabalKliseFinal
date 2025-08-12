using Godot;

[GlobalClass]
public partial class PlayerStats : Resource
{
    [Export] public int MaxHealth = 5;
    [Export] public float MoveSpeed = 500f;
    [Export] public float DashDistance = 120f;
    [Export] public float DashSpeed = 1200f;
    [Export] public float DashCooldown = 0.8f;
}
