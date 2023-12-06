using Godot;
using System;

public partial class TownHallControl : Node2D
{
	void OnTownHallInput(Node viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is not InputEventMouseButton { Pressed: true } mouseEvent) return;
		
		if (mouseEvent.ButtonIndex == MouseButton.Left)
		{
			var questManager = GetNode<QuestManager>("/root/QuestManager");

			TownHallMenu.menuInstance.OpenMainPanel();

			questManager.townHallClicked();

		}
	}

	public override void _Ready()
	{
		base._Ready();

		TownManager.SetTownHallPosition(GlobalPosition - new Vector2(-10, -10));
	}
}
