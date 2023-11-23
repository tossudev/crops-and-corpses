using Godot;
using System;

public partial class VillagerInfo : Control
{
	ColorRect _backgroundColor;
	
	TextureRect _villagerHatTextureRect;
	Texture2D _villagerHatTexture;
	public Texture2D villagerHatTexture => _villagerHatTexture;
	
	TextureRect _villagerFaceTextureRect;
	
	Texture2D _villagerFaceTexture;
	public Texture2D villagerFaceTexture => _villagerFaceTexture;
	
	RichTextLabel _nameText;
	RichTextLabel _loreText;
	RichTextLabel _statusText;
	
	
	public override void _Ready()
	{
		_backgroundColor = GetNode<ColorRect>("ColorRect");

		_villagerFaceTextureRect = GetNode<TextureRect>("ColorRect/VillagerTexture");
		_nameText = GetNode<RichTextLabel>("ColorRect/NameText");
		_loreText = GetNode<RichTextLabel>("ColorRect/LoreText");
		_statusText = GetNode<RichTextLabel>("ColorRect/StatusText");

		Visible = false;
	}

	public void InitializeVillagerInfo(Texture2D villagerFaceTexture, string villagerName, string villagerLore, VillagerState villagerState)
	{
		//TODO: Correct textures setter
		// _villagerHatTexture = 
		// _villagerHatTextureRect.Texture = _villagerHatTexture;
		
		_villagerFaceTexture = villagerFaceTexture;
		_villagerFaceTextureRect.Texture = _villagerFaceTexture;
		
		_nameText.Text = villagerName;
		_loreText.Text = villagerLore;
		UpdateStatus(villagerState);
		
	}


	public void UpdateStatus(VillagerState villagerState){
		switch(villagerState){
			case VillagerState.RoamAround:
				_statusText.Text = "Status: Healthy";
				break;
			case VillagerState.FollowPlayer:
				_statusText.Text = "Status: Following";
				break;
			case VillagerState.FixFence:
				_statusText.Text = "Status: Repairing";
				break;
			case VillagerState.FindArcherTower:
				_statusText.Text = "Status: Defending";
				break;
			case VillagerState.FindShelter:
				_statusText.Text = "Status: Finding Cover";
				break;
			case VillagerState.ChooseTask:
				_statusText.Text = "Status: Healthy";
				break;
			case VillagerState.FarmingTask:
				_statusText.Text = "Status: Farming";
				break;
			
			case VillagerState.FindWoodTask:
				_statusText.Text = "Status: Cutting wood";
				break;
			
			case VillagerState.FindStoneTask:
				_statusText.Text = "Status: Mining stone";
				break;
		}
	}
	public void OpenInfo()
	{
		Visible = true;
	}
	
	public void CloseInfo()
	{
		Visible = false;
	}
}
