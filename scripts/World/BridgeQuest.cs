using Godot;
using System;

public partial class BridgeQuest : Node2D
{
	[Export] private Node2D _bridge;
	private CanvasLayer _canvasLayer;
	private bool _playerInArea;
	private bool _questFinished;
	private const int WOOD_NEEDED = 50;

	public override void _Ready()
	{
		_canvasLayer = GetNode<CanvasLayer>("%BridgeQuestUI");
	}

	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button && _playerInArea)
		{
			_canvasLayer.Visible = true;
		}
	}
	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			_playerInArea = true;

			if (StorageData.ExistsInStorage(SaveData.organizedPlayerInventory, 0, WOOD_NEEDED))
			{
				GetNode<Button>("%FinishQuestBtn").Disabled = false;	
				_questFinished = true;			
			}
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			_playerInArea = false;
		}
	}

	private void OnCloseButtonPressed()
	{
		_canvasLayer.Visible = false;
	}

	private void OnFinishQuestButtonPressed()
	{
		if (_questFinished)
		{
			if (_bridge != null) _bridge.Visible = true;
			SceneInfo.forestBuildABridgeOpen = true;
			GetNode<StaticBody2D>("%Barrier").QueueFree();
			_canvasLayer.Visible = false;
			GetNode<Label>("%BridgeLabel").Text = "Nice job, you finished the bridge! I wonder where it leads to...";
		}		
	}
}
