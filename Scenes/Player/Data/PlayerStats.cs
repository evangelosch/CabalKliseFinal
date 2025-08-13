<<<<<<< HEAD
// PlayerStats.cs
=======
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
using Godot;

[GlobalClass]
public partial class PlayerStats : Resource
{
<<<<<<< HEAD
    [ExportCategory("Movement")]
    [Export] public float MoveSpeed    { get; set; } = 220f;

    [ExportCategory("Dash")]
    [Export] public float DashDistance { get; set; } = 160f; // px
    [Export] public float DashSpeed    { get; set; } = 900f; // px/s
    [Export] public float DashCooldown { get; set; } = 2f; // seconds per charge
=======
    [Export] public int MaxHealth = 5;
    [Export] public float MoveSpeed = 500f;
    [Export] public float DashDistance = 120f;
    [Export] public float DashSpeed = 1200f;
    [Export] public float DashCooldown = 0.8f;
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
}
