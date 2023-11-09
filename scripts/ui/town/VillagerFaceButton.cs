using Godot;
using System;

public partial class VillagerFaceButton : Button
{
	Villager _currentResident;
	public ulong id;

	public override void _Pressed()
	{
		base._Pressed();
		
		_currentResident.currentResidence.OpenVillagerDialoguePanel(_currentResident);
	}

	public void InitButton(Villager residingVillager)
	{
		_currentResident = residingVillager;
		id = residingVillager.GetInstanceId();
	}
}
