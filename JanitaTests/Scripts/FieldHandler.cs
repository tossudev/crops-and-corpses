using Godot;
using System;
using System.Collections.Generic;


public partial class FieldHandler : Node
{
	[Export] CollisionObject2D _col;

	int _currentPlants=0;
	int _maxPlantSlots=3;

	Plant _plant = null;
	List<Plant> _plants = new List<Plant>();

	[Export] NodePath [] _nodePath = new NodePath[3];

	public override void _Ready() {
		_col.InputEvent +=InteractWithField;
	}

	public void SetPlant(string newPlant){
		_plant = FarmManager.instance.GetPlant(newPlant);
		GD.Print("Chose plant: "+_plant.plantName);
		
	}

	void InteractWithField(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button)
		{
			if(_plant.seedName!=null && button.IsPressed() && _currentPlants < _maxPlantSlots){
				GD.Print("Planted a "+_plant.seedName);
				GD.Print(_plant.sproutTexture);
				PlantPlant();
			}
		}
		

	}

	void PlantPlant(){
		if (_plant!= null)
        {
			int _index = FirstAvailableCropSlot();
			_plants.Add(_plant);
			TextureRect plantTexture =  GetNode<TextureRect>(_nodePath[_index]);
			plantTexture.AddChild(_plant);
			_plant.myField = this;
			_plant.myFieldIndex = _index;
			plantTexture.Visible=true;
			_currentPlants++;
        }
		string _plantinfo = _plant.plantName.ToString();
		_plant = FarmManager.instance.GetPlant(_plantinfo);
	}
	
	int FirstAvailableCropSlot(){

		for(int i=0; i<_nodePath.Length; i++){
			if(GetNode<TextureRect>(_nodePath[i]).GetChildCount()==0){
				return i;
			}
		}
		return -1;
	}

	public void RemovePlant(){
		_currentPlants--;
	}
	
}
