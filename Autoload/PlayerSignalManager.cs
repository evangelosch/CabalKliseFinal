using Godot;
using System;

public partial class PlayerSignalManager : Node
{
	public static PlayerSignalManager Instance { get; private set; }
	[Signal] public delegate void OnPlayerDiedEventHandler();
	[Signal] public delegate void OnPlayerHealthBelowHalfEventHandler(int hp, int maxHp);
	
	public override void _Ready()
	{
		Instance = this;
		ProcessMode = ProcessModeEnum.Always;
	}
	public static void EmitOnPlayerDied() => Instance.EmitSignal(SignalName.OnPlayerDied);
    public static void EmitOnPlayerHealthBelowHalf() => Instance.EmitSignal(SignalName.OnPlayerHealthBelowHalf);
}
