using Godot;
using System;
using System.Collections.Generic;
using static VillagerManager;

public partial class VillagerSkeleton : Skeleton2D
{
	bool _hatMoved = false;
	const string HAT_NODENAME = "%VillagerHat";
	const string HEAD_NODENAME = "%Head";
	const string BODY_NODENAME = "%Body";
	const string ARM_R_NODENAME = "%RightArm";
	const string ARM_L_NODENAME = "%LeftArm";
	const string LEG_R_NODENAME = "%RightLeg";
	const string LEG_L_NODENAME = "%LeftLeg";
	public Sprite2D villagerHat;
	public Sprite2D villagerHead;
	public Sprite2D villagerBody;
	public Sprite2D villagerArmRight;
	public Sprite2D villagerArmLeft;
	public Sprite2D villagerLegRight;
	public Sprite2D villagerLegLeft;
	List<Sprite2D> _allSprites = new();

	public override void _Ready()
	{
		villagerHat = GetNode<Sprite2D>(HAT_NODENAME);
		villagerHead = GetNode<Sprite2D>(HEAD_NODENAME);
		villagerBody = GetNode<Sprite2D>(BODY_NODENAME);
		villagerArmRight = GetNode<Sprite2D>(ARM_R_NODENAME);
		villagerArmLeft = GetNode<Sprite2D>(ARM_L_NODENAME);
		villagerLegRight = GetNode<Sprite2D>(LEG_R_NODENAME);
		villagerLegLeft = GetNode<Sprite2D>(LEG_L_NODENAME);

		_allSprites.AddRange(new[]
			{villagerHat,
			villagerHead,
			villagerBody,
			villagerArmRight,
			villagerArmLeft,
			villagerLegRight,
			villagerLegLeft});
	}

	public void FlipSpritesHorizontal(bool flip)
	{
		_allSprites.ForEach(sprite => sprite.FlipH = flip);
	}

	public void InitializeSkeleton(VillagerRawData data)
	{
		ChangeHat(data.currentOccupation);
		villagerHead.Texture = villagerManagerInstance.GetTextureByType(
			data.villagerType, BodyPartTextureType.Head);

		villagerBody.Texture = villagerManagerInstance.GetTextureByType(
			data.villagerType, BodyPartTextureType.Body);

		villagerArmRight.Texture = villagerManagerInstance.GetTextureByType(
			data.villagerType, BodyPartTextureType.RightArm);

		villagerArmLeft.Texture = villagerManagerInstance.GetTextureByType(
			data.villagerType, BodyPartTextureType.LeftArm);

		villagerLegRight.Texture = villagerManagerInstance.GetTextureByType(
			data.villagerType, BodyPartTextureType.RightFoot);

		villagerLegLeft.Texture = villagerManagerInstance.GetTextureByType(
			data.villagerType, BodyPartTextureType.LeftFoot);
	}

	public void ChangeHat(VillagerOccupation occupation)
	{
		
		if(!_hatMoved && occupation == VillagerOccupation.Woodcutter)
		{
			villagerHat.MoveLocalY(35);
			_hatMoved = true;
		}
		if(_hatMoved && occupation != VillagerOccupation.Woodcutter)
		{
			villagerHat.MoveLocalY(-35);
			_hatMoved = false;
		}
		villagerHat.Texture = villagerManagerInstance.allVillagerData.GetHatByOccupation(occupation);
	}

}
