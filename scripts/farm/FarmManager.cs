using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class FarmManager : Node
{
	public static FarmManager instance;

	[Export] PackedScene [] _plantPrefabs;

	List<Plant> _plants = new List<Plant>();

	List <Plant> _allPlantedPlants = new List<Plant>();

	List<RawInventoryItem> _buckets = new List<RawInventoryItem>();
	
	public override void _Ready()
	{

		if(instance==null)instance=this;else QueueFree();
		InitializePlants();
		
		
		/*Item asd = ResourceLoader.Load("res://assets/resources/game_items/0_log.tres") as Item;
		
		
		RawInventoryItem asd1 = new RawInventoryItem(asd.ID, asd.Name, 10, asd.StackSize);
		
		PlayerInventoryController.AddItem(asd1);*/
		
		

	}

	void InitializePlants()
	{
		for(int i=0; i<_plantPrefabs.Length;i++){

			var scene = ResourceLoader.Load<PackedScene>(_plantPrefabs[i].ResourcePath).Instantiate();
     	  	Plant _newPlant = scene as Plant;   
        	if (_newPlant != null)
        	{
            	_plants.Add(_newPlant);
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
	}
	public void RemovePlantedPlant(Plant plant){
		_allPlantedPlants.Remove(plant);
	}

	public bool IsWaterBucketEquipped(){
		if(PlayerInventoryController.selectedItem != null && PlayerInventoryController.selectedItem.name=="Bucket of Water")
		return true;
		else return false;
	}
	public bool IsBugSprayEquipped()
	{
		return PlayerInventoryController.selectedItem != null &&
		       PlayerInventoryController.selectedItem.name == "Bug Spray";
	}
	
	public void EmptyWaterBucket(){
		if(PlayerInventoryController.selectedItem != null && PlayerInventoryController.selectedItem.name=="Bucket of Water"){
			Item emptyB = ResourceLoader.Load("res://assets/resources/game_items/tool_items_350_449/405_bucket.tres") as Item;
			RawInventoryItem bucket = new RawInventoryItem(emptyB.ID, emptyB.Name, 1, emptyB.StackSize);
			PlayerInventoryController.SwapItems(bucket, PlayerInventoryController.selectedItem.indexInOrganizedInventory);		
		}else{
			GD.Print("Water bucket not selected");
		}
	}
	public void FillWaterBucket(){
		if(PlayerInventoryController.selectedItem != null && PlayerInventoryController.selectedItem.name=="Bucket"){
			Item waterB = ResourceLoader.Load("res://assets/resources/game_items/tool_items_350_449/406_bucket_water.tres") as Item;
			RawInventoryItem waterBucket = new RawInventoryItem(waterB.ID, waterB.Name, 1, waterB.StackSize);
			PlayerInventoryController.SwapItems(waterBucket, PlayerInventoryController.selectedItem.indexInOrganizedInventory);		

			int index = PlayerInventoryController.selectedItem.indexInOrganizedInventory;

			//PlayerInventoryController.RemoveItemFromInventory(PlayerInventoryController.selectedItem);


		}else{
			GD.Print("Empty bucket not selected");
		}

	}
}
public enum PlantType
{	
	Potato, 
	Pumpkin, 
	Mushroom,
	Wheat,
	Maize,
	Lupine,
	Poppy
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
