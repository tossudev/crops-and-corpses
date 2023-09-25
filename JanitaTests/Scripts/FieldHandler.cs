using Godot;
using System;
using System.Collections.Generic;


public partial class FieldHandler : Node
{
	[Export] CollisionObject2D MyCollider;
	Seed seed= new Seed();
	List<Seed> seeds = new List<Seed>();

	int seedSlotsCount=0;
	int maxSeedSlots=3;


	private Timer [] plantTimers = new Timer[3];
	[Export] NodePath [] nodePath = new NodePath[3];

	public override void _Ready() {

		for(int i=0; i<3; i++){
			plantTimers[i] = new Timer();
			AddChild(plantTimers[i]);
		}

		MyCollider.InputEvent +=InteractWithField;
		seed = FarmManager.instance.GetSeed(PlantType.Potato);
	}

	void InteractWithField(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button)
		{
			
			if(button.IsPressed() && seedSlotsCount< maxSeedSlots){
				GD.Print("Planted a "+seed.Name);
				PlantPlant();
			}
		}
		

	}

	void PlantPlant(){
		if (seed!= null)
        {
			int index = seedSlotsCount;
			plantTimers[seedSlotsCount].Timeout += () => EvolvePlant(index);
			seeds.Add(seed);
			TextureButton plantButton =  GetNode<TextureButton>(nodePath[seedSlotsCount]);
            plantButton.TextureNormal = seed.SproutTexture;
			plantButton.Visible=true;
			WaitGrowth();
			seedSlotsCount++;
        }
	}

	void WaitGrowth(){

		 GD.Print("Growing stage started for "+seed.PlantType);
        // Set the timer's wait time (in seconds)
        plantTimers[seedSlotsCount].WaitTime = seed.GrowthTime;
        plantTimers[seedSlotsCount].Start();
	}

	void EvolvePlant(int index){
 		
 		GD.Print("Plant evolved at crop index: "+index);
		plantTimers[index].Stop();
		TextureButton plantButton =  GetNode<TextureButton>(nodePath[index]);
        plantButton.TextureNormal = seed.PlantTexture;
	}

	
}
