using Godot;
using System;

public partial class RoomFight1 : Node2D
{
    [Export] public PackedScene EnemyScene;

    public override void _Ready()
    {
        var viewport = GetViewportRect();
        float centerX = viewport.Size.X / 2f;

        // Spawn one enemy at each Marker2D in the scene
        foreach (Node child in GetChildren())
        {
            if (child is Marker2D marker)
            {
                var enemy = (CharacterBody2D)EnemyScene.Instantiate();
                enemy.Position = marker.Position;

                // Set direction based on marker position
                if (enemy is Enemy enemyScript)
                {
                    enemyScript.Direction = marker.Position.X < centerX ? 1 : -1;
                }

                AddChild(enemy);
            }
        }
    }
}
