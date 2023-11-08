using Godot;
using System;
using System.Collections.Generic;


public partial class FieldHandler : Node2D
{
	[Export] CollisionObject2D _col;
	int _currentPlants=0;
	int _maxPlantSlots=1;
	Plant _plant = null;
	[Export] NodePath  _nodePath;

	bool _isPlayerNearby=false;
	public override void _Ready() {
		_col.InputEvent +=InteractWithField;
	}

	public void SetPlant(string seedName){
		_plant = FarmManager.instance.GetPlant(seedName);
	}

	void InteractWithField(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button && _isPlayerNearby && button.IsPressed())
		{	
			if(PlayerInventoryController.selectedItem!=null ) SetPlant(PlayerInventoryController.selectedItem.name);
			if(_plant!=null && _currentPlants < _maxPlantSlots){
				GD.Print("Planted a "+_plant.seedName);
				PlantPlant();
			}
		}
		
	}
	async void PlantPlant(){
		
		await PlayerInventoryController.RemoveItemFromInventory(new RawInventoryItem(
			PlayerInventoryController.selectedItem.id,
			PlayerInventoryController.selectedItem.name,
			1,
			PlayerInventoryController.selectedItem.stackSize));
		
		TextureRect plantTexture =  GetNode<TextureRect>(_nodePath);
		plantTexture.AddChild(_plant);
		_plant.myField = this;
		plantTexture.Visible=true;
		_currentPlants++;
	}
	
	public void RemovePlant(){
		_currentPlants--;
		FarmManager.instance.RemovePlantedPlant(_plant);
	}

	private void OnInteractable(Area2D body)
	{
		_isPlayerNearby=true;
	}

	private void OnNonInteractable(Area2D body)
	{
		_isPlayerNearby=false;
	}
	
}
