using Godot;
using System;

public partial class VillagerFaceButton : Button
{
	public ulong id;
	public override void _Ready()
	{
	}

	public void InitButton(Villager_Info info)
	{
		id = info.GetInstanceId();
	}
}
