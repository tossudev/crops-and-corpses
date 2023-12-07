using Godot;
using System;

public partial class BridgeQuest : Node2D
{
	[Export] private Node2D _bridge;
	private CanvasLayer _canvasLayer;
	private bool _playerInArea;
	private bool _questFinished;
	private Item _log;
	private const int WOOD_NEEDED = 50;

	public override void _Ready()
	{
		_log = (Item)ResourceLoader.Load("res://assets/resources/game_items/resource_items_0_to_99/0_log.tres");
		_canvasLayer = GetNode<CanvasLayer>("%BridgeQuestUI");
	}

	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton button && _playerInArea)
		{
			_canvasLayer.Visible = true;
		}
	}
	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			_playerInArea = true;

			if (StorageData.ExistsInInventoryOrHotbar(_log.ID, WOOD_NEEDED))
			{
				GetNode<Button>("%FinishQuestBtn").Disabled = false;
				GetNode<Label>("%FinishBtnLabel").Visible = true;
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

	private async void RemoveLogsFromInventory()
	{
		RawInventoryItem _logsForQuest = new RawInventoryItem(_log.ID, _log.Name, WOOD_NEEDED, _log.StackSize);
		await PlayerInventoryController.RemoveItemFromInventory(_logsForQuest);
	}

	private void OnFinishQuestButtonPressed()
	{
		if (_questFinished)
		{
			RemoveLogsFromInventory();

			if (_bridge != null) _bridge.Visible = true;
			TownManager.ApplyUnlock(TownUnlock.RUINS_UNLOCK);
			_canvasLayer.Visible = false;

			GetNode<StaticBody2D>("%Barrier").QueueFree();
			GetNode<Button>("%FinishQuestBtn").Visible = false;
			GetNode<Label>("%FinishBtnLabel").Visible = false;
			GetNode<Label>("%BridgeLabel").Text = "Nice job, you finished the bridge! I wonder where it leads to...";
		}
	}
}
