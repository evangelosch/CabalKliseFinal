using Godot;

public partial class EnemyShoot : Node
{
    [Export] public AudioStream ShotSfx;
    [Export(PropertyHint.Range, "-40,6,0.5")] public float ShotVolumeDb = -4f;
    [Export(PropertyHint.Range, "0,0.5,0.01")] public float PitchVariance = 0.05f;
    [Export] public string AudioBus = "SFX";

    private AudioStreamPlayer2D _player;
    private float _cooldown;
    private float _interval;
    private int _shotsLeft;

    public override void _Ready()
    {
        _player = new AudioStreamPlayer2D { Bus = AudioBus };
        AddChild(_player);
        GD.Randomize();
    }

    public void BeginBurst(int shots, float fireRate)
    {
        _shotsLeft = Mathf.Max(1, shots);
        _interval = 1f / Mathf.Max(0.01f, fireRate);
        _cooldown = 0f; // fire immediately
    }

    // returns true when finished
    public bool TickBurst(Node2D host, double dt)
    {
        if (_shotsLeft <= 0) return true;

        _cooldown -= (float)dt;
        if (_cooldown <= 0f)
        {
            FireOnce(host);
            _shotsLeft--;
            _cooldown = _interval;
        }
        return _shotsLeft <= 0;
    }

    private void FireOnce(Node2D host)
    {
        if (ShotSfx == null) return;
        _player.Stream = ShotSfx;
        _player.VolumeDb = ShotVolumeDb;
        _player.PitchScale = 1f + ((GD.Randf() * 2f - 1f) * PitchVariance);
        _player.GlobalPosition = host.GlobalPosition;
        _player.Play();
        // TODO: spawn projectile toward player later
    }
}
