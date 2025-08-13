// PlayerStats.cs
using Godot;

[GlobalClass]
public partial class PlayerStats : Resource
{
    [ExportCategory("Movement")]
    [Export] public float MoveSpeed    { get; set; } = 220f;

    [ExportCategory("Dash")]
    [Export] public float DashDistance { get; set; } = 160f; // px
    [Export] public float DashSpeed    { get; set; } = 900f; // px/s
    [Export] public float DashCooldown { get; set; } = 2f; // seconds per charge
}
