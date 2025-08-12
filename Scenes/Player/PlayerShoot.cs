using Godot;
using System;

public partial class PlayerShoot : Node
{
    [Export] public float ShootCooldown = 1.0f;
    private float shootTimer = 0.0f;

    public override void _Process(double delta)
    {
        if (shootTimer > 0)
            shootTimer -= (float)delta;

        if (Input.IsActionJustPressed("shoot") && shootTimer <= 0)
        {
            Shoot();
            shootTimer = ShootCooldown;
        }
    }

    private void Shoot()
    {
        GD.Print("Player shoots!");
        // Assumes Crosshair is a sibling or child of Player

        var gunSound = GetNodeOrNull<AudioStreamPlayer2D>("GunSound");
        gunSound?.Play();
        var crosshair = GetParent().GetNodeOrNull<Node2D>("PlayerCrosshair");
        if (crosshair == null)
        {
            GD.Print("Crosshair not found!");
            return;
        }

        var enemies = (crosshair as dynamic).GetEnemiesUnderCrosshair();
        foreach (var enemy in enemies)
        {
            if (enemy is Node enemyNode && enemyNode.HasMethod("TakeDamage"))
                enemyNode.Call("TakeDamage", 1);
        }
    }
}