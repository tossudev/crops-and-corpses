using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AllVillagerData : Node
{
	const string FARMER_HAT_RESPATH = "res://assets/sprites/character/hats/straw_hat_for_villager.png";
	const string SOLDIER_HAT_RESPATH = "res://assets/sprites/character/hats/helmet_hat_for_villager.png";
	const string WOODCUTTER_HAT_RESPATH = "res://assets/sprites/character/hats/ushanka_hat_for_villager.png";
	const string MINER_HAT_RESPATH = "res://assets/sprites/character/hats/mining_hat_for_villager.png";

	const string FEMALE1_HEAD_RESPATH = "res://assets/sprites/character/npc1/SVG/head.svg";
	const string FEMALE2_HEAD_RESPATH = "res://assets/sprites/character/npc2/SVG/head.svg";
	const string FEMALE3_HEAD_RESPATH = "res://assets/sprites/character/npc3/SVG/head.svg";

	const string FEMALE1_BODY_RESPATH = "res://assets/sprites/character/npc1/SVG/body.svg";
	const string FEMALE2_BODY_RESPATH = "res://assets/sprites/character/npc2/SVG/body.svg";
	const string FEMALE3_BODY_RESPATH = "res://assets/sprites/character/npc3/SVG/body.svg";

	const string FEMALE1_ARM_R_RESPATH = "res://assets/sprites/character/npc1/SVG/right_arm.svg";
	const string FEMALE2_ARM_R_RESPATH = "res://assets/sprites/character/npc2/SVG/right_arm.svg";
	const string FEMALE3_ARM_R_RESPATH = "res://assets/sprites/character/npc3/SVG/right_arm.svg";

	const string FEMALE1_ARM_L_RESPATH = "res://assets/sprites/character/npc1/SVG/left_arm.svg";
	const string FEMALE2_ARM_L_RESPATH = "res://assets/sprites/character/npc2/SVG/left_arm.svg";
	const string FEMALE3_ARM_L_RESPATH = "res://assets/sprites/character/npc3/SVG/left_arm.svg";

	const string FEMALE1_LEG_R_RESPATH = "res://assets/sprites/character/npc1/SVG/right_foot.svg";
	const string FEMALE2_LEG_R_RESPATH = "res://assets/sprites/character/npc2/SVG/right_foot.svg";
	const string FEMALE3_LEG_R_RESPATH = "res://assets/sprites/character/npc3/SVG/right_foot.svg";

	const string FEMALE1_LEG_L_RESPATH = "res://assets/sprites/character/npc1/SVG/left_foot.svg";
	const string FEMALE2_LEG_L_RESPATH = "res://assets/sprites/character/npc2/SVG/left_foot.svg";
	const string FEMALE3_LEG_L_RESPATH = "res://assets/sprites/character/npc3/SVG/left_foot.svg";

	public Dictionary<VillagerType, Dictionary<BodyPartTextureType, Texture2D>> villagerTextures = new ();
	VillagerSkeleton _villagerSkeleton;

	[Export] string [] _villagerFirstNames;
	[Export] string [] _villagerLastNames;

	[Export] string [] _villagerInfos;

	public string GetName(){
		Random random = new Random();
		
        int random1 = random.Next(0, _villagerFirstNames.Length);
        
        int random2 = random.Next(0, _villagerLastNames.Length);
        
		return $"{_villagerFirstNames[random1]} {_villagerLastNames [random2]}";
	}

	public string GetInfo(){
		Random random = new Random();
        int randomNumber = random.Next(0, _villagerInfos.Length);
		return _villagerInfos[randomNumber];
	}

	public override void _Ready()
	{
		base._Ready();
		InitializeVillagerTextures();
	}

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

	public Texture2D GetHatByOccupation(VillagerOccupation occupation)
	{
		if (occupation == VillagerOccupation.Builder)
		{
			return new Texture2D();
		}
		
		string hatResourceString = occupation switch
		{
			VillagerOccupation.Farmer => FARMER_HAT_RESPATH,
			VillagerOccupation.Soldier => SOLDIER_HAT_RESPATH,
			VillagerOccupation.Woodcutter => WOODCUTTER_HAT_RESPATH,
			VillagerOccupation.Miner => MINER_HAT_RESPATH,
			_ => throw new ArgumentOutOfRangeException(nameof(occupation), occupation, null)
		};

		return ResourceLoader.Load<Texture2D>(hatResourceString);
	}

 	public Texture2D GetTextureByType(VillagerType type, BodyPartTextureType part)
	{
		Texture2D tex = new();
		if(villagerTextures.TryGetValue(type, out Dictionary<BodyPartTextureType, Texture2D> textureDictionary))
		{
			if(textureDictionary.TryGetValue(part, out Texture2D texture))
			{
				tex = texture;
			}
		}
		return tex;
	} 
}
