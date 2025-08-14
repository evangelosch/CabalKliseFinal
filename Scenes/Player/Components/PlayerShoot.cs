using Godot;

public partial class PlayerShoot : Node
{
    // --- Crosshair ---
    [Export] public NodePath CrosshairPath;
    private Crosshair _crosshair;

    // --- Audio ---
    [Export] public AudioStream ShotSfx;                 // assign in Inspector
    [Export(PropertyHint.Range, "-40,6,0.5")] public float ShotVolumeDb = -2f;
    [Export(PropertyHint.Range, "0,0.5,0.01")] public float PitchVariance = 0.07f;
    [Export] public string AudioBus = "SFX";             // optional bus name

    // --- Firing control ---
    [Export(PropertyHint.Range, "1,30,0.1")] public float FireRate = 8f; // shots/sec for auto-fire
    [Export] public bool AutoFire = true;                                 // hold to spray; if false → single shot
    [Export(PropertyHint.Range, "0,5,0.01")] public float ShotCooldown = 1.0f; // HARD cooldown between shots (sec)

    public float Cooldown { get; private set; } = 0f;    // chart reads this as shoot_cd
    public bool IsReady => Cooldown <= 0f;

    private AudioStreamPlayer2D _shotPlayer;
    private float _shotTimer = 0f; // for AutoFire pacing

    public override void _Ready()
    {
        if (CrosshairPath != null && !CrosshairPath.IsEmpty)
            _crosshair = GetNode<Crosshair>(CrosshairPath);

        // Start hidden; Player shows it when entering Aim (toggle in your state callbacks)
        SetCrosshairVisible(true);

        _shotPlayer = new AudioStreamPlayer2D { Bus = AudioBus };
        AddChild(_shotPlayer);
        GD.Randomize();
    }

    public void SetCrosshairVisible(bool v)
    {
        if (IsInstanceValid(_crosshair)) _crosshair.Visible = v;
    }

    public void Equip(Weapon w) { /* later */ }

    public void TickAim(double dt) { /* crosshair moves itself */ }

    public void TickCooldown(double dt)
    {
        if (Cooldown > 0f) Cooldown = Mathf.Max(0f, Cooldown - (float)dt);
    }

    // Called when Shoot state is entered
    public void BeginBurst()
    {
        _shotTimer = 0f;         // allow immediate attempt
        TryShoot();              // single shot on press
    }

    // Called each frame while in Shoot
    public void TickShooting(double dt)
    {
        TickCooldown(dt);

        if (!AutoFire) return;

        // Pace auto-fire by FireRate, but also gate with Cooldown
        float interval = 1f / Mathf.Max(0.01f, FireRate);
        _shotTimer -= (float)dt;

        if (_shotTimer <= 0f && IsReady)
        {
            TryShoot();
            _shotTimer = interval; // next time we're allowed to try again
        }
    }

    // Called when exiting Shoot
    public void EndBurst()
    {
        _shotTimer = 0f; // reset loop
    }

    // Public: attempt to fire once, respecting cooldown
    public bool TryShoot()
    {
        if (!IsReady) return false;
        FireOnce();
        return true;
    }

    // --- Actual shot ---
    private void FireOnce()
    {
        // TODO: spawn bullet toward crosshair here if you like
        PlayShotSfx();

        // Start hard cooldown. Your chart guard Aim->Shoot should check shoot_cd <= 0
        Cooldown = ShotCooldown;
    }

    private void PlayShotSfx()
    {
        if (ShotSfx == null) return;

        _shotPlayer.Stream = ShotSfx;
        _shotPlayer.VolumeDb = ShotVolumeDb;
        _shotPlayer.PitchScale = 1f + ((GD.Randf() * 2f - 1f) * PitchVariance);

        // Place near the nearest Node2D ancestor (usually the Player). No risky casts.
        Node n = this;
        while (n != null && n is not Node2D) n = n.GetParent();
        if (n is Node2D host2D) _shotPlayer.GlobalPosition = host2D.GlobalPosition;

        _shotPlayer.Play();
    }
}
