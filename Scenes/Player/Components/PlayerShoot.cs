using Godot;

public partial class PlayerShoot : Node
{
    public float Cooldown { get; private set; } = 0f;

    public void Equip(Weapon w) { /* later */ }

    public void TickAim(double dt) { /* later */ }

    public void TickCooldown(double dt)
    {
        if (Cooldown > 0f) Cooldown = Mathf.Max(0f, Cooldown - (float)dt);
    }

    public void BeginBurst() { /* start shooting */ }

    public void TickShooting(double dt)
    {
        TickCooldown(dt);
        // fire logic, reduce Cooldown, etc.
    }

    public void EndBurst() { /* stop shooting */ }
}
