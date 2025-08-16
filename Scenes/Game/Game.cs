using Godot;
using System;

public partial class Game : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//PlayerData.Instance.NewRun();
		PlayerSignalManager.Instance.OnPlayerDied += GameOver;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("start_game"))
		{
			GameManager.LoadMain();
		}
		
	}

	private void GameOver()
	{
		// Handle game over logic here, e.g., show a game over screen or reset the game
		GD.Print("Game Over!");
		PlayerSignalManager.EmitOnPlayerDied();
	}

	public override void _ExitTree()
	{
		PlayerSignalManager.Instance.OnPlayerDied -= GameOver;
		// Optionally, you can reset the game state or perform cleanup here
		GD.Print("Exiting Game Scene");
	}
}
