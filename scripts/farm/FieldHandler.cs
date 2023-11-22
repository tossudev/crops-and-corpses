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
		if(@event is InputEventMouseButton button && _isPlayerNearby && button.IsPressed() && button.ButtonIndex == MouseButton.Left)
		{
			if (!PlayerInventoryController.isItemSelected || PlayerInventoryController.heldItem == null) return;

			SetPlant(
				PlayerInventoryController.isItemSelected
					? PlayerInventoryController.selectedItem.name
					: PlayerInventoryController.heldItem.name);

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
		_plant.myField = this;
		plantTexture.AddChild(_plant);
		
		plantTexture.Visible=true;
		_currentPlants++;
	}

	public void LoadPlant(string seedName, double growthTime, float savedTime, bool isGrowing, bool isTendedTo, bool isDead)
	{
		SetPlant(seedName);

        TextureRect plantTexture = GetNode<TextureRect>(_nodePath);
        _plant.myField = this;
        plantTexture.AddChild(_plant);

        plantTexture.Visible = true;
        _currentPlants++;

		if (!isGrowing)
			return;

        GlobalTime globaltime = GetNode<GlobalTime>("/root/GlobalTime");

		double difference = globaltime.GetTime() - savedTime;

		double currentGrowthTime = growthTime + difference;

        if (((!isTendedTo && difference > _plant.growthCycleLength) || isDead) && growthTime < _plant.growthCycleLength * _plant.maxCycles)
        {
			_plant.Die();
			return;
        }

        _plant.currentGrowthTime = currentGrowthTime;

		_plant.LoadPlant(currentGrowthTime);
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
