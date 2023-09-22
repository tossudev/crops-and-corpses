using Godot;
using System;
using System.Collections.Generic;

public partial class FarmManager : Node
{
	public static FarmManager instance;

	private Texture2D seedTexture, sproutTexture, plantTexture;

	private List<Seed> seeds = new List<Seed>();
	public override void _Ready()
	{
		if(instance==null)instance=this;else QueueFree();
		InitializeAllSeeds();
	}

	void InitializeAllSeeds(){

		seedTexture = (Texture2D)ResourceLoader.Load("res://JanitaTests/Images/sprouttemp.png");
		sproutTexture = (Texture2D)ResourceLoader.Load("res://JanitaTests/Images/sprouttemp.png");
		plantTexture = (Texture2D)ResourceLoader.Load("res://JanitaTests/Images/wheattemp.png");
		seeds.Add(new Seed(PlantType.Potato, "Potato Seed", "Potatoes for food", 4, seedTexture, sproutTexture, plantTexture));

		seedTexture = (Texture2D)ResourceLoader.Load("res://JanitaTests/Images/sprouttemp.png");
		sproutTexture = (Texture2D)ResourceLoader.Load("res://JanitaTests/Images/sprouttemp.png");
		plantTexture = (Texture2D)ResourceLoader.Load("res://JanitaTests/Images/wheattemp.png");
		seeds.Add(new Seed(PlantType.Cabbage, "Cabbage Seed", "Cabbages for food", 5, seedTexture, sproutTexture, plantTexture));


		for(int i=0; i<seeds.Count;i++){
			GD.Print(seeds[i].PlantType);
		}
		
	}

	public Seed GetSeed(PlantType plantType){

		for(int i=0; i< seeds.Count; i++){
			if(seeds[i].PlantType == plantType){
				return seeds[i];
			}
		}	
		
		return null;
	}

}
