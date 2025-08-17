// EnemySpawner.cs
using Godot;

public partial class EnemySpawner : Node
{
    [Export] public PackedScene EnemyScene;

    // Lanes (Y positions) for the two enemies
    [Export] public float LaneY1 = 120f;
    [Export] public float LaneY2 = 160f;

    // Tunables for their paths (pixels)
    [Export] public float A_WalkRight = 300f;
    [Export] public float A_WalkLeft  = 220f;

    [Export] public float B_WalkLeft  = 280f;
    [Export] public float B_WalkRight = 180f;
    [Export] public float B_DashDist  = 160f;

    public override void _Ready()
    {
        // Defer so the scene finishes building before we add children
        CallDeferred(nameof(SpawnTwo));
    }

    private void SpawnTwo()
    {
        if (EnemyScene == null)
        {
            GD.PushError("[EnemySpawner] Please assign EnemyScene (drag Enemy.tscn here).");
            return;
        }

        // --- Enemy A: classic patrol & shoot loop ---
        var enemyA = EnemyScene.Instantiate<Enemy>();
        GetTree().CurrentScene.AddChild(enemyA);
        enemyA.Name = "Enemy_A";

        enemyA
            .SpawnLeftTop(LaneY1, margin: 8f)
            .WalkRight(A_WalkRight)
            .StopFor(0.25f)
            .ShootBurst(3, fireRate: 7f)
            .WalkLeft(A_WalkLeft)
            .StopFor(0.15f)
            .ShootBurst(2, fireRate: 8f)
            .Loop(true)
            .StartScript();

        // --- Enemy B: shoots, relocates, and dashes once in a while ---
        var enemyB = EnemyScene.Instantiate<Enemy>();
        GetTree().CurrentScene.AddChild(enemyB);
        enemyB.Name = "Enemy_B";

        enemyB
            .SpawnRightTop(LaneY2, margin: 8f)
            .WalkLeft(B_WalkLeft)
            .StopFor(0.20f)
            .ShootBurst(4, fireRate: 9f)
            .DashRight(B_DashDist)          // quick reposition
            .StopFor(0.10f)
            .ShootBurst(2, fireRate: 10f)
            .WalkRight(B_WalkRight)
            .Loop(true)
            .StartScript();

        GD.Print("[EnemySpawner] Spawned Enemy_A and Enemy_B.");
    }
}
