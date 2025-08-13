<<<<<<< HEAD
// PlayerShoot.cs
=======
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
using Godot;

public partial class PlayerShoot : Node
{
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

    private AudioStreamPlayer2D _sfx2D;
    private float _cd; // seconds remaining until we can shoot again

    public float Cooldown => _cd; // expose to statechart

    public override void _Ready()
    {
        _sfx2D = !ShootSfxPath.IsEmpty
            ? GetNodeOrNull<AudioStreamPlayer2D>(ShootSfxPath)
            : GetNodeOrNull<AudioStreamPlayer2D>("GunSound");

        if (_sfx2D == null)
            GD.PushWarning("[PlayerShoot] No AudioStreamPlayer2D assigned.");
    }

    public void TickAim(double dt)
    {
        // optional aim visuals here
    }

    /// Call this every frame in Aim to drain cooldown.
    public void TickCooldown(double dt)
    {
        if (_cd > 0f)
            _cd = Mathf.Max(0f, _cd - (float)dt);
    }

    public void BeginBurst()
    {
        // Gate on cooldown: do NOT fire if still cooling down
        if (_cd > 0f) return;

        PlayShot();
        _cd = Mathf.Max(0.001f, DelayBetweenShots);
    }

    public void TickShooting(double dt)
    {
        if (!Automatic) return; // single-shot per press; hold does nothing extra

        // Auto mode: fire again whenever cooldown elapses
        if (_cd > 0f)
        {
            _cd = Mathf.Max(0f, _cd - (float)dt);
            return;
        }

        PlayShot();
        _cd = Mathf.Max(0.001f, DelayBetweenShots);
    }

    public void EndBurst() { /* stop loops if you add any later */ }

    private void PlayShot()
    {
        if (_sfx2D == null) return;
        if (_sfx2D.Playing) _sfx2D.Stop(); // retrigger cleanly
        _sfx2D.Play();
    }
>>>>>>> 4b0dc389250f29563fe0bfcbb72737fa1564e3ea
}
