using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }
	private PackedScene _gameScene = GD.Load<PackedScene>("res://Scenes/Game/Game.tscn");

	public override void _Ready()
	{
		Instance = this;
		ProcessMode = ProcessModeEnum.Always;
	}

	public static void LoadMain()						
	{
		Instance.GetTree().ChangeSceneToPacked(Instance._gameScene);	
	}

}
