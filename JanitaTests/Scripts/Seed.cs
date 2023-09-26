using Godot;
using System;

public class Seed
{
	public PlantType PlantType { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public float GrowthTime{ get; set; }

	public Texture2D SeedTexture{ get; set; }
	public Texture2D SproutTexture{ get; set; }
	public Texture2D PlantTexture{ get; set; }

	public Seed(PlantType plantType, string name, string description, float growthTime, Texture2D seedTexture, Texture2D sproutTexture, Texture2D plantTexture){

		PlantType = plantType;
		Name = name;
		Description = description;
		GrowthTime = growthTime;
		SeedTexture = seedTexture;
		SproutTexture = sproutTexture;
		PlantTexture = plantTexture;
		

	}

	public Seed(){

	}

}


