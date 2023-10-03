using Godot;
using System;
using System.Collections.Generic;

public partial class FarmManager : Node
{
	public static FarmManager instance;

	[Export] PackedScene [] _plantPrefabs;

	List<Plant> _plants = new List<Plant>();


	bool _isWaterCanEquipped, _isBugSprayEquipped;

	public override void _Ready()
	{

		if(instance==null)instance=this;else QueueFree();
		InitializePlants();
		_isWaterCanEquipped = _isBugSprayEquipped=true;

	}

	void InitializePlants()
	{
		for(int i=0; i<_plantPrefabs.Length;i++){

			var scene = ResourceLoader.Load<PackedScene>(_plantPrefabs[i].ResourcePath).Instantiate();
     	  	Plant _newPlant = scene as Plant;   
        	if (_newPlant != null)
        	{
            	_plants.Add(_newPlant);
            	GD.Print(_newPlant.plantName);
        	}
        	else
        	{
            	GD.Print("Failed to cast to Plant: " + _plantPrefabs[i].ResourceName);
        	}
		}
	}

	public Plant GetPlant(string plantName){

		foreach(Plant _plant in _plants){
			if(_plant.plantName.ToString() == plantName)
			{
				Plant _newPlant = _plant.Duplicate() as Plant;
				return _newPlant;
			}
		}

		return null;
	}


	public void EquipWaterCan(bool isEquipped){
		_isWaterCanEquipped = isEquipped;
	}
	public bool IsWaterCanEquipped(){
		return _isWaterCanEquipped;
	}
	public void EquipBugSpray(bool isEquipped){	
		_isBugSprayEquipped = isEquipped;
	}
	public bool IsBugSprayEquipped(){
		return _isBugSprayEquipped;
	}
	
}
public enum PlantType
{	
	Potato, 
	Cabbage, 
	Tomato
}

public enum GrowthState{

	WaitWatering,
	StartGrowth,
	ContinueGrowth,
	IsWilting,
	IsInfested,
	IsHarvestable,
	IsDead

}
