using Godot;
using System;

public partial class PlayerDash : Node
{
    [Export] public float DashDistance = 200.0f;
    [Export] public float DashDuration = 0.15f;
    [Export] public int MaxDashCharges = 2;
    [Export] public float DashRechargeTime = 1.0f;

    private int dashCharges = 2;
    private float dashRechargeTimer = 0.0f;

    private bool isDashing = false;
    private float dashTimer = 0.0f;
    private Vector2 dashStart;
    private Vector2 dashEnd;

    public override void _Process(double delta)
    {
        var player = GetParent() as CharacterBody2D;
        if (player == null)
            return;

        // Recharge dash charges
        if (dashCharges < MaxDashCharges && !isDashing)
        {
            dashRechargeTimer += (float)delta;
            if (dashRechargeTimer >= DashRechargeTime)
            {
                dashCharges++;
                dashRechargeTimer = 0.0f;
            }
        }

        // Dash input
        if (!isDashing && dashCharges > 0)
        {
            if (Input.IsActionPressed("move_left") && Input.IsActionJustPressed("dash"))
            {
                StartDash(player, -1);
            }
            else if (Input.IsActionPressed("move_right") && Input.IsActionJustPressed("dash"))
            {
                StartDash(player, 1);
            }
        }

        // Dash movement
        if (isDashing)
        {
            dashTimer += (float)delta;
            float t = dashTimer / DashDuration;

            if (t >= 1.0f)
            {
                t = 1.0f;
                isDashing = false;
            }

            player.GlobalPosition = dashStart.Lerp(dashEnd, t);
        }
    }

    private void StartDash(CharacterBody2D player, int direction)
    {
        isDashing = true;
        dashTimer = 0.0f;
        dashCharges--;
        dashRechargeTimer = 0.0f;

        dashStart = player.GlobalPosition;
        dashEnd = dashStart + new Vector2(direction * DashDistance, 0);
    }

    // Optional getters
    public bool IsDashing() => isDashing;
    public int GetDashCharges() => dashCharges;
}
