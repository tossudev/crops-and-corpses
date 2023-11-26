using Godot;
using System;
using System.Collections.Generic;

public partial class AllVillagerData : Node
{
	[Export]Texture2D [] _villagerTextures;

	[Export] string [] _villagerNames;

	[Export] string [] _villagerInfos;
	public Texture2D GetTexture(){

		Random random = new Random();
        int randomNumber = random.Next(0, _villagerTextures.Length);
		return _villagerTextures[randomNumber];
	}
	public string GetName(){
		Random random = new Random();
        int randomNumber = random.Next(0, _villagerNames.Length);
		return _villagerNames[randomNumber];
	}

	public string GetInfo(){
		Random random = new Random();
        int randomNumber = random.Next(0, _villagerInfos.Length);
		return _villagerInfos[randomNumber];
	}

	public Texture2D GetTextureByType(VillagerType type, BodyPartTextureType part)
	{
		//Todo: this

		return GetTexture();
	}
}
