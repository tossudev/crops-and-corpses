using Godot;
using System;
using System.Collections.Generic;

public partial class FarmManager : Node
{
	List<RawInventoryItem> items = new List<RawInventoryItem>();
	public static FarmManager instance;

	[Export] PackedScene [] _plantPrefabs;

	List<Plant> _plants = new List<Plant>();

	List <Plant> _allPlantedPlants = new List<Plant>();
	bool _isWaterCanEquipped, _isBugSprayEquipped;

	public override void _Ready()
	{

		if(instance==null)instance=this;else QueueFree();
		InitializePlants();
		_isWaterCanEquipped = _isBugSprayEquipped=true;
		 

		/*Item asd = ResourceLoader.Load("res://assets/resources/game_items/7_tomato_seed.tres") as Item;
		Item asd1 = ResourceLoader.Load("res://assets/resources/game_items/8_potato_seed.tres") as Item;
		Item asd2 = ResourceLoader.Load("res://assets/resources/game_items/9_cabbage_seed.tres") as Item;

		RawInventoryItem asd3 = new RawInventoryItem(asd.ID, asd.Name, 10, asd.StackSize);
		RawInventoryItem asd4 = new RawInventoryItem(asd1.ID, asd1.Name, 10, asd1.StackSize);
		RawInventoryItem asd5 = new RawInventoryItem(asd2.ID, asd2.Name, 10, asd2.StackSize);
		items.Add(asd3);
		items.Add(asd4);
		items.Add(asd5);
		for(int i=0; i<items.Count; i++){
			PlayerInventoryController.AddItem(items[i]);
		}*/
		

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

	public Plant GetPlant(string seedName){
		foreach(Plant _plant in _plants){
			if(_plant.seedName.ToLower() == seedName.ToLower())
			{
				Plant _newPlant = _plant.Duplicate() as Plant;
				return _newPlant;
			}
		}

		return null;
	}

	public List<Plant> GetPlantedPlants(){
		return _allPlantedPlants;
	}
	public void AddPlantedPlant(Plant plant){
		_allPlantedPlants.Add(plant);
		GD.Print(_allPlantedPlants.Count);
	}
	public void RemovePlantedPlant(Plant plant){
		_allPlantedPlants.Remove(plant);
		GD.Print(_allPlantedPlants.Count);
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
