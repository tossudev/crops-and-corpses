using Godot;
using System;

public partial class VillagerFaceButton : Button
{
	TextureRect _faceSprite;
	const string FACE_SPRITE_NODENAME = "%FaceSprite";
	
	
	Villager _currentResident;
	public int id;

	public override void _Pressed()
	{
		base._Pressed();
		
		_currentResident.OpenDialogue();
	}

	public void InitButton(Villager residingVillager)
	{
		_faceSprite = GetNode<TextureRect>(FACE_SPRITE_NODENAME);
		
		_currentResident = residingVillager;

		_faceSprite.Texture = _currentResident.villagerInfo.villagerHeadTexture;
			
		id = residingVillager.rawData.id;
	}
}
