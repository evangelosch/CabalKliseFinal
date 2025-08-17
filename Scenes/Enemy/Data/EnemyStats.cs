// EnemyStats.cs
using Godot;

[GlobalClass]
public partial class EnemyStats : Resource
{
    [ExportCategory("Movement")]
    [Export] public float WalkSpeed { get; set; } = 160f;
    [Export] public float PatrolPause { get; set; } = 0.2f; // small stop before aim
    [Export] public float AimTime { get; set; } = 0.35f;    // how long to aim before shooting
    [Export] public int   MinRelocateDist { get; set; } = 120;
    [Export] public int   MaxRelocateDist { get; set; } = 300;

    [ExportCategory("Shoot")]
    [Export] public float FireRate { get; set; } = 6f;      // shots/sec during burst
    [Export] public int   BurstCount { get; set; } = 3;     // shots per burst
    [Export] public float ShotCooldown { get; set; } = 0.1f;
    [Export] public AudioStream ShotSfx { get; set; }

    [ExportCategory("Spawn")]
    [Export] public int   ScreenMargin { get; set; } = 24;  // how far from edges we keep targets
}
