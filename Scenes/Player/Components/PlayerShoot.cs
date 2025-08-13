<<<<<<< Updated upstream
<<<<<<< HEAD
// PlayerShoot.cs
=======
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
// PlayerShoot.cs
>>>>>>> Stashed changes
using Godot;

public partial class PlayerShoot : Node
{
<<<<<<< Updated upstream
<<<<<<< HEAD
    public float Cooldown { get; private set; } = 0f;

    public void Equip(Weapon w) { /* later */ }

    public void TickAim(double dt) { /* aim logic later */ }

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
=======
    [Export] public NodePath ShootSfxPath;
    [Export] public bool Automatic = false;        // hold to auto-fire
    [Export] public float DelayBetweenShots = 1f;  // seconds between shots
=======
    public float Cooldown { get; private set; } = 0f;
>>>>>>> Stashed changes

    public void Equip(Weapon w) { /* later */ }

    public void TickAim(double dt) { /* aim logic later */ }

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

<<<<<<< Updated upstream
    public void EndBurst() { /* stop loops if you add any later */ }

    private void PlayShot()
    {
        if (_sfx2D == null) return;
        if (_sfx2D.Playing) _sfx2D.Stop(); // retrigger cleanly
        _sfx2D.Play();
    }
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
=======
    public void EndBurst() { /* stop shooting */ }
>>>>>>> Stashed changes
}
