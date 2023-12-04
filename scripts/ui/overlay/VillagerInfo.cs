using Godot;
using System;
using System.Collections.Generic;

public partial class VillagerInfo : Control
{
	const string FARMER_HAT_RESPATH = "res://assets/sprites/character/hats/straw_hat.png";
	const string SOLDIER_HAT_RESPATH = "res://assets/sprites/character/hats/bucket_helmet.png";
	const string WOODCUTTER_HAT_RESPATH = "res://assets/sprites/character/hats/ushanka.png";
	const string MINER_HAT_RESPATH = "res://assets/sprites/character/hats/mining_hat.png";

	// HEAD
	const string FEMALE1_HEAD_RESPATH = "res://assets/sprites/character/npc1/SVG/head.svg";
	const string FEMALE2_HEAD_RESPATH = "res://assets/sprites/character/npc2/SVG/head.svg";
	const string FEMALE3_HEAD_RESPATH = "res://assets/sprites/character/npc3/SVG/head.svg";

	// BODY
	const string FEMALE1_BODY_RESPATH = "res://assets/sprites/character/npc1/SVG/body.svg";
	const string FEMALE2_BODY_RESPATH = "res://assets/sprites/character/npc2/SVG/body.svg";
	const string FEMALE3_BODY_RESPATH = "res://assets/sprites/character/npc3/SVG/body.svg";

	// RIGHT ARM
	const string FEMALE1_ARM_R_RESPATH = "res://assets/sprites/character/npc1/SVG/right_arm.svg";
	const string FEMALE2_ARM_R_RESPATH = "res://assets/sprites/character/npc2/SVG/right_arm.svg";
	const string FEMALE3_ARM_R_RESPATH = "res://assets/sprites/character/npc3/SVG/right_arm.svg";

	// LEFT ARM
	const string FEMALE1_ARM_L_RESPATH = "res://assets/sprites/character/npc1/SVG/left_arm.svg";
	const string FEMALE2_ARM_L_RESPATH = "res://assets/sprites/character/npc2/SVG/left_arm.svg";
	const string FEMALE3_ARM_L_RESPATH = "res://assets/sprites/character/npc3/SVG/left_arm.svg";

	// RIGHT LEG
	const string FEMALE1_LEG_R_RESPATH = "res://assets/sprites/character/npc1/SVG/right_foot.svg";
	const string FEMALE2_LEG_R_RESPATH = "res://assets/sprites/character/npc2/SVG/right_foot.svg";
	const string FEMALE3_LEG_R_RESPATH = "res://assets/sprites/character/npc3/SVG/right_foot.svg";

	// LEFT LEG
	const string FEMALE1_LEG_L_RESPATH = "res://assets/sprites/character/npc1/SVG/left_foot.svg";
	const string FEMALE2_LEG_L_RESPATH = "res://assets/sprites/character/npc2/SVG/left_foot.svg";
	const string FEMALE3_LEG_L_RESPATH = "res://assets/sprites/character/npc3/SVG/left_foot.svg";


	public Dictionary<VillagerType, Dictionary<BodyPartTextureType, Texture2D>> villagerTextures;
	Texture2D _villagerHatTexture;
	public Texture2D villagerHatTexture => _villagerHatTexture;
    
	Texture2D _villagerHeadTexture;
	public Texture2D villagerHeadTexture => _villagerHeadTexture;
    

	public void InitializeVillagerTextures()
	{
		villagerTextures[VillagerType.Female1] = new Dictionary<BodyPartTextureType, Texture2D>
		{
			{BodyPartTextureType.Head, (Texture2D)GD.Load(FEMALE1_HEAD_RESPATH)},
			{BodyPartTextureType.Body, (Texture2D)GD.Load(FEMALE1_BODY_RESPATH)},
			{BodyPartTextureType.RightArm, (Texture2D)GD.Load(FEMALE1_ARM_R_RESPATH)},
			{BodyPartTextureType.LeftArm, (Texture2D)GD.Load(FEMALE1_ARM_L_RESPATH)},
			{BodyPartTextureType.RightFoot, (Texture2D)GD.Load(FEMALE1_LEG_R_RESPATH)},
			{BodyPartTextureType.LeftFoot, (Texture2D)GD.Load(FEMALE1_LEG_L_RESPATH)}
		};

		villagerTextures[VillagerType.Female2] = new Dictionary<BodyPartTextureType, Texture2D>
		{
			{BodyPartTextureType.Head, (Texture2D)GD.Load(FEMALE2_HEAD_RESPATH)},
			{BodyPartTextureType.Body, (Texture2D)GD.Load(FEMALE2_BODY_RESPATH)},
			{BodyPartTextureType.RightArm, (Texture2D)GD.Load(FEMALE2_ARM_R_RESPATH)},
			{BodyPartTextureType.LeftArm, (Texture2D)GD.Load(FEMALE2_ARM_L_RESPATH)},
			{BodyPartTextureType.RightFoot, (Texture2D)GD.Load(FEMALE2_LEG_R_RESPATH)},
			{BodyPartTextureType.LeftFoot, (Texture2D)GD.Load(FEMALE2_LEG_L_RESPATH)}
		};

		villagerTextures[VillagerType.Female3] = new Dictionary<BodyPartTextureType, Texture2D>
		{
			{BodyPartTextureType.Head, (Texture2D)GD.Load(FEMALE3_HEAD_RESPATH)},
			{BodyPartTextureType.Body, (Texture2D)GD.Load(FEMALE3_BODY_RESPATH)},
			{BodyPartTextureType.RightArm, (Texture2D)GD.Load(FEMALE3_ARM_R_RESPATH)},
			{BodyPartTextureType.LeftArm, (Texture2D)GD.Load(FEMALE3_ARM_L_RESPATH)},
			{BodyPartTextureType.RightFoot, (Texture2D)GD.Load(FEMALE3_LEG_R_RESPATH)},
			{BodyPartTextureType.LeftFoot, (Texture2D)GD.Load(FEMALE3_LEG_L_RESPATH)}
		};
	}

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
