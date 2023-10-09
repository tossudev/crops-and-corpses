using Godot;
using System;
using System.Collections.Generic;


public partial class FieldHandler : Node2D
{
	[Export] PlantOptionButton oBTest;

	[Export] CollisionObject2D _col;
	int _currentPlants=0;
	int _maxPlantSlots=1;
	Plant _plant = null;
	[Export] NodePath  _nodePath;


	bool _isPlayerNearby=false;
	public override void _Ready() {
		_col.InputEvent +=InteractWithField;
	}

	public void SetPlant(string newPlant){
		_plant = FarmManager.instance.GetPlant(newPlant);
		
	}

	void InteractWithField(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button && _isPlayerNearby)
		{	
			if(_plant==null) SetPlant("Potato");
			if(_plant!=null && button.IsPressed() && _currentPlants < _maxPlantSlots){
				GD.Print("Planted a "+_plant.seedName);
				PlantPlant();
			}
		}
		
	}
	void PlantPlant(){
		if (_plant!= null)
        {
			TextureRect plantTexture =  GetNode<TextureRect>(_nodePath);
			plantTexture.AddChild(_plant);
			_plant.myField = this;
			plantTexture.Visible=true;
			_currentPlants++;
        }
		string _plantinfo = _plant.plantName.ToString();
		_plant = FarmManager.instance.GetPlant(_plantinfo);
		FarmManager.instance.AddPlantedPlant(_plant);
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
