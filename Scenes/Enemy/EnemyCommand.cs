using Godot;

public enum EnemyCmdType { SetPos, Walk, Wait, Shoot, Dash }

public struct EnemyCommand
{
    public EnemyCmdType Type;
    public float F1, F2, F3; // generic floats for params
    public int I1;

    // Factories
    public static EnemyCommand SetPos(Vector2 pos) => new() { Type = EnemyCmdType.SetPos, F1 = pos.X, F2 = pos.Y };
    // dir: +1 right, -1 left; distance in px
    public static EnemyCommand Walk(float dir, float distance) => new() { Type = EnemyCmdType.Walk, F1 = dir, F2 = distance };
    public static EnemyCommand Wait(float seconds) => new() { Type = EnemyCmdType.Wait, F1 = seconds };
    // shots I1, fireRate F1 (shots/sec)
    public static EnemyCommand Shoot(int shots, float fireRate) => new() { Type = EnemyCmdType.Shoot, I1 = shots, F1 = fireRate };
    // Dash: dir F1 (+1/-1), distance F2, speed F3
    public static EnemyCommand Dash(float dir, float distance, float speed) => new() { Type = EnemyCmdType.Dash, F1 = dir, F2 = distance, F3 = speed };
}
