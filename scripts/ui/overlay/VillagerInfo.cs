using Godot;
using System;

public partial class VillagerInfo : Control
{
	const string FARMER_HAT_RESPATH = "res://assets/sprites/character/hats/straw_hat.png";
	const string SOLDIER_HAT_RESPATH = "res://assets/sprites/character/hats/bucket_helmet.png";
	const string WOODCUTTER_HAT_RESPATH = "res://assets/sprites/character/hats/ushanka.png";
	const string MINER_HAT_RESPATH = "res://assets/sprites/character/hats/mining_hat.png";
	
	
	Texture2D _villagerHatTexture;
	public Texture2D villagerHatTexture => _villagerHatTexture;
    
	Texture2D _villagerHeadTexture;
	public Texture2D villagerHeadTexture => _villagerHeadTexture;
    

	public void InitializeVillagerInfo(VillagerRawData data)
	{
		//Hat
		ChangeHat(data.currentOccupation);
		
		_villagerHeadTexture = VillagerManager.villagerManagerInstance.GetTextureByType(
			data.villagerType, BodyPartTextureType.Head);
	}

	public void ChangeHat(VillagerOccupation occupation)
	{
		if (occupation == VillagerOccupation.Builder)
		{
			_villagerHatTexture = new Texture2D();
			return;
		}
		
		string hatResourceString = occupation switch
		{
			VillagerOccupation.Farmer => FARMER_HAT_RESPATH,
			VillagerOccupation.Soldier => SOLDIER_HAT_RESPATH,
			VillagerOccupation.Woodcutter => WOODCUTTER_HAT_RESPATH,
			VillagerOccupation.Miner => MINER_HAT_RESPATH,
			_ => throw new ArgumentOutOfRangeException(nameof(occupation), occupation, null)
		};

		_villagerHatTexture = ResourceLoader.Load<Texture2D>(hatResourceString);
	}
}
