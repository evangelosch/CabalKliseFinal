using Godot;

[GlobalClass]
public partial class Weapon : Resource
{
    [Export] public int BulletsPerShot = 1;
    [Export] public float SpreadDegrees = 0f;
}
