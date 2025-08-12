using Godot;

public partial class PlayerShoot : Node
{
    [Export] public NodePath ShootSfxPath;
    [Export] public bool Automatic = false; // hold-to-fire
    [Export] public float FireRate = 6f;

    private AudioStreamPlayer _sfx;
    private AudioStreamPlayer2D _sfx2D;
    private float _cd;

    public override void _Ready()
    {
        _sfx  = !ShootSfxPath.IsEmpty ? GetNodeOrNull<AudioStreamPlayer>(ShootSfxPath) : GetNodeOrNull<AudioStreamPlayer>("AudioStreamPlayer");
        _sfx2D= !ShootSfxPath.IsEmpty ? GetNodeOrNull<AudioStreamPlayer2D>(ShootSfxPath) : GetNodeOrNull<AudioStreamPlayer2D>("GunSound");
        if (_sfx == null && _sfx2D == null)
            GD.PushWarning("[PlayerShoot] No AudioStreamPlayer(2D) assigned.");
    }

    public void TickAim(double dt) { }

    public void BeginBurst()
    {
        _cd = 0f;           // allow immediate shot on enter
        PlayShot();
    }

    public void TickShooting(double dt)
    {
        if (!Automatic) return; // single-shot per press; nothing to do per-frame

        _cd -= (float)dt;
        if (_cd <= 0f)
        {
            PlayShot();
            _cd = 1f / Mathf.Max(FireRate, 0.001f);
        }
    }

    public void EndBurst() { }

    private void PlayShot()
    {
        if (_sfx != null)  { if (_sfx.Playing) _sfx.Stop(); _sfx.Play(); }
        else if (_sfx2D != null) { if (_sfx2D.Playing) _sfx2D.Stop(); _sfx2D.Play(); }
    }
}
